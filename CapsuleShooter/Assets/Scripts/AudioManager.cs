using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour 
{
	public enum AudioChannel
	{
		Master, SFX, Music
	};

	public static AudioManager m_Instance;

	public float m_MasterVolumePercent {get ; private set;}
	public float m_SFXVolumePercent {get ; private set;}
	public float m_MusicVolumePercent {get ; private set;}

	private AudioSource m_SFX2DSource;
	private AudioSource[] m_MusicSources;
	private SoundLibrary m_Library;
	private int m_ActiveMusicSourceIndex;

	private Transform m_AudioListener;
	private Transform m_PlayerTransform;

	private void Awake()
	{
		if (m_Instance != null) 
		{
			Destroy (gameObject);
		} 
		else
		{
			m_Instance = this;

			m_Library = GetComponent<SoundLibrary> ();

			m_MusicSources = new AudioSource[2];

			for (int i = 0; i < 2; i++) 
			{
				GameObject newMusicSource = new GameObject ("Music Source" + (i + 1));
				m_MusicSources [i] = newMusicSource.AddComponent<AudioSource> ();
				newMusicSource.transform.parent = transform;
			}

			GameObject newSFX2DSource = new GameObject ("SFX2DSource");
			m_SFX2DSource = newSFX2DSource.AddComponent<AudioSource> ();
			newSFX2DSource.transform.parent = transform;

			m_AudioListener = FindObjectOfType<AudioListener> ().transform;
			if (FindObjectOfType<Player>() != null) 
			{
				m_PlayerTransform = FindObjectOfType<Player> ().transform;

			}

			m_MasterVolumePercent = PlayerPrefs.GetFloat ("Master Vol", 1);
			m_SFXVolumePercent = PlayerPrefs.GetFloat ("SFX Vol", 1);
			m_MusicVolumePercent = PlayerPrefs.GetFloat ("Music Vol", 1);
		}
	}

	private void Update()
	{
		if (m_PlayerTransform != null) 
		{
			m_AudioListener.position = m_PlayerTransform.position;
		}
	}

	public void SetVolume(float volumePercent, AudioChannel channel)
	{
		switch (channel) 
		{
		case AudioChannel.Master:
			m_MasterVolumePercent = volumePercent;
			break;

		case AudioChannel.SFX:
			m_SFXVolumePercent = volumePercent;
			break;

		case AudioChannel.Music:
			m_MusicVolumePercent = volumePercent;
			break;
		}

		m_MusicSources [0].volume = m_MusicVolumePercent * m_MasterVolumePercent;
		m_MusicSources [1].volume = m_MusicVolumePercent * m_MasterVolumePercent;

		PlayerPrefs.SetFloat ("Master Vol", m_MasterVolumePercent);
		PlayerPrefs.SetFloat ("SFX Vol", m_SFXVolumePercent);
		PlayerPrefs.SetFloat ("Music Vol", m_MusicVolumePercent);
		PlayerPrefs.Save ();

	}

	public void PlayMusic(AudioClip clip, float fadeDuration = 1f)
	{
		m_ActiveMusicSourceIndex = 1 - m_ActiveMusicSourceIndex;
		m_MusicSources [m_ActiveMusicSourceIndex].clip = clip;
		m_MusicSources [m_ActiveMusicSourceIndex].Play ();

		StartCoroutine (AnimateMusicCrossFade(fadeDuration));
	}

	public void PlaySound(AudioClip clip, Vector3 pos)
	{
		if (clip != null) 
		{
			AudioSource.PlayClipAtPoint (clip, pos, m_SFXVolumePercent * m_MasterVolumePercent);
		}
	}

	public void PlaySound(string soundName, Vector3 pos)
	{
		PlaySound (m_Library.GetClipFromName (soundName), pos);
	}

	public void PlaySound2D(string soundName)
	{
		m_SFX2DSource.PlayOneShot (m_Library.GetClipFromName (soundName), m_SFXVolumePercent * m_MasterVolumePercent);
	}

	private IEnumerator AnimateMusicCrossFade(float duration)
	{
		float percent = 0;

		while (percent < 1) 
		{
			percent += Time.deltaTime * 1 / duration;
			m_MusicSources [m_ActiveMusicSourceIndex].volume = Mathf.Lerp (0, m_MusicVolumePercent * m_MasterVolumePercent, percent);
			m_MusicSources [1 - m_ActiveMusicSourceIndex].volume = Mathf.Lerp ( m_MusicVolumePercent * m_MasterVolumePercent, 0, percent);

			yield return null;

		}
	}
}
