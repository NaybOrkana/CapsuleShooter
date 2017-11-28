using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundLibrary : MonoBehaviour 
{
	public SoundGroup[] m_SoundGroups;

	private Dictionary<string, AudioClip[]> m_GroupDictionary = new Dictionary<string, AudioClip[]> ();

	private void Awake()
	{
		foreach (SoundGroup soundGroup in m_SoundGroups) 
		{
			m_GroupDictionary.Add(soundGroup.groupID, soundGroup.group);
		}
	}

	public AudioClip GetClipFromName(string name)
	{
		if (m_GroupDictionary.ContainsKey(name)) 
		{
			AudioClip[] sounds = m_GroupDictionary [name];
			return sounds [Random.Range (0, sounds.Length)];
		}
		return null;
	}

	[System.Serializable]
	public class SoundGroup
	{
		public string groupID;
		public AudioClip[] group;
	}
}
