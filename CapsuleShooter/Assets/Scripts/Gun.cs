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
	public int m_BurstCount;

	public Transform m_Shell;
	public Transform m_ShellExtraction;

	private MuzzleFlash m_MuzzleFlash;

	private float m_NextShotTime;

	private int m_ShotsRemainingInBurst;

	private bool m_TriggerReleasedSinceLastShot;

	private void Start()
	{
		m_MuzzleFlash = GetComponent<MuzzleFlash> ();
		m_ShotsRemainingInBurst = m_BurstCount;
	}

	private void Shoot()
	{
		if (Time.time > m_NextShotTime) 
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
				// The time between shots is calculated in miliseconds, then the bullet is instantiated and it is given a set speed depeding on the gun.
				m_NextShotTime = Time.time + m_MSBetweenShots / 1000f;

				Projectile newProjectile = Instantiate (m_Projectile, m_Muzzle[i].position, m_Muzzle[i].rotation) as Projectile;
				newProjectile.SetSpeed (m_MuzzleVelocity);

			}

			Instantiate (m_Shell, m_ShellExtraction.position, m_ShellExtraction.rotation);
			m_MuzzleFlash.Activate ();
		}
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
