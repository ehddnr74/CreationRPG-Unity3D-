using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    private CameraController cameraController;

    [SerializeField] GameObject dialogueBar;
    [SerializeField] GameObject dialogueNameBar;
    [SerializeField] Button acceptBtn;
    [SerializeField] Button closeBtn;


    [SerializeField] TextMeshProUGUI dialogueText;
    [SerializeField] TextMeshProUGUI dialogueNameText;
    [SerializeField] TextMeshProUGUI acceptText;
    [SerializeField] TextMeshProUGUI closeText;


    Dialogue[] dialogues;

    bool activeUICount = false; // ī�޶��� Ȱ��ȭ�� UI üũ���� ��Ʈ�� 
    bool isDialogue = false;
    bool isNext = false; // Ư�� Ű �Է� ���.

    [Header("�ؽ�Ʈ ��� ������.")]
    [SerializeField] float textDelay;

    int lineCount = 0; // ��ȭ ī��Ʈ
    int contextCount = 0; // ��� ī��Ʈ

    bool questDialog; // ����Ʈ ��ȭ���� ���� 
    bool impossibleNext; // ��ȭ ���� �Ұ���

    public Quest quest;

    private void Start()
    {
        cameraController = GameObject.Find("Camera").GetComponent<CameraController>();
        acceptBtn.onClick.AddListener(Accept);
        closeBtn.onClick.AddListener(EndDialogue);
    }

    private void Accept()
    {
        QuestManager.instance.StartQuest(quest.id);
        EndDialogue();
    }

    private void Update()
    {
        if(isDialogue)
        {
            if(isNext)
            {
                if(Input.GetKeyDown(KeyCode.Space) && !impossibleNext)
                {
                    isNext = false;
                    dialogueText.text = "";
                    if (++contextCount < dialogues[lineCount].contexts.Length)
                    {
                        StartCoroutine(TypeWriter());
                    }
                    else
                    {
                        contextCount = 0; 
                        if (++lineCount < dialogues.Length)
                        {
                            cameraController.CameraTargetting(dialogues[lineCount].target);
                            StartCoroutine(TypeWriter());
                        }
                        else // ������ ��ȭ�� ���      
                        {
                            //CloseBtnAlphaSetting(1); // ��ȭ�� ������ �� ��ư ���̱�
                            //EndDialogue();
                        }
                    }
                }
            }
        }
    }

    public void ShowDialogue(Dialogue[] mdialogues, bool quest)
    {
        questDialog = quest;
        isDialogue = true;
        dialogueText.text = "";
        dialogueNameText.text = "";
        dialogues = mdialogues;
        SettingUI(true);
        cameraController.CameraOriginSetting();
        cameraController.CameraTargetting(dialogues[lineCount].target);
        StartCoroutine(TypeWriter());
    }

    void EndDialogue()
    {
        isDialogue = false;
        contextCount = 0;
        lineCount = 0;
        dialogues = null;
        isNext = false;

        cameraController.CameraReset();
        SettingUI(false);

        activeUICount = !activeUICount;
        cameraController.SetUIActiveCount(activeUICount);

        if (questDialog)
        {
            AcceptBtnAlphaSetting(0); // ���� ��ư �����
            CloseBtnAlphaSetting(0); // �ݱ� ��ư �����
        }
        else
        {
            CloseBtnAlphaSetting(0); // �ݱ� ��ư �����
        }
        
        impossibleNext = false;
    }

    IEnumerator TypeWriter()
    {
        SettingUI(true);

        string replaceText = dialogues[lineCount].contexts[contextCount];
        replaceText = replaceText.Replace("'", ",");

        string replaceNameText = dialogues[lineCount].name;

        if (replaceNameText == "��")
        {
            replaceNameText = replaceNameText.Replace("��", DataManager.instance.playerData.name);
            replaceNameText = $"<color=#FF0000>{replaceNameText}</color>";
            dialogueNameText.text = replaceNameText;
        }
        else
        {
            string nameText = dialogues[lineCount].name;
            nameText = $"<color=#00FFFF>{nameText}</color>";
            dialogueNameText.text = nameText;
        }

        for (int i=0;i< replaceText.Length;i++)
        {
            dialogueText.text += replaceText[i];

            yield return new WaitForSeconds(textDelay);
        }

        isNext = true;

        // Ÿ���� �ڷ�ƾ�� ������ �� �ݱ� ��ư ���̱�
        if (lineCount == dialogues.Length - 1 && contextCount == dialogues[lineCount].contexts.Length - 1)
        {
            impossibleNext = true;

            if (questDialog)
            {
                AcceptBtnAlphaSetting(1);
                CloseBtnAlphaSetting(1);
            }
            else
            {
                CloseBtnAlphaSetting(1);
            }
        }
    }

    void SettingUI(bool flag)
    {
        dialogueBar.SetActive(flag);
        dialogueNameBar.SetActive(flag);

        if (!activeUICount && flag)
        {
            activeUICount = !activeUICount;
            cameraController.SetUIActiveCount(flag);
        }
    }


    void AcceptBtnAlphaSetting(float alpha)
    {
        Image acceptBtnImg = acceptBtn.GetComponent<Image>();
        Color tempColor = acceptBtnImg.color;
        tempColor.a = alpha;
        acceptBtnImg.color = tempColor;

        if (alpha == 1)
        {
            acceptText.text = "����";
        }
        else
        {
            acceptText.text = "";
        }
    }

    void CloseBtnAlphaSetting(float alpha)
    {
        Image closeBtnImg = closeBtn.GetComponent<Image>();
        Color tempColor = closeBtnImg.color;
        tempColor.a = alpha;
        closeBtnImg.color = tempColor;

        if (!questDialog)
        {
            if (alpha == 1)
            {
                closeText.text = "�ݱ�";
            }
            else
            {
                closeText.text = "";
            }
        }
        else
            if (alpha == 1)
        {
            closeText.text = "����";
        }
        else
        {
            closeText.text = "";
        }
    }


}
