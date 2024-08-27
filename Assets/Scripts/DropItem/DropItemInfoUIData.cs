using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;
using UnityEngine.UI;
using TMPro;

public class DropItemInfoUIData : MonoBehaviour
{
    private DropItemInfoPool dropItemInfoPool;

    public void Initialize(DropItemInfoPool pool, Item item)
    {
        dropItemInfoPool = pool;

        Image itemIcon = transform.GetChild(0).GetComponent<Image>();
        itemIcon.sprite = item.Icon;

        if(item.Name == "Gold")
        {
            TextMeshProUGUI itemText2 = transform.GetChild(1).GetComponent<TextMeshProUGUI>();
            itemText2.text = $"<color=yellow>'{item.gold}'</color>G";
        }
        else
        {
            TextMeshProUGUI itemText = transform.GetChild(1).GetComponent<TextMeshProUGUI>();
            itemText.text = item.Name;
        }

        gameObject.SetActive(true);

        StartCoroutine(AutoHide());
    }

    private IEnumerator AutoHide()
    {
        yield return new WaitForSeconds(1.5f);
        gameObject.SetActive(false);
        dropItemInfoPool.ReturnDropItemInfoUI(gameObject);
    }
}
