using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LivingEntity : MonoBehaviour, IDamageable
{
	public float m_StartingHealth;

	public  float m_Health { get; protected set;}
	protected bool m_IsItDead;

	public event System.Action OnDeath;

	protected virtual void Start()
	{
		m_Health = m_StartingHealth;
	}

	public virtual void TakeHit (float damage, Vector3 hitPoint, Vector3 hitDdirection)
	{
		TakeDamage (damage);
	}

	public virtual void TakeDamage(float damage)
	{

		m_Health -= damage;

		if (m_Health <= 0f && !m_IsItDead) 
		{
			Die ();
		}	
	}

	public virtual void Die()
	{
		m_IsItDead = true;

		if (OnDeath != null) 
		{
			OnDeath ();
		}

		GameObject.Destroy (gameObject);
	}
}
