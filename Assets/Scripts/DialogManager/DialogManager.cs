using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogManager : MonoBehaviour
{
    public static DialogManager instance;

    public GameObject[] dialogs;

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
    }

    public void HideAllDialogs()
    {
        foreach (var dialog in dialogs)
        {
            dialog.SetActive(false);
        }
    }
}
