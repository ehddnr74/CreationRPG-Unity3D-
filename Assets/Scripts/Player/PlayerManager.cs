using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public PlayerData playerData;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI InventorygGoldText;
    public TextMeshProUGUI shopGoldText;

    private void Start()
    {
        // ������ �Ŵ����� ���� �÷��̾� ������ �ε�
        if (DataManager.instance != null)
        {
            DataManager.instance.LoadPlayerData();
            playerData = DataManager.instance.playerData;
            UpdateUI();
        }
    }

    private void UpdateUI() // StatusBar ������Ʈ
    {
        if (nameText != null && playerData != null)
            nameText.text = playerData.name;

        if (levelText != null && playerData != null)
            levelText.text = playerData.level.ToString();

        if (InventorygGoldText != null && playerData != null)
            InventorygGoldText.text = playerData.gold.ToString();

        if (shopGoldText != null && playerData != null)
            shopGoldText.text = playerData.gold.ToString();
    }

    public void UpdateInventoryGoldText(PlayerData pd)// �÷��̾� ��忡 ���� Inventory GoldTextUI ���� 
    {
        playerData = pd;

        if (InventorygGoldText != null)
            InventorygGoldText.text = playerData.gold.ToString();
    }

    public void UpdateShopGoldText(PlayerData pd) // �÷��̾� ��忡 ���� Shop GoldTextUI ���� 
    {
        playerData = pd;

        if (shopGoldText != null)
            shopGoldText.text = pd.gold.ToString();
    }
}