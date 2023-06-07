using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;


public enum ItemType { Weapon, Armor, Consumable, Accessory, Misc }
public abstract class Item : ScriptableObject
{
	private long itemId;
	public string itemName;
	[SerializeField] private Sprite sprite;
	public GameObject gameObject;
	public ItemType itemType;
	public bool autoUse;
	public float speedPrimary;
	bool canAtackPrimary;

	public float speedSecondary;
	bool canAttackSecondary;

	private static InventoryManager inventoryManager;
	private bool canUse;

	public bool stackable;
	public int quantity;

	private void OnEnable()
	{
		inventoryManager = FindObjectOfType<InventoryManager>();
	}

	private void UseItem(float attackSpeed, bool primaryAttackType)
	{
		if (primaryAttackType)
			canAtackPrimary = false;
		else
			canAttackSecondary = false;


		Timer timer = null;

		timer = new Timer((obj) =>
		{
			MakeUseable(primaryAttackType);
			timer.Dispose();
		},
		null, 1000, Timeout.Infinite);
	}

	private void MakeUseable(bool primaryAttackType)
	{
		if (primaryAttackType)
			canAtackPrimary = true;
		else
			canAttackSecondary = true;
	}

	/// <summary>
	/// Action performed when LMB is pressed
	/// </summary>
	public virtual void UsePrimary()
	{
		if (!canAtackPrimary) return;
		UseItem(speedPrimary, true);
	}

	/// <summary>
	/// Action performed when RMB is pressed
	/// </summary>
	public virtual void UseSecondary()
	{
		if (!canAttackSecondary) return;
		UseItem(speedSecondary, false);
	}

	/// <summary>
	/// Called when Item is equipped into Armor/Accessory Slot
	/// </summary>
	public virtual void Equip() { }

	/// <summary>
	/// Called when Item is unequipped from Armor/Accessory Slot
	/// </summary>
	public virtual void Unequip() { }

	/// <summary>
	/// Called when Item is picked up into inventory
	/// </summary>
	public virtual void PickUp(GameObject obj)
	{
		gameObject = obj;
	}

	/// <summary>
	/// Called when Item is dropped from inventory
	/// </summary>
	public virtual void Drop() { }

	/// <summary>
	/// Calls Idle Animation
	/// </summary>
	public virtual void Idle() { }

	/// <summary>
	/// Stops Idle Animation
	/// </summary>
	public virtual void StopIdle() { }

	public virtual void OnHold() { }

	public Sprite GetSprite() => sprite;
}
