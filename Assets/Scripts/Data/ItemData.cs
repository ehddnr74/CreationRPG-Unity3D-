using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemData
{
    public int id;
    public string name;
    public string type;
    public string description;
    public int price;
    public string iconpath;
    public Sprite icon;
    public bool stackable;

    public List<Item> items;
}
