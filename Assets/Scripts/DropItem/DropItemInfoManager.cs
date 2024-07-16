using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropItemInfoManager : MonoBehaviour
{
    private Queue<Item> droppedItemsQueue = new Queue<Item>();
    private bool isShowing = false;

    public static DropItemInfoManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddDroppedItem(Item newItem)
    {
        droppedItemsQueue.Enqueue(newItem);

        if (!isShowing)
        {
            StartCoroutine(ShowDroppedItems());
        }
    }

    private IEnumerator ShowDroppedItems()
    {
        isShowing = true;

        while (droppedItemsQueue.Count > 0)
        {
            Item item = droppedItemsQueue.Dequeue();

            GameObject dropItemInfoUIInstance = DropItemInfoPool.instance.GetDropItemInfoUI();
            DropItemInfoUIData dropItemInfoData = dropItemInfoUIInstance.GetComponent<DropItemInfoUIData>();
            dropItemInfoData.Initialize(DropItemInfoPool.instance, item);

            yield return new WaitForSeconds(1.5f);
        }

        isShowing = false;
    }
}
