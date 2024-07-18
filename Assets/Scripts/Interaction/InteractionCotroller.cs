using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using static UnityEditor.Experimental.GraphView.GraphView;

public class InteractionCotroller : MonoBehaviour
{
    public string npcName;

    private Inventory inv;
    private Shop shop;
    private CameraController cameraController;
    public float interactionDistance = 3.0f;
    public KeyCode interactionKey = KeyCode.Mouse1;
    private Transform player;
    public int interactionLayer = 7; // ��ȣ�ۿ� �� ������ ���̾�
    public int defaultLayer = 0; // �⺻ ���̾�

    public GameObject mesh;

    public bool highlight = false;

    DialogueManager theDM;

    void Start()
    {
        cameraController = GameObject.Find("Camera").GetComponent<CameraController>();
        theDM = GameObject.Find("DialogueUI").GetComponent<DialogueManager>();
        player = GameObject.Find("Player").transform;
        inv = GameObject.Find("Inventory").GetComponent<Inventory>();
        shop = GameObject.Find("Shop").GetComponent<Shop>();
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

            if (angleToPlayer <= 20.0f && angleToNPC <= 20.0f) // ���θ� �ٶ󺸴� ���� üũ
            {
                mesh.layer = interactionLayer;

                if (Input.GetKeyDown(interactionKey))
                {
                    if (!cameraController.interaction)
                        Interact();
                }
            }
            else
            {
                mesh.layer = defaultLayer;
            }
        }
        else
        {
            mesh.layer = defaultLayer;
        }
    }

    void Interact()
    {
        if (npcName == "�Ƶ�")
        {
            Adel adle = GameObject.Find("Adel").GetComponent<Adel>();
            if (adle.CheckPossibleQuest(QuestManager.instance.questData.quests[1].id))
            {
                theDM.ShowDialogue(GetComponent<InteractionEvent>().GetDialogue(1, 2), true); // ����Ʈ ���� �� ���� ���
            }
            else if (QuestManager.instance.questData.quests[1].status == "������"
                 && CheckCollectionQuestItem(QuestManager.instance.questData.quests[1], 0))
            {
                theDM.ShowDialogue(GetComponent<InteractionEvent>().GetDialogue(3, 4), false); // ���� ����Ʈ �Ϸ� ������ ��� 
                QuestManager.instance.CompleteQuest(QuestManager.instance.questData.quests[1].id);
                inv.RemoveItem(0, 5, true);
            }
            else if (QuestManager.instance.questData.quests[1].status == "������"
                      && !CheckCollectionQuestItem(QuestManager.instance.questData.quests[1], 0))
            {
                theDM.ShowDialogue(GetComponent<InteractionEvent>().GetDialogue(5, 6), false); //�巡�� ��� ����Ʈ �Ϸ��� �� ���� ���
            }
            else
            {
                theDM.ShowDialogue(GetComponent<InteractionEvent>().GetDialogue(7, 8), false); // �ƹ��� ����� ���� ���
            }
        }

        if (npcName == "ī��")
        {
            Kain kain = GameObject.Find("Kain").GetComponent<Kain>();
            if (kain.CheckPossibleQuest(QuestManager.instance.questData.quests[0].id))
            {
                theDM.ShowDialogue(GetComponent<InteractionEvent>().GetDialogue(11, 13), true); // ����Ʈ ���� �� ���� ���
            }
            else if (QuestManager.instance.questData.quests[0].status == "������"
                 && QuestManager.instance.questData.quests[0].currentKillCount >= QuestManager.instance.questData.quests[0].targetKillCount)
            {
                theDM.ShowDialogue(GetComponent<InteractionEvent>().GetDialogue(16, 17), false); //�巡�� ��� ����Ʈ �Ϸ��� �� �ִ� ���
                QuestManager.instance.CompleteQuest(QuestManager.instance.questData.quests[0].id);
            }
            else if (QuestManager.instance.questData.quests[0].status == "������"
                 && QuestManager.instance.questData.quests[0].currentKillCount < QuestManager.instance.questData.quests[0].targetKillCount)
            {
                theDM.ShowDialogue(GetComponent<InteractionEvent>().GetDialogue(14, 15), false); //�巡�� ��� ����Ʈ �Ϸ��� �� ���� ���
            }
            else
            {
                theDM.ShowDialogue(GetComponent<InteractionEvent>().GetDialogue(9, 10), false); // �ƹ��� ����� ���� ���
            }
        }

        if(npcName == "����")
        {
            if (!shop.visibleShop)
            {
                shop.visibleShop = !shop.visibleShop;
                shop.shopPanel.SetActive(shop.visibleShop);

                cameraController.SetUIActiveCount(shop.visibleShop);
            }
        }
    }

    private bool CheckCollectionQuestItem(Quest quest, int ID)
    {
        for (int i = 0; i < inv.items.Count; i++)
        {
            if (inv.items[i].ID == ID)
            {
                ItemDT data = inv.slots[i].transform.GetChild(0).GetComponent<ItemDT>();

                if (data.amount >= quest.maxCollectionCount)
                {
                    return true;
                }
            }
        }
        return false;
    }
}

