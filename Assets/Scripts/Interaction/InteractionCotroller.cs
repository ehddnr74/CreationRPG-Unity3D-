using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class InteractionCotroller : MonoBehaviour
{
    public string npcName;

    private CameraController cameraController;
    public float interactionDistance = 3.0f;
    public KeyCode interactionKey = KeyCode.F;
    private Transform player;

    DialogueManager theDM;

    void Start()
    {
        cameraController = GameObject.Find("Camera").GetComponent<CameraController>();
        theDM = GameObject.Find("DialogueUI").GetComponent<DialogueManager>();
        player = GameObject.Find("Player").transform;
    }

    void Update()
    {
        CheckInteraction();
    }

    void CheckInteraction()
    {
        float distance = Vector3.Distance(player.position, transform.position);

        if (distance <= interactionDistance)
        {
            Vector3 directionToPlayer = player.position - transform.position;
            float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);

            Vector3 directionToNPC = transform.position - player.position;
            float angleToNPC = Vector3.Angle(player.forward, directionToNPC);

            if (angleToPlayer <= 10.0f && angleToNPC <= 10.0f) // ���θ� �ٶ󺸴� ���� üũ
            {
                if (Input.GetKeyDown(interactionKey))
                {
                    if(!cameraController.interaction)
                    Interact();
                }
            }
        }
    } 

    void Interact()
    {
        if(npcName == "�Ƶ�")
        {
            Adel adle = GameObject.Find("Adel").GetComponent<Adel>();
            if (adle.CheckPossibleQuest())
            {
                theDM.ShowDialogue(GetComponent<InteractionEvent>().GetDialogue(1, 3), true); // ����Ʈ ���� �� ���� ���
            }
            else
                theDM.ShowDialogue(GetComponent<InteractionEvent>().GetDialogue(4, 6), false); // �ƹ��� ����� ���� ���
        }

        if (npcName == "ī��")
        {
            Kain kain = GameObject.Find("Kain").GetComponent<Kain>();
            if (kain.CheckPossibleQuest())
                theDM.ShowDialogue(GetComponent<InteractionEvent>().GetDialogue(9, 11), true); // ����Ʈ ���� �� ���� ���
            else
                theDM.ShowDialogue(GetComponent<InteractionEvent>().GetDialogue(7, 8), false); // �ƹ��� ����� ���� ���
        }

        //theDM.ShowDialogue(GetComponent<InteractionEvent>().GetDialogue());
        // ���⿡�� ��ȣ�ۿ� ���� ������ �߰��մϴ�. ��: ��ȭ UI ǥ��
    }

    //IEnumerator WaitCollision()
    //{
    //    yield return null;

    //}
}
