using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LivingEntity : MonoBehaviour, IDamageable
{
	public float m_StartingHealth;

	protected float m_Health;
	protected bool m_IsItDead;

	public event System.Action OnDeath;

	protected virtual void Start()
	{
		m_Health = m_StartingHealth;
	}

	public void TakeHit (float damage, RaycastHit hit)
	{
		TakeDamage (damage);
	}

	public void TakeDamage(float damage)
	{

		m_Health -= damage;

		if (m_Health <= 0f && !m_IsItDead) 
		{
			Die ();
		}	
	}

	protected void Die()
	{
		m_IsItDead = true;

		if (OnDeath != null) 
		{
			OnDeath ();
		}

		GameObject.Destroy (gameObject);
	}
}
