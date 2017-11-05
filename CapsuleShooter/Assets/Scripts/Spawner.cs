using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour 
{
	public Wave[] m_Waves;
	public Enemy m_Enemy;

	private int m_EnemiesRemainingToSpawn;
	private int m_EnemiesRemainingAlive;
	private float m_NextSpawnTime;

	private Wave m_CurrentWave;
	private int m_CurrentWaveNumber;

	private void Start()
	{
		NextWave ();
	}

	private void Update()
	{
		if (m_EnemiesRemainingToSpawn > 0 && Time.time > m_NextSpawnTime) 
		{
			m_EnemiesRemainingToSpawn--;
			m_NextSpawnTime = Time.time + m_CurrentWave.timeBetweenSpawns;

			Enemy spawnedEnemy = Instantiate (m_Enemy, Vector3.zero, Quaternion.identity) as Enemy;
			spawnedEnemy.OnDeath += SpawnedEnemy_OnDeath;
		}
	}

	void SpawnedEnemy_OnDeath ()
	{
		m_EnemiesRemainingAlive--;

		if (m_EnemiesRemainingAlive == 0) 
		{
			NextWave ();
		}
	}

	private void NextWave()
	{
		m_CurrentWaveNumber++;

		if (m_CurrentWaveNumber - 1 < m_Waves.Length)
		{
			m_CurrentWave = m_Waves [m_CurrentWaveNumber - 1];

			m_EnemiesRemainingToSpawn = m_CurrentWave.enemyCount;
			m_EnemiesRemainingAlive = m_EnemiesRemainingToSpawn;
		}
	}

	[System.Serializable]
	public class Wave
	{
		public int enemyCount;
		public float timeBetweenSpawns;
	}
}
