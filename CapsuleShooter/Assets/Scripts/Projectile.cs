﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
	public float m_Speed = 10f;
	public float m_Damage = 1f;

	public LayerMask m_CollisionMask;
	public Color m_TrailColor;

	private float m_Lifetime = 3f;
	private float m_SkinWidth = .1f;

	private void Start()
	{
		Destroy (gameObject, m_Lifetime);

		Collider[] initialCollitions = Physics.OverlapSphere (transform.position, .1f, m_CollisionMask);
		if (initialCollitions.Length > 0)
		{
			OnHitObject (initialCollitions[0], transform.position);
		}

		GetComponent<TrailRenderer> ().material.SetColor ("_TintColor", m_TrailColor);
	}

	public void SetSpeed(float newSpeed)
	{
		m_Speed = newSpeed;
	}

	private void Update () 
	{
		float moveDistance = m_Speed * Time.deltaTime;
		CheckCollisions (moveDistance);

		transform.Translate (Vector3.forward * Time.deltaTime * m_Speed);
	}

	private void CheckCollisions (float moveDistance)
	{
		//A ray is casted whenever a projectile is shot as to know if it's able to hit a target. This is done to avoid the possible error of triggers missing the collision cause of the speed of the projectile
		Ray ray = new Ray (transform.position, transform.forward);
		RaycastHit hit;

		if (Physics.Raycast(ray, out hit, moveDistance + m_SkinWidth, m_CollisionMask, QueryTriggerInteraction.Collide)) 
		{
			OnHitObject (hit.collider, hit.point);
		}
	}



	private void OnHitObject(Collider c, Vector3 hitPoint)
	{
		//Whenever a target is hit, the damage is dealt via the IDamageable interface and the bullet is destroyed.
		IDamageable damageableObject = c.GetComponent<IDamageable> ();

		if (damageableObject != null)
		{
			damageableObject.TakeHit (m_Damage, hitPoint, transform.forward);
		}

		GameObject.Destroy (gameObject);
	}
}
