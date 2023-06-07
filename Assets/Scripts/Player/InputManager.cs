using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
	public PlayerController playerController;

	[Header("Movement Controls")]
	public static KeyCode MoveForward = KeyCode.W;
	public static KeyCode MoveBackward = KeyCode.S;
	public static KeyCode MoveLeft = KeyCode.A;
	public static KeyCode MoveRight = KeyCode.D;
	public static KeyCode Jump = KeyCode.Space;

	[Header("Inventory Controls")]
	public static KeyCode Inventory = KeyCode.LeftAlt;
	public static KeyCode DropItem = KeyCode.T;
	public static KeyCode Escape = KeyCode.Escape;
	public static KeyCode ShiftItem = KeyCode.LeftShift;

	[Header("Control Block")]
	public static bool BlockMovement = false;
	public static bool BlockInventory = false;
	public static bool BlockAttack = false;

	// Scripts
	private PlayerMovementController movementController;
	private InventoryManager inventoryManager;

	private void Update()
	{
		if (inventoryManager.GetInventoryOpen())
		{
			BlockMovement = true;
			BlockAttack = true;
		}

		if (!inventoryManager.GetInventoryOpen())
		{
			BlockMovement = false;
			BlockAttack = false;
		}
	}
}
