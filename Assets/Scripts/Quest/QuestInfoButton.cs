using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestInfoButton : MonoBehaviour
{
    public Quest quest;

    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        QuestManager.instance.ShowQuestInfo(quest);
    }
}
