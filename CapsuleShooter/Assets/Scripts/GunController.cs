using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour 
{
	public Transform m_WeaponHolder;
	public Gun[] m_AllGuns;

	private Gun m_EquippedGun;

	private void Start()
	{
		
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

	public void EquipGun(int weaponIndex)
	{
		EquipGun (m_AllGuns [weaponIndex]);
	}

	public void OnTriggerHold()
	{
		if (m_EquippedGun != null)
		{
			// If there's a weapon equipped we are able to shoot.
			m_EquippedGun.OnTriggerHold ();
		}
	}

	public void OnTriggerReleased()
	{
		if (m_EquippedGun != null)
		{
			m_EquippedGun.OnTriggerReleased ();
		}
	}

	public float GetHeight
	{
		get
		{
			return m_WeaponHolder.position.y;
		}
	}

	public void Aim()
	{
		if (m_EquippedGun != null)
		{
			m_EquippedGun.Aim ();
		}
	}

	public void Reload()
	{
		if (m_EquippedGun != null)
		{
			m_EquippedGun.Reloading ();
		}
	}
}
