using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour 
{
	public Transform m_WeaponHolder;
	public Gun m_StartingGun;

	private Gun m_EquippedGun;

	private void Start()
	{
		if (m_StartingGun != null) 
		{
			// If there is no gun equipped the starting gun we'll be equipped.
			EquipGun (m_StartingGun);
		}
	}

	public void EquipGun(Gun gunToEquip)
	{
		if (m_EquippedGun != null) 
		{
			// Otherwise it'll be replaced by the new pickep up weapon.
			Destroy (m_EquippedGun.gameObject);
		}

		// The gun is instantiated in the scened and parented to the player's "hand".
		m_EquippedGun = Instantiate (gunToEquip, m_WeaponHolder.position, m_WeaponHolder.rotation) as Gun;
		m_EquippedGun.transform.parent = m_WeaponHolder;
	}

	public void TriggerShoot()
	{
		if (m_EquippedGun != null)
		{
			// If there's a weapon equipped we are able to shoot.
			m_EquippedGun.Shoot ();
		}
	}

}
