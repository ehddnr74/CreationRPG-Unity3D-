using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponMount : MonoBehaviour
{
    public GameObject weaponPrefab; // ������ ���� ������
    private GameObject currentWeapon; // ���� ������ ����

    public Transform weaponMountPoint; // ���⸦ ������ ��ġ (������ ��ġ)
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
            // ���� ���Ⱑ �ִٸ� ����
            if (currentWeapon != null)
            {
                Destroy(currentWeapon);
            }

            // ���ο� ���⸦ �����Ͽ� ���� ��ġ�� ���Դϴ�.
            currentWeapon = Instantiate(weaponPrefab, weaponMountPoint);
            currentWeapon.transform.localPosition = Vector3.zero;
            currentWeapon.transform.localRotation = Quaternion.Euler(weaponRotationOffset);
        }
    }
}
