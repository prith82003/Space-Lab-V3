using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombatController : CombatController
{
	float attackTimer = 0f;
	bool canAttack = true;

	public float idleTimerMax;
	public float idleTimer;

	private Item itemEquipped = null;

	private void Update()
	{
		if (Input.GetMouseButton(0))
			idleTimer = 0f;
		else
			idleTimer += Time.deltaTime;

		HandleInput();

		if (!itemEquipped) return;

		if (idleTimer >= idleTimerMax)
			itemEquipped.Idle();
		else
			itemEquipped.StopIdle();
	}

	private void HandleInput()
	{
		Item item = InventoryManager.GetSelectedItem();
	}
}
