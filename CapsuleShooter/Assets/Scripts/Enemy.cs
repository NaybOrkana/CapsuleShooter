using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent (typeof (NavMeshAgent))]
public class Enemy : LivingEntity 
{
	public float m_Damage = 1f;

	public ParticleSystem m_DeathEffect;

	public enum State{Idle, Chasing, Attacking};
	private State m_CurrentState;

	private Transform m_Target;
	private NavMeshAgent m_Pathfinder;

	private LivingEntity m_TargetEntity;
	private Material m_SkinMaterial;
	private Color m_OriginalColor;

	private float m_AttackDistanceThreshold = 1.5f;
	private float m_TimeBetweenAttacks = 1f;
	private float m_NextAttackTime;
	private float m_EnemyCollisionRadius;
	private float m_TargetCollisionRadius;

	private bool m_HasTarget;

	private void Awake()
	{
		m_Pathfinder = GetComponent<NavMeshAgent> ();

		if (GameObject.FindGameObjectWithTag("Player") != null) 
		{
			m_HasTarget = true;

			m_Target = GameObject.FindGameObjectWithTag ("Player").transform;
			m_TargetEntity = m_Target.GetComponent<LivingEntity> ();

			m_EnemyCollisionRadius = GetComponent<CapsuleCollider> ().radius;
			m_TargetCollisionRadius = GetComponent<CapsuleCollider> ().radius;
		}
	}

	protected override void Start()
	{
		base.Start ();
		m_Pathfinder = GetComponent<NavMeshAgent> ();

		if (m_HasTarget) 
		{
			m_CurrentState = State.Chasing;

			m_TargetEntity.OnDeath += OnTargetDeath;

			StartCoroutine (UpdatePath());
		}
	}

	public void SetParameters(float moveSpeed, int hitsToKillPlayer, float enemyHealth, Color skinColor)
	{
		m_Pathfinder.speed = moveSpeed;

		if (m_HasTarget)
		{
			m_Damage = Mathf.Ceil (m_TargetEntity.m_StartingHealth / hitsToKillPlayer);
		}

		m_StartingHealth = enemyHealth;

		m_SkinMaterial = GetComponent<Renderer> ().material;
		m_SkinMaterial.color = skinColor;
		m_OriginalColor = m_SkinMaterial.color;
	}

	public override void TakeHit (float damage, Vector3 hitPoint, Vector3 hitDdirection)
	{
		if (damage >= m_Health)
		{
			Destroy(Instantiate (m_DeathEffect.gameObject, hitPoint, Quaternion.FromToRotation (Vector3.forward, hitDdirection)) as GameObject , m_DeathEffect.main.startLifetime.constant);
		}

		base.TakeHit (damage, hitPoint, hitDdirection);
	}

	private void OnTargetDeath()
	{
		m_HasTarget = false;
		m_CurrentState = State.Idle;
	}

	private void Update()
	{
		if (m_HasTarget) 
		{
			if (Time.time > m_NextAttackTime) 
			{
				float sqrDstToTarget = (m_Target.position - transform.position).sqrMagnitude;

				if (sqrDstToTarget < Mathf.Pow(m_AttackDistanceThreshold + m_EnemyCollisionRadius + m_TargetCollisionRadius, 2))
				{
					m_NextAttackTime = Time.time + m_TimeBetweenAttacks;

					StartCoroutine (AttackProcedure ());
				}
			}

		}
	}


	private IEnumerator AttackProcedure()
	{
		m_CurrentState = State.Attacking;

		m_Pathfinder.enabled = false;

		Vector3 originalPosition = transform.position;
		Vector3 dirToTarget = (m_Target.position - transform.position).normalized;
		Vector3 attackPosition = m_Target.position - dirToTarget * (m_EnemyCollisionRadius);


		float attackSpeed = 3f;
		float percent = 0f;

		m_SkinMaterial.color = Color.red;

		bool hasAppliedDamage = false;

		while (percent <= 1f) 
		{
			if (percent >= 0.5f && !hasAppliedDamage) 
			{
				hasAppliedDamage = true;
				m_TargetEntity.TakeDamage (m_Damage);
			}

			percent += Time.deltaTime * attackSpeed;
			float interpolation = (-Mathf.Pow(percent, 2f) + percent) * 4f;
			transform.position = Vector3.Lerp (originalPosition, attackPosition, interpolation);


			yield return null;
		}

		m_SkinMaterial.color = m_OriginalColor;

		m_CurrentState = State.Chasing;

		m_Pathfinder.enabled = true;

	}

	private IEnumerator UpdatePath()
	{
		//A coroutine is called as to only update the destination path a quarter of a second. The Enemies will always search for the player.
		float refreshRate = .25f;

		while (m_HasTarget)
		{
			if (m_CurrentState == State.Chasing) 
			{
				Vector3 dirToTarget = (m_Target.position - transform.position).normalized;
				Vector3 targetPosition = m_Target.position - dirToTarget * (m_EnemyCollisionRadius + m_TargetCollisionRadius + m_AttackDistanceThreshold/2f);
				if (!m_IsItDead) 
				{
					m_Pathfinder.SetDestination (targetPosition);
				}
			}
			yield return new WaitForSeconds (refreshRate);
		}
	}
}
