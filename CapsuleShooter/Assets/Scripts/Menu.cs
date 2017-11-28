using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour 
{
	public GameObject m_MainMenuHolder;
	public GameObject m_OptionsMenuHolder;

	public Slider[] m_VolumeSliders;
	public Toggle[] m_ResolutionToggles;
	public Toggle m_FullscreenToggle;
	public int[] m_ScreenWidths;

	private int m_ActiveScreenResIndex;

	private void Start()
	{
		m_ActiveScreenResIndex = PlayerPrefs.GetInt ("Screen res index");
		bool isFullScreen = (PlayerPrefs.GetInt ("fullscreen") == 1) ? true : false;

		m_VolumeSliders [0].value = AudioManager.m_Instance.m_MasterVolumePercent;
		m_VolumeSliders [1].value = AudioManager.m_Instance.m_SFXVolumePercent;
		m_VolumeSliders [2].value = AudioManager.m_Instance.m_MusicVolumePercent;

		for (int i = 0; i < m_ResolutionToggles.Length; i++) 
		{
			m_ResolutionToggles [i].isOn = i == m_ActiveScreenResIndex;
		}

		m_FullscreenToggle.isOn = isFullScreen;
	}

	public void Play()
	{
		SceneManager.LoadScene ("DebugLevel");
	}

	public void Quit()
	{
		Application.Quit ();
	}

	public void OptionsMenu()
	{
		m_MainMenuHolder.SetActive (false);
		m_OptionsMenuHolder.SetActive (true);
	}

	public void MainMenu()
	{
		m_MainMenuHolder.SetActive (true);
		m_OptionsMenuHolder.SetActive (false);
	}

	public void SetScreenResolution(int i)
	{
		if (m_ResolutionToggles[i].isOn) 
		{
			m_ActiveScreenResIndex = i;	
			float aspectRatio = 16 / 9f;
			Screen.SetResolution (m_ScreenWidths[i], (int)(m_ScreenWidths[i] / aspectRatio), false);
			PlayerPrefs.SetInt ("Screen res index", m_ActiveScreenResIndex);
			PlayerPrefs.Save ();
		}
	}

	public void SetFullscreen(bool isFullScreen)
	{
		for (int i = 0; i < m_ResolutionToggles.Length; i++) 
		{
			m_ResolutionToggles [i].interactable = !isFullScreen;
		}

		if (isFullScreen) 
		{
			Resolution[] allResolutions = Screen.resolutions;
			Resolution maxResolution = allResolutions [allResolutions.Length - 1];
			Screen.SetResolution (maxResolution.width, maxResolution.height, true);
		} 
		else 
		{
			SetScreenResolution (m_ActiveScreenResIndex);
		}

		PlayerPrefs.SetInt ("fullscreen", (isFullScreen) ? 1 : 0);
		PlayerPrefs.Save ();
	}

	public void SetMasterVolume(float value)
	{
		AudioManager.m_Instance.SetVolume (value, AudioManager.AudioChannel.Master);
	}

	public void SetSFXVolume(float value)
	{
		AudioManager.m_Instance.SetVolume (value, AudioManager.AudioChannel.SFX);
	}

	public void SetMusicVolume(float value)
	{
		AudioManager.m_Instance.SetVolume (value, AudioManager.AudioChannel.Music);
	}
}
