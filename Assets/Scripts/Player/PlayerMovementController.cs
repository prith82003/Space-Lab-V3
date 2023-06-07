using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class PlayerMovementController : MovementController
{
	[Header("Face Direction")]
	[SerializeField] float biasLeft = .65f;
	[SerializeField] float biasRight = .35f;
	[SerializeField] bool faceMouse = true;
	int prevDir = 1;

	[Header("Jump")]
	[SerializeField] bool isGrounded;
	[SerializeField] float coyoteTimer;
	[SerializeField] float extraGravForce;
	[SerializeField] LayerMask groundLayer;
	[SerializeField] Transform groundCheck;
	[SerializeField] float groundRadius;
	bool jumpPressed = false;
	public int jumpsLeft;
	public int maxJumps;
	public float extraJumpForceMult;
	GameObject iris;
	Transform irisOrigin;
	public float maxIrisDist;
	CinemachineFramingTransposer vcamTransposer;

	[Header("Debug")]
	public float x;

	private void Start()
	{
		iris = GameObject.Find("Iris");
		irisOrigin = iris.transform.parent;
		jumpsLeft = maxJumps;
		vcamTransposer = FindObjectOfType<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineFramingTransposer>();
	}

	// ! Replace Keycodes with Buttons

	private void FixedUpdate()
	{
		bool isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, groundLayer);

		if (canJump && !this.isGrounded) canJump = false;

		if (jumpPressed)
		{
			float mult = (this.isGrounded) ? 1 : extraJumpForceMult;

			rb.AddForce(Vector2.up * jumpSpeed * mult, ForceMode2D.Impulse);
			jumpPressed = false;
		}

		if (isGrounded)
			this.isGrounded = true;
		else
			Invoke("Ungrounded", coyoteTimer);


		if (this.isGrounded)
		{
			Debug.Log("Grounded");
			jumpsLeft = maxJumps;
		}
	}

	void Ungrounded()
	{
		isGrounded = false;
	}

	private void Update()
	{
		FaceDirection();

		if (!InputManager.BlockMovement)
			HandleMovement();
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(groundCheck.position, groundRadius);
	}

	void FaceDirection()
	{
		Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

		bool faceLeft = true;

		if (faceMouse)
			faceLeft = (mousePos.x < transform.position.x);
		else
			faceLeft = (prevDir < 0);

		if (faceLeft)
		{
			transform.localScale = new Vector3(-1, 1, 1);
			vcamTransposer.m_ScreenX = biasLeft;
		}
		else
		{
			transform.localScale = new Vector3(1, 1, 1);
			vcamTransposer.m_ScreenX = biasRight;
		}

		var mouseSightLine = mousePos - (Vector2)irisOrigin.position;
		mouseSightLine = mouseSightLine.normalized * maxIrisDist;

		iris.transform.position = (Vector2)irisOrigin.position + mouseSightLine;
	}

	void HandleMovement()
	{
		x = Input.GetAxisRaw("Horizontal");

		if (x != 0)
			prevDir = (int)x;

		rb.AddForce(Vector2.right * x * speed);

		if (Input.GetKeyDown(KeyCode.Space) && jumpsLeft > 0)
		{
			jumpsLeft--;
			jumpPressed = true;
		}
	}
}
