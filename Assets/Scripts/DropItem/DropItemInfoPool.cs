using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DropItemInfoPool : MonoBehaviour
{
    public GameObject dropItemInfoPrefab; 
    public int poolSize = 50;

    private Queue<GameObject> dropItemInfoPool = new Queue<GameObject>();

    public static DropItemInfoPool instance;

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

    // Start is called before the first frame update
    void Start()
    {
        InitializePool();
    }

    private void InitializePool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            CreateNewDropItemInfoUI();
        }
    }

    private void CreateNewDropItemInfoUI()
    {
        GameObject dropItemInfoUIInstance = Instantiate(dropItemInfoPrefab, transform);
        dropItemInfoUIInstance.SetActive(false);
        dropItemInfoPool.Enqueue(dropItemInfoUIInstance);
    }

    public GameObject GetDropItemInfoUI()
    {
        if (dropItemInfoPool.Count > 0)
        {
            GameObject dropItemInfoUIInstance = dropItemInfoPool.Dequeue();
            dropItemInfoUIInstance.SetActive(true);
            return dropItemInfoUIInstance;
        }
        else
        {
            CreateNewDropItemInfoUI();
            return GetDropItemInfoUI();
        }
    }

    public void ReturnDropItemInfoUI(GameObject dropItemInfoUIInstance)
    {
        dropItemInfoUIInstance.SetActive(false);
        dropItemInfoPool.Enqueue(dropItemInfoUIInstance);
    }
}
