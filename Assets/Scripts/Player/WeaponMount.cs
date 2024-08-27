using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponMount : MonoBehaviour
{
    public GameObject weaponPrefab; // 장착할 무기 프리팹
    private GameObject currentWeapon; // 현재 장착된 무기

    public Transform weaponMountPoint; // 무기를 장착할 위치 (오른손 위치)
    public Vector3 weaponRotationOffset;

    void Start()
    {
        EquipWeapon();
    }

    public void EquipWeapon()
    {
        if (weaponPrefab == null)
            Destroy(currentWeapon);
        
        if (weaponPrefab != null && weaponMountPoint != null)
        {
            // 기존 무기가 있다면 제거
            if (currentWeapon != null)
            {
                Destroy(currentWeapon);
            }

            // 새로운 무기를 생성하여 장착 위치에 붙입니다.
            currentWeapon = Instantiate(weaponPrefab, weaponMountPoint);
            currentWeapon.transform.localPosition = Vector3.zero;
            currentWeapon.transform.localRotation = Quaternion.Euler(weaponRotationOffset);
        }
    }
}
