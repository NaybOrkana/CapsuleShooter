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

	private void Start ()
	{
		FindObjectOfType<Player> ().OnDeath += OnGameOver;
	}
	
	void OnGameOver()
	{
		StartCoroutine (Fade(Color.clear, Color.black, m_FadingSpeed));
		m_GameOverUI.SetActive (true);
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
}
