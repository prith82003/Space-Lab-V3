using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	public MovementController movementController;
	public CombatController combatController;
	public static GameObject player;

	private void Awake()
	{
		player = gameObject;
	}

	private void Update()
	{
		if (Input.GetMouseButtonDown(0))
		{
			Item selectedItem = InventoryManager.GetSelectedItem();

			if (selectedItem != null)
				selectedItem.UsePrimary();
		}
		else if (Input.GetMouseButtonDown(1))
		{
			Item selectedItem = InventoryManager.GetSelectedItem();

			if (selectedItem != null)
				selectedItem.UseSecondary();
		}

	}
}
