using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (Rigidbody))]
public class PlayerController : MonoBehaviour 
{
	private Rigidbody m_RB;

	private Vector3 m_Velocity;

	private void Start()
	{
		m_RB = GetComponent<Rigidbody> ();
	}

	public void Move(Vector3 velocity)
	{
		m_Velocity = velocity;
	}

	public void LookAt(Vector3 lookPoint)
	{
		transform.LookAt (lookPoint);
	}

	private void FixedUpdate()
	{
		m_RB.MovePosition(m_RB.position + m_Velocity * Time.fixedDeltaTime);
	}
}
