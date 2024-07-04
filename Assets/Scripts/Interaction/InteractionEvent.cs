using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionEvent : MonoBehaviour
{
    [SerializeField] DialogueEvent dialogueEvent;

    public Dialogue[] GetDialogue(int startLine, int endLine) // 대화의 몇번째 줄 부터 몇번째 줄까지를 받아올건지 결정 
    {
        DialogueEvent t_dialogueEvent = new DialogueEvent();
        t_dialogueEvent.dialogues = DialogueDataManager.instance.GetDialogue(startLine, endLine);
        for (int i = 0; i <= endLine - startLine; i++) 
        {
            t_dialogueEvent.dialogues[i].target = dialogueEvent.dialogues[i].target;
        }
        dialogueEvent.dialogues = t_dialogueEvent.dialogues;
        return dialogueEvent.dialogues;
    }
}
