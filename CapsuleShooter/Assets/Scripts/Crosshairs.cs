using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crosshairs : MonoBehaviour
{
	public LayerMask m_TargetMask;
	public float m_RotationSpeed;

	public Color m_DotHighlightColor;

	public SpriteRenderer m_Dot;

	private Color m_DotOriginalColor;

	private void Start ()
	{
		Cursor.visible = false;
		m_DotOriginalColor = m_Dot.color;
	}
	

	private void Update () 
	{
		transform.Rotate (Vector3.forward * m_RotationSpeed * Time.deltaTime);
	}

	public void DetectRays(Ray ray)
	{
		 
		if (Physics.Raycast (ray, 100f, m_TargetMask)) 
		{
			m_Dot.color = m_DotHighlightColor;

		} 
		else 
		{
			m_Dot.color = m_DotOriginalColor;
		}
	}
}
