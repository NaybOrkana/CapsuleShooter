using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
	public enum FireMode
	{
		Auto, Burst, Single
	};

	public FireMode m_FireMode;

	public Transform[] m_Muzzle;
	public Projectile m_Projectile;
	public float m_MSBetweenShots = 100f;
	public float m_MuzzleVelocity = 35f;
	public float m_MaxRecoilForce = 0.2f;
	public int m_BurstCount;
	public Vector2 m_KickMinMax = new Vector2 (0.05f, .2f);
	public Vector2 m_RecoilAngleMinMax = new Vector2 (5f, 10f);
	public float m_RecoilAnimationTime = 0.1f;
	public float m_RotationRecoilAnimationTime = 0.1f;
	public float m_ReloadTime = 0.3f;
	public int m_AmmunitionsPerMagazine = 15;
	public AudioClip m_ShootAudio;
	public AudioClip m_ReloadAudio;

	public Transform m_Shell;
	public Transform m_ShellExtraction;

	private MuzzleFlash m_MuzzleFlash;

	private float m_NextShotTime;

	private Camera m_ViewCamera;

	private int m_ShotsRemainingInBurst;
	private int m_AmmunitionsRemainingInMagazine;
	private Vector3 m_RecoilSmoothDampVelocity;
	private float m_RecoilRotSmoothDampVelocity;
	private bool m_TriggerReleasedSinceLastShot;
	private float m_RecoilAngle;
	private bool m_IsReloading;


	private void Start()
	{
		m_MuzzleFlash = GetComponent<MuzzleFlash> ();
		m_ShotsRemainingInBurst = m_BurstCount;
		m_AmmunitionsRemainingInMagazine = m_AmmunitionsPerMagazine;

		m_ViewCamera = Camera.main;
	}

	private void LateUpdate()
	{
		//Animating the recoil
		transform.localPosition = Vector3.SmoothDamp(transform.localPosition, Vector3.zero, ref m_RecoilSmoothDampVelocity, m_RecoilAnimationTime);
		m_RecoilAngle = Mathf.SmoothDamp(m_RecoilAngle, 0f, ref m_RecoilRotSmoothDampVelocity, m_RotationRecoilAnimationTime);
		transform.localEulerAngles = Vector3.left * m_RecoilAngle;

		if (!m_IsReloading && m_AmmunitionsRemainingInMagazine == 0) 
		{
			Reloading ();
		}

		//This is the same as in the player, it rotates the gun following the crosshair. 
		//This is here to circumvent a bug that makes the player rotate even if constrainted and ignores the rotation of the gun otherwise.
		//It sends an error while looping through maps and guns, it can be ignored.
		if (FindObjectOfType<Player>() != null) 
		{
			Aim();
		}

	}

	private void Shoot()
	{
		if (!m_IsReloading && Time.time > m_NextShotTime && m_AmmunitionsRemainingInMagazine > 0) 
		{
			if (m_FireMode == FireMode.Burst)
			{
				if (m_ShotsRemainingInBurst == 0) 
				{
					return;
				}

				m_ShotsRemainingInBurst--;
			} 
			else if (m_FireMode == FireMode.Single) 
			{
				if (!m_TriggerReleasedSinceLastShot) 
				{
					return;
				}
			}

			for (int i = 0; i < m_Muzzle.Length; i++)
			{
				if (m_AmmunitionsRemainingInMagazine == 0)
				{
					break;
				}

				m_AmmunitionsRemainingInMagazine--;

				// The time between shots is calculated in miliseconds, then the bullet is instantiated and it is given a set speed depeding on the gun.
				m_NextShotTime = Time.time + m_MSBetweenShots / 1000f;

				Projectile newProjectile = Instantiate (m_Projectile, m_Muzzle[i].position, m_Muzzle[i].rotation) as Projectile;
				newProjectile.SetSpeed (m_MuzzleVelocity);

			}

			Instantiate (m_Shell, m_ShellExtraction.position, m_ShellExtraction.rotation);
			m_MuzzleFlash.Activate ();
			transform.localPosition -= Vector3.forward * Random.Range(m_KickMinMax.x, m_KickMinMax.y);
			m_RecoilAngle += Random.Range(m_RecoilAngleMinMax.x, m_RecoilAngleMinMax.y);
			m_RecoilAngle = Mathf.Clamp (m_RecoilAngle, 0f, 30f);

			AudioManager.m_Instance.PlaySound (m_ShootAudio, transform.position);
		}
	}

	public void Aim ()
	{
		Ray cursorRay = m_ViewCamera.ScreenPointToRay (Input.mousePosition);
		Plane groundPlane = new Plane (Vector3.up, Vector3.up * transform.position.y);
		float rayDistance;

		if (groundPlane.Raycast (cursorRay, out rayDistance)) 
		{
			Vector3 point = cursorRay.GetPoint (rayDistance);
			//Debug.DrawLine (cursorRay.origin, point, Color.red);
			transform.LookAt (point);
		}
	}

	public void Reloading()
	{
		if (!m_IsReloading && m_AmmunitionsRemainingInMagazine != m_AmmunitionsPerMagazine) 
		{
			StartCoroutine (AnimateReload ());
			AudioManager.m_Instance.PlaySound (m_ReloadAudio, transform.position);
		}
	}

	private IEnumerator AnimateReload()
	{
		m_IsReloading = true;
		yield return new WaitForSeconds (0.2f);

		float reloadSpeed = 1f / m_ReloadTime;
		float percent = 0;
		Vector3 initialRot = transform.localEulerAngles;
		float maxReloadAngle = 40f;

		while (percent < 1) 
		{
			percent += Time.deltaTime * reloadSpeed;
			float interpolation = (-Mathf.Pow (percent, 2f) + percent) * 4f;
			float reloadAngle = Mathf.Lerp (0, maxReloadAngle, interpolation);
			transform.Rotate(initialRot + Vector3.left * reloadAngle);

			yield return null;
		}

		m_IsReloading = false;

		m_AmmunitionsRemainingInMagazine = m_AmmunitionsPerMagazine;
	}

	public void OnTriggerHold()
	{
		Shoot ();
		m_TriggerReleasedSinceLastShot = false;
	}

	public void OnTriggerReleased()
	{
		m_TriggerReleasedSinceLastShot = true;
		m_ShotsRemainingInBurst = m_BurstCount;

	}
}
