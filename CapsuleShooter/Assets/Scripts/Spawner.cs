using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour 
{
	public bool devMode;

	public Wave[] m_Waves;
	public Enemy m_Enemy;

	private LivingEntity m_PlayerEntity;
	private Transform m_PlayerTransform;

	private int m_EnemiesRemainingToSpawn;
	private int m_EnemiesRemainingAlive;
	private float m_NextSpawnTime;

	private Wave m_CurrentWave;
	private int m_CurrentWaveNumber;

	private MapGenerator m_Map;

	private float m_TimeBetweenCampingChecks = 3f;
	private float m_CampThresholdDistance = 1.5f;
	private float m_NextCampCheckTime;
	private Vector3 m_PreviousCampPosition;
	private bool m_IsCamping;

	private bool m_IsDisabled;

	public event System.Action<int> OnNewWave;

	private void Start()
	{
		m_PlayerEntity = FindObjectOfType<Player> ();
		m_PlayerTransform = m_PlayerEntity.transform;

		m_NextCampCheckTime = m_TimeBetweenCampingChecks + Time.time;
		m_PreviousCampPosition = m_PlayerTransform.position;

		m_PlayerEntity.OnDeath += OnPlayerDeath;

		m_Map = FindObjectOfType<MapGenerator> ();
		NextWave ();
	}

	private void Update()
	{
		if (!m_IsDisabled)
		{
			if (Time.time > m_NextCampCheckTime)
			{
				m_NextCampCheckTime = Time.time + m_TimeBetweenCampingChecks;

				m_IsCamping = (Vector3.Distance (m_PlayerTransform.position, m_PreviousCampPosition) < m_CampThresholdDistance);
				m_PreviousCampPosition = m_PlayerTransform.position;
			}

			if ((m_EnemiesRemainingToSpawn > 0 || m_CurrentWave.infinite) && Time.time > m_NextSpawnTime) 
			{
				m_EnemiesRemainingToSpawn--;
				m_NextSpawnTime = Time.time + m_CurrentWave.timeBetweenSpawns;

				StartCoroutine ("SpawnEnemies");
			}
		}

		if (devMode)
		{
			if (Input.GetKeyDown(KeyCode.X)) 
			{
				StopCoroutine ("SpawnEnemies");
				foreach (Enemy enemy  in FindObjectsOfType<Enemy>()) 
				{
					GameObject.Destroy (enemy.gameObject);
				}

				NextWave ();
			}
		}
	}

	private IEnumerator SpawnEnemies()
	{
		float spawnDelay = 1.5f;
		float tileFlashSpeed = 5f;

		Transform randomTile = m_Map.GetRandomOpenTile ();

		if (m_IsCamping) 
		{
			randomTile = m_Map.GetTileFromPosition (m_PlayerTransform.position);
		}

		Material tileMaterial = randomTile.GetComponent<Renderer> ().material;
		Color initialColor = tileMaterial.color;
		Color flashingColor = Color.red;
		float spawnTimer = 0f;

		while (spawnTimer < spawnDelay)
		{
			tileMaterial.color = Color.Lerp (initialColor, flashingColor, Mathf.PingPong (spawnTimer * tileFlashSpeed, 1f));

			spawnTimer += Time.deltaTime;
			yield return null;
		}

		Enemy spawnedEnemy = Instantiate (m_Enemy, randomTile.position + Vector3.up, Quaternion.identity) as Enemy;
		spawnedEnemy.OnDeath += SpawnedEnemy_OnDeath;
		spawnedEnemy.SetParameters (m_CurrentWave.moveSpeed, m_CurrentWave.hitsToKillPlayer, m_CurrentWave.enemyHealth, m_CurrentWave.skinColor);
	}

	void OnPlayerDeath()
	{
		m_IsDisabled = true;
	}

	void SpawnedEnemy_OnDeath ()
	{
		m_EnemiesRemainingAlive--;

		if (m_EnemiesRemainingAlive == 0) 
		{
			NextWave ();
		}
	}

	 void ResetPlayerPosition ()
	{
		m_PlayerTransform.position = m_Map.GetTileFromPosition (Vector3.zero).position + Vector3.up * 3f;
	}

	private void NextWave()
	{
		if (m_CurrentWaveNumber > 0) {
			AudioManager.m_Instance.PlaySound2D ("Level Complete");
		}
		m_CurrentWaveNumber++;

		if (m_CurrentWaveNumber - 1 < m_Waves.Length)
		{
			m_CurrentWave = m_Waves [m_CurrentWaveNumber - 1];

			m_EnemiesRemainingToSpawn = m_CurrentWave.enemyCount;
			m_EnemiesRemainingAlive = m_EnemiesRemainingToSpawn;

			if (OnNewWave != null) 
			{
				OnNewWave (m_CurrentWaveNumber);
			}

			ResetPlayerPosition ();
		}
	}

	[System.Serializable]
	public class Wave
	{
		public bool infinite;
		public int enemyCount;
		public float timeBetweenSpawns;

		public float moveSpeed;
		public int hitsToKillPlayer;
		public float enemyHealth;
		public Color skinColor;
	}
}
