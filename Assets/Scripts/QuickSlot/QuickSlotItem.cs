using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class QuickSlotItem
{
    public int slotNum;
    public string iconPath;
    public int itemAmount;


    public QuickSlotItem(int slotnum, string iconPath, int itemAmount)
    {
        this.slotNum = slotnum;
        this.iconPath = iconPath;
        this.itemAmount = itemAmount;
    }
}