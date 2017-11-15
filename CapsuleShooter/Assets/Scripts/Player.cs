using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (PlayerController))]
[RequireComponent (typeof (GunController))]
public class Player : LivingEntity
{
	public float m_MoveSpeed = 5f;

	public Crosshairs m_Crosshairs;

	private PlayerController m_Controller;
	private GunController m_GunController;
	private Camera m_ViewCamera;

	protected override void Start () 
	{
		base.Start ();
		m_Controller = GetComponent<PlayerController> ();
		m_GunController = GetComponent<GunController> ();

		m_ViewCamera = Camera.main;
	}
	

	private void Update () 
	{
		// Movement input: Store the axis for movement and apply them with the desired speed. The input is normalized for a better direction input.
		Vector3 moveInput = new Vector3 (Input.GetAxisRaw ("Horizontal"), 0f, Input.GetAxisRaw ("Vertical"));
		Vector3 moveVelocity = moveInput.normalized * m_MoveSpeed;

		m_Controller.Move (moveVelocity);

		// Look input: A ray from the camera is casted into an invisible plane, the player will always be looking at that specific point which is determined by the mouse cursor's position.
		Ray cursorRay = m_ViewCamera.ScreenPointToRay (Input.mousePosition);
		Plane groundPlane = new Plane (Vector3.up, Vector3.up * m_GunController.GetHeight);
		float rayDistance;

		if (groundPlane.Raycast(cursorRay, out rayDistance)) 
		{
			Vector3 point = cursorRay.GetPoint (rayDistance);
			//Debug.DrawLine (cursorRay.origin, point, Color.red);

			m_Controller.LookAt (point);

			m_Crosshairs.transform.position = point;
			//m_Crosshairs.DetectRays (cursorRay);
		}

		// Weapon input: Whenver the left mouse button is clicked and the player is able to. The gun will be triggered.

		if (Input.GetMouseButton(0))
		{
			m_GunController.OnTriggerHold ();
		}

		if (Input.GetMouseButtonUp(0))
		{
			m_GunController.OnTriggerReleased ();
		}
	}
}
