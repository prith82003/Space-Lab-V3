using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Weapon/Sword")]
public class Sword : Weapon
{
	private void Awake()
	{
		this.attackType = DamageType.Melee;
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		if (isAttacking)
			Debug.Log("Sword hit " + other.name);
	}
}
