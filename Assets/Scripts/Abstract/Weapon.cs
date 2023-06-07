using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Animations;


public enum DamageType { Melee, Ranged, Magic, Healer };
public abstract class Weapon : Item
{
	public float damage;
	public float critChance;
	public DamageType attackType;
	public bool isAttacking;
	[SerializeField] private AnimatorController animController;
	private Animator anim;
	private GameObject itemObj;

	// private void Update()
	// {
	// 	if (Input.GetMouseButton(0))
	// 		anim.SetBool("Attack", true);
	// 	else
	// 		anim.SetBool("Attack", false);
	// }

	public override void UsePrimary() => Attack();

	public override void PickUp(GameObject obj)
	{
		base.PickUp(obj);
		anim = gameObject.GetComponent<Animator>();
		Debug.Log("Picked Up");
		Debug.Log("GameObject: " + gameObject.name);
	}

	public override void OnHold()
	{
		anim.runtimeAnimatorController = animController;
	}

	public virtual void Attack()
	{
		anim.SetTrigger("Use");
		// Default Attack
		SpecialAttack();
	}

	public virtual void SpecialAttack()
	{
		// Default Special
	}

	public override void Idle()
	{
		anim.SetBool("Idle", true);
	}

	public override void StopIdle()
	{
		anim.SetBool("Idle", false);
	}
}
