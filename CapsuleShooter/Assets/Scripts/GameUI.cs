using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameUI : MonoBehaviour 
{
	public Image m_FadePlane;
	public GameObject m_GameOverUI;
	public float m_FadingSpeed = 1f;

	public RectTransform m_NewWaveBanner;
	public RectTransform m_HealthBar;
	public Text m_NewWaveTitle;
	public Text m_NewWaveEnemyCount;
	public Text m_ScoreUI;
	public Text m_GameOverScoreUI;

	private Spawner m_Spawner;
	private Player m_Player;

	private void Start()
	{
		m_Player = FindObjectOfType<Player> ();
		m_Player.OnDeath += OnGameOver;
	}

	private void Awake()
	{
		m_Spawner = FindObjectOfType<Spawner> ();
		m_Spawner.OnNewWave += OnNewWave;
	}

	private void Update()
	{
		m_ScoreUI.text = ScoreKeeper.m_Score.ToString ("D7");

		float healthPercent = 0;
		if (m_Player != null) 
		{
			healthPercent = m_Player.m_Health / m_Player.m_StartingHealth;
		}

		m_HealthBar.localScale = new Vector3 (healthPercent, 1, 1);

	}

	void OnNewWave (int waveNumber)
	{
		m_NewWaveTitle.text = "- Wave " + (waveNumber-1) + " -";

		string enemyCountString = ((m_Spawner.m_Waves [waveNumber - 1].infinite) ? "Infinite" : m_Spawner.m_Waves [waveNumber - 1].enemyCount + "");

		m_NewWaveEnemyCount.text = "Enemies: " + enemyCountString; 


		StopCoroutine ("AnimateNewWaveBanner");
		StartCoroutine ("AnimateNewWaveBanner");
	}
		
	private IEnumerator AnimateNewWaveBanner()
	{
		float delayTime = 2f;
		float animSpeed = 2f;
		float animatePercent = 0f;
		int dir = 1;

		float endDelayTime = Time.time + 1 / animSpeed + delayTime;

		while (animatePercent >= 0) 
		{
			animatePercent += Time.deltaTime * animSpeed * dir;

			if (animatePercent >= 1) 
			{
				animatePercent = 1;

				if (Time.time > endDelayTime) 
				{
					dir = -1;
				}
			}

			m_NewWaveBanner.anchoredPosition = Vector2.up * Mathf.Lerp (-220, 0, animatePercent);

			yield return null;
		}
	}
	
	void OnGameOver()
	{
		StartCoroutine (Fade(Color.clear, Color.black, m_FadingSpeed));
		m_GameOverScoreUI.text = m_ScoreUI.text;
		m_GameOverUI.SetActive (true);
		m_ScoreUI.gameObject.SetActive (false);
		m_HealthBar.transform.parent.gameObject.SetActive (false);
		Cursor.visible = true;
	}

	private IEnumerator Fade(Color fromColor, Color toColor, float time)
	{
		float speed = 1f / time;
		float percent = 0f;

		while (percent < 1f) 
		{
			percent += Time.deltaTime * speed;
			m_FadePlane.color = Color.Lerp (fromColor, toColor, percent);
			yield return null;
		}
	}

	//UI Input

	public void StartNewGame()
	{
		SceneManager.LoadScene ("DebugLevel");	
	}

	public void ReturnToMainMenu()
	{
		SceneManager.LoadScene ("Menu");	
	}
}
