using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
	public Transform m_Muzzle;
	public Projectile m_Projectile;
	public float m_MSBetweenShots = 100f;
	public float m_MuzzleVelocity = 35f;

	private float m_NextShotTime;

	public void Shoot()
	{
		if (Time.time > m_NextShotTime) 
		{
			// The time between shots is calculated in miliseconds, then the bullet is instantiated and it is given a set speed depeding on the gun.
			m_NextShotTime = Time.time + m_MSBetweenShots / 1000f;

			Projectile newProjectile = Instantiate (m_Projectile, m_Muzzle.position, m_Muzzle.rotation) as Projectile;
			newProjectile.SetSpeed (m_MuzzleVelocity);
		}
	}
}
