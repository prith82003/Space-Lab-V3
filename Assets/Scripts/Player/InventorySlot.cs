using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
	[SerializeField] private InventoryItem item = null;
	[SerializeField] private int index;
	[SerializeField] private static Transform itemsParent = null;
	public bool hotbarSlot;
	public bool selected;
	private static Color SELECTED_COLOR = new Color(1f, 0.9801697f, 0.4386792f, 1f);
	private InventorySlot matchSlot;

	private void Awake()
	{
		if (!hotbarSlot)
			InventoryManager.InitialiseInventory += Initialise;
		else
			InventoryManager.InitialiseHotbar += InitialiseHotbar;
		InventoryManager.OnOpenInventory += OnOpenInventory;
		InventoryManager.OnCloseInventory += OnCloseInventory;
		InventoryManager.OnUpdateInventory += OnCloseInventory;

		itemsParent = GameObject.Find("Items").transform;
	}

	private void Update()
	{
		if (selected)
			GetComponent<Image>().color = SELECTED_COLOR;
		else
			GetComponent<Image>().color = Color.white;
	}

	private void Initialise()
	{
		string res = Regex.Match(gameObject.name, @"\d+").Value;
		index = (res == "") ? 0 : int.Parse(res);
		InventoryManager.SetInventorySlot(index, this);
	}

	private void InitialiseHotbar()
	{
		string num = Regex.Match(gameObject.name, @"\d+").Value;
		int match = (num == "") ? 0 : int.Parse(num);

		match = match % 40;

		matchSlot = InventoryManager.GetInventorySlot(match);
		InventoryManager.SetHotbarSlot(match, this);
	}

	public void SetItem(InventoryItem item)
	{
		this.item = item;
		item.transform.SetParent(itemsParent);
		item.slot = this;
		item.transform.position = transform.position;
	}
	public Item GetItem()
	{
		if (item == null)
			return null;
		return item.GetItem();
	}

	public InventoryItem GetInventoryItem() => item;

	public void RemoveItem() => item = null;

	public static InventorySlot operator +(InventorySlot slot, int quantity)
	{
		slot.AddQuantity(quantity);
		return slot;
	}

	private bool AddQuantity(int quantity)
	{
		if (!item.GetItem().stackable)
			return false;

		item.GetItem().quantity += quantity;
		return true;
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		// Debug.Log("Pointer Enter: " + name);
		InventoryManager.destinationSlot = this;
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		// Debug.Log("Pointer Exit: " + name);
		InventoryManager.destinationSlot = null;
	}

	private void OnOpenInventory()
	{
		if (!hotbarSlot) return;

		if (item != null)
		{
			item.transform.SetParent(itemsParent);
			item.transform.position = matchSlot.transform.position;
		}
	}

	private void OnCloseInventory()
	{
		if (!hotbarSlot) return;

		item = matchSlot.GetInventoryItem();

		if (item != null)
		{
			item.transform.SetParent(transform);
			item.transform.localPosition = Vector3.zero;
		}
	}
}
