using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour 
{
	public AudioClip m_MainTheme;
	public AudioClip m_MenuTheme;

	private string m_SceneName;

	private void Start()
	{
		OnLevelLoad (0);
	}

	private void OnLevelLoad(int sceneIndex)
	{
		string newSceneName = SceneManager.GetActiveScene ().name;

		if (newSceneName != m_SceneName) {
			m_SceneName = newSceneName;
			Invoke ("PlayMusic", .2f);
		}
	}

	private void PlayMusic()
	{
		AudioClip clipToPlay = null;

		if (m_SceneName == "Menu")
		{
			clipToPlay = m_MenuTheme;
		} else 
		{
			clipToPlay = m_MainTheme;
		}

		if (clipToPlay != null) 
		{
			AudioManager.m_Instance.PlayMusic (clipToPlay, 2);
			Invoke ("PlayMusic", clipToPlay.length);
		}
	}


}
