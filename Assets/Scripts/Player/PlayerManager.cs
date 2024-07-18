using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI InventorygGoldText;
    public TextMeshProUGUI shopGoldText;

    private Shop shop;

    private void Start()
    {
        shop = GameObject.Find("Shop").GetComponent<Shop>();
        UpdateUI();
    }

    private void UpdateUI() // StatusBar 업데이트
    {
        if (nameText != null && DataManager.instance.playerData != null)
            nameText.text = DataManager.instance.playerData.name;

        if (levelText != null && DataManager.instance.playerData != null)
            levelText.text = DataManager.instance.playerData.level.ToString();

        if (InventorygGoldText != null && DataManager.instance.playerData != null)
            InventorygGoldText.text = DataManager.instance.playerData.gold.ToString();

        if (shopGoldText != null && DataManager.instance.playerData != null)
            shopGoldText.text = DataManager.instance.playerData.gold.ToString();
    }

    public void UpdateGoldText()// 플레이어 골드에 따른 Inventory / Shop GoldTextUI 갱신 
    {
        if (InventorygGoldText != null)
            InventorygGoldText.text = DataManager.instance.playerData.gold.ToString();

        if (shopGoldText != null && DataManager.instance.playerData != null)
            shopGoldText.text = DataManager.instance.playerData.gold.ToString();
    }
}