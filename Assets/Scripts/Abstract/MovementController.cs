using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementController : MonoBehaviour
{
	public float speed;
	public bool entityCanJump;
	public bool canJump;
	public float jumpSpeed;
	protected Rigidbody2D rb;

	private void Awake()
	{
		rb = GetComponent<Rigidbody2D>();
	}
}
