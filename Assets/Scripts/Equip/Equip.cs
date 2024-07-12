using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Equip : MonoBehaviour
{
    // 0 : Head
    // 1 : Chest
    // 2 : Shield
    // 3 : Weapon
    // 4 : Pants
    // 5 : Boots
    private CameraController cameraController;

    public GameObject equipPanel;
    public GameObject[] equipSlot; 
    public GameObject[] equipitem;

    public bool visibleEquip = false; // 장비창이 보여지고 있는가

    private void Start()
    {
        cameraController = GameObject.Find("Camera").GetComponent<CameraController>();
        equipPanel.SetActive(visibleEquip);
    }
}
