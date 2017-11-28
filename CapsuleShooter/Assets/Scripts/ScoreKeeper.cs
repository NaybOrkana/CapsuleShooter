using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreKeeper : MonoBehaviour 
{
	public static int m_Score { get; private set;}

	private float m_LastEnemyKillTime;
	private int m_StreakCount;
	private float m_StreakExpiryTime = 1;

	private void Start ()
	{
		m_Score = 0;
		Enemy.OnDeathStatic += OnEnemyKilled;
		FindObjectOfType<Player> ().OnDeath += OnPlayerDeath;
	}

	private void OnEnemyKilled()
	{
		if (Time.time < m_LastEnemyKillTime + m_StreakExpiryTime) 
		{
			m_StreakCount++;
		}
		else 
		{
			m_StreakCount = 0;
		}

		m_LastEnemyKillTime = Time.time;

		m_Score += 5 + (int)Mathf.Pow (2, m_StreakCount);
	}

	private void OnPlayerDeath()
	{
		Enemy.OnDeathStatic -= OnEnemyKilled;
	}
}
