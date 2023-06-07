using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryManager : MonoBehaviour
{
	static InventoryManager self;

	[Header("Inventory")]
	private static Inventory inventory;
	private static Inventory hotbar;
	[SerializeField] private bool inventoryOpen;
	[SerializeField] private LayerMask InventoryLayer;
	[SerializeField] private int maxInventorySize = 20;
	private GameObject inventoryUI;
	[SerializeField] private GameObject inventoryItemPrefab;
	public static System.Action InitialiseInventory;
	private List<GameObject> pullItems;
	[SerializeField] private float itemSpeed = 5f;
	private GameObject hotbarParent;

	public static System.Action OnOpenInventory;
	public static System.Action OnCloseInventory;
	public static System.Action InitialiseHotbar;
	public static System.Action OnUpdateInventory;

	[Header("Held Item")]
	[SerializeField] private static InventoryItem mouseItem;
	public static InventorySlot originSlot;
	public static InventorySlot destinationSlot;
	public static SpriteRenderer holdItemSprite;

	[Header("Hotbar")]
	[SerializeField] private int currentItemIndex = 0;

	private void Awake()
	{
		inventory = new Inventory(maxInventorySize);
		hotbar = new Inventory(10);

		inventoryUI = GameObject.Find("InventoryUI");
		hotbarParent = GameObject.Find("Hotbar");

		inventoryUI.SetActive(false);
		inventoryOpen = false;
		pullItems = new List<GameObject>();
		self = this;

		InitialiseInventory();
		InitialiseHotbar();
		hotbar.GetInventorySlot(0).selected = true;

		holdItemSprite = GameObject.Find("Hold Item").GetComponent<SpriteRenderer>();
	}

	private void Update()
	{
		if (Input.GetKeyDown(InputManager.Inventory))
			ToggleInventory();

		if (inventoryOpen)
		{
			if (Input.GetKeyDown(InputManager.Escape))
			{
				CloseInventory();
				goto AfterInventory;
			}
		}

	AfterInventory:

		if (pullItems.Count > 0)
		{
			foreach (var item in pullItems)
			{
				if (!inventory.Full())
				{
					item.GetComponent<Rigidbody2D>().gravityScale = .6f;
					item.transform.position = Vector2.MoveTowards(item.transform.position, transform.position, itemSpeed * Time.deltaTime);
				}
				else
					item.GetComponent<Rigidbody2D>().gravityScale = 1f;
			}
		}

		if (Input.mouseScrollDelta.y != 0)
		{
			hotbar.GetInventorySlot(currentItemIndex).selected = false;

			if (Input.mouseScrollDelta.y > 0)
				currentItemIndex--;
			else
				currentItemIndex++;

			currentItemIndex = LoopX(currentItemIndex, 0, 9);
			hotbar.GetInventorySlot(currentItemIndex).selected = true;
			GetSelectedItem()?.OnHold();
		}

		Item selected = GetSelectedItem();

		holdItemSprite.sprite = selected ? selected.GetSprite() : null;
	}

	private int LoopX(int x, int min, int max)
	{
		if (x < min)
			return max;
		else if (x > max)
			return min;
		else
			return x;
	}

	public int GetInventorySize() => inventory.GetInventorySize();
	public int GetInventoryCapacity() => maxInventorySize;
	public bool GetInventoryOpen() => inventoryOpen;

	public static void SetInventorySlot(int index, InventorySlot slot) => inventory[index] = slot;
	public static InventorySlot GetInventorySlot(int index) => inventory[index];

	public static void SetHotbarSlot(int index, InventorySlot slot) => hotbar[index] = slot;
	public static InventorySlot GetHotbarSlot(int index) => hotbar[index];

	public static Item GetSelectedItem() => hotbar[self.currentItemIndex].GetItem();

	void ToggleInventory()
	{
		inventoryOpen = !inventoryOpen;
		hotbarParent.SetActive(!inventoryOpen);

		if (!inventoryOpen)
			CloseInventory();
		else
			OpenInventory();
	}

	void OpenInventory()
	{
		OnOpenInventory();
		inventoryUI.SetActive(true);
	}

	void CloseInventory()
	{
		OnCloseInventory();
		inventoryUI.SetActive(false);
	}

	public bool PickUpItem(InventoryItem item) => inventory.ShiftItem(item);

	private void OnCollisionEnter2D(Collision2D other)
	{
		if (other.gameObject.CompareTag("Item"))
		{
			var item = other.gameObject.GetComponent<InventoryItem>();
			Debug.Log("Picked Up: " + item.name);
			if (pullItems.Contains(other.gameObject))
				pullItems.Remove(other.gameObject);

			var invItem = Instantiate(inventoryItemPrefab, inventoryUI.transform).GetComponent<InventoryItem>();
			invItem.SetItem(item.GetItem());
			invItem.PickUp(PlayerController.player.transform.GetChild(2).gameObject);

			if (inventory.ShiftItem(invItem, true))
			{
				invItem.GetItem().Equip();
				Destroy(other.gameObject);
			}
			else
			{
				Debug.LogWarning("Inventory Full");
				Destroy(invItem.gameObject);
			}

			OnUpdateInventory();
		}
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		if (other.CompareTag("Item"))
			pullItems.Add(other.gameObject);
	}

	public static void BeginDragItem(InventorySlot slot, InventoryItem item)
	{
		originSlot = slot;
		mouseItem = item;
	}

	public static void EndDragItem(InventoryItem item)
	{
		if (destinationSlot == null)
		{
			Debug.Log("Returning To Origin Slot");
			if (originSlot != null)
				originSlot.SetItem(item);
			else
				Debug.LogWarning("Origin Slot is Null");

			return;
		}

		if (destinationSlot.GetItem() == null)
		{
			Debug.Log("Moving Item to Empty Slot");
			destinationSlot.SetItem(item);
			originSlot.RemoveItem();
			return;
		}

		if (destinationSlot.GetItem() == item.GetItem())
		{
			Debug.Log("Stacking Item");
			destinationSlot += item.GetItem().quantity;
			originSlot.RemoveItem();
			Destroy(item);
			return;
		}

		OnUpdateInventory();
		originSlot.SetItem(item);

		mouseItem = null;
	}
}

public class Inventory
{
	InventorySlot[] inventory;
	int maxInventorySize;
	int currentInventorySize;

	public Inventory(int inventorySize)
	{
		inventory = new InventorySlot[inventorySize];
		maxInventorySize = inventorySize;
		currentInventorySize = 0;
	}

	public int GetInventorySize() => currentInventorySize;
	public bool Full() => currentInventorySize == maxInventorySize;

	public InventorySlot this[int key]
	{
		get => inventory[key];
		set => inventory[key] = value;
	}

	public void SetInventorySlot(InventorySlot slot, int index)
	{
		if (inventory == null)
		{
			Debug.Log("Inventory Null");
			return;
		}
		inventory[index] = slot;
	}

	public InventorySlot GetInventorySlot(int index)
	{
		if (inventory == null)
		{
			Debug.Log("Inventory Null");
			return null;
		}
		return inventory[index];
	}

	/// <summary>
	/// Quick Stack Item to Inventory
	/// </summary>
	/// <param name="item">Item to Stack</param>
	/// <returns>Succesfully Stacked</returns>
	public bool ShiftItem(InventoryItem invItem, bool addItem = false)
	{
		var item = invItem.GetItem();
		// Check if item is stackable
		if (item.stackable)
		{
			// If stackable, check if item is already in inventory
			if (SearchItem(item))
			{
				// If item is already in inventory, add quantity to existing item
				inventory[GetIndex(item)] += item.quantity;
				GameObject.Destroy(invItem.gameObject);
				return true;
			}

			// If item is not in inventory, add item to inventory
			if (currentInventorySize >= maxInventorySize)
				return false;

			try
			{
				inventory[GetNextIndex()].SetItem(invItem);
				if (addItem) currentInventorySize++;
			}
			catch { return false; }

			currentInventorySize++;
			return true;
		}

		// If not stackable, check if inventory is full
		if (currentInventorySize >= maxInventorySize)
			return false;

		// If inventory is not full, add item to inventory
		inventory[GetNextIndex()].SetItem(invItem);
		if (addItem)
			currentInventorySize++;
		return true;
	}

	private int GetNextIndex()
	{
		for (int i = 0; i < maxInventorySize; i++)
		{
			if (inventory[i].GetItem() == null)
			{
				return i;
			}
		}

		throw new System.Exception("Inventory is full");
	}

	private int GetIndex(Item item)
	{
		for (int i = 0; i < maxInventorySize; i++)
		{
			if (inventory[i].GetItem() == item)
				return i;
		}

		throw new System.Exception("Item not found in inventory");
	}

	private bool SearchItem(Item item)
	{
		foreach (var slot in inventory)
		{
			if (slot.GetItem() == item)
				return true;
		}

		return false;
	}

	/// <summary>
	/// Insert an Item into specific inventory slot
	/// </summary>
	/// <param name="item">Item to Insert</param>
	/// <param name="index">Inventory Slot to Insert into</param>
	public void InsertItem(InventoryItem item, int index)
	{

	}

	/// <summary>
	/// Drop an Item from Inventory
	/// </summary>
	/// <param name="item">Item to Drop</param>
	public void DropItem(InventoryItem item)
	{

	}
}