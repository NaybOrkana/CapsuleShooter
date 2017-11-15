using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shell : MonoBehaviour 
{
	public Rigidbody m_RB;

	public float m_ForceMin;
	public float m_ForceMax;

	private float m_LifeTime = 4f;
	private float m_FadeTime = 2f;

	private void Start()
	{
		float force = Random.Range (m_ForceMin, m_ForceMax);

		m_RB.AddForce (transform.right * force);
		m_RB.AddTorque (Random.insideUnitSphere * force);

		StartCoroutine (FadeShell ());
	}

	private IEnumerator FadeShell ()
	{
		yield return new WaitForSeconds (m_LifeTime);

		float percent = 0;
		float fadeSpeed = 1f / m_FadeTime;

		Material mat = GetComponent<Renderer> ().material;
		Color initialColor = mat.color;

		while (percent <= 1f) 
		{
			percent += Time.deltaTime * fadeSpeed;
			mat.color = Color.Lerp (initialColor, Color.clear, percent);
			yield return null;
		}

		Destroy (gameObject);

	}
}
