using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public GameObject slotPanel;
    public GameObject inventorySlot;
    public GameObject inventoryItem;

    public GameObject content;

    public int slotAmount;

    public List<GameObject> slots = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        slotPanel = GameObject.Find("Slot Panel");

        for (int i = 0; i < slotAmount; i++) 
        {
            slots.Add(Instantiate(inventorySlot));
            slots[i].GetComponent<Slot>().id = i;
            slots[i].transform.SetParent(content.transform, false);
            GameObject slotItem = Instantiate(inventoryItem);
            slotItem.transform.SetParent(slots[i].transform, false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
