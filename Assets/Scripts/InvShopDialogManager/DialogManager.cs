using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogManager : MonoBehaviour
{
    public static DialogManager instance;

    public GameObject[] dialogs;

    public bool visibleShopDialogs;

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

    public void ShowDialog(GameObject dialogToShow)
    {
        foreach (var dialog in dialogs)
        {
            dialog.SetActive(false);
        }
        dialogToShow.SetActive(true);
        visibleShopDialogs = true;
        Canvas canvas = dialogToShow.transform.parent.GetComponent<Canvas>();

        // _globalSortingOrderCounter를 static으로 접근
        MovableUI._globalSortingOrderCounter++;
        canvas.sortingOrder = MovableUI._globalSortingOrderCounter;
    }

    public void HideAllDialogs()
    {
        foreach (var dialog in dialogs)
        {
            dialog.SetActive(false);
        }
        visibleShopDialogs = false;
    }
}
