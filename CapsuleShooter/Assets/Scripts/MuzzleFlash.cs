using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MuzzleFlash : MonoBehaviour
{
	public GameObject m_FlashHolder;

	public Sprite[] m_FlashSprites;
	public SpriteRenderer[] m_FlashRenderes;

	public float m_FlashTime;

	private void Start()
	{
		Deactivate ();
	}

	public void Activate()
	{
		m_FlashHolder.SetActive (true);

		int flashSpriteIndex = Random.Range (0, m_FlashSprites.Length);

		for (int i = 0; i < m_FlashRenderes.Length; i++) 
		{
			m_FlashRenderes [i].sprite = m_FlashSprites [flashSpriteIndex];
		}

		Invoke ("Deactivate", m_FlashTime);
	}

	public void Deactivate()
	{
		m_FlashHolder.SetActive (false);
	}
}
