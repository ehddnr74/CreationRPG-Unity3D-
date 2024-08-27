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

    bool activeUICount = false; // 카메라의 활성화된 UI 체크변수 컨트롤 
    bool isDialogue = false;
    bool isNext = false; // 특정 키 입력 대기.

    [Header("텍스트 출력 딜레이.")]
    [SerializeField] float textDelay;

    int lineCount = 0; // 대화 카운트
    int contextCount = 0; // 대사 카운트

    bool questDialog; // 퀘스트 대화인지 여부 
    bool impossibleNext; // 대화 진행 불가능

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
                        else // 마지막 대화인 경우      
                        {
                            //CloseBtnAlphaSetting(1); // 대화가 끝났을 때 버튼 보이기
                            //EndDialogue();
                        }
                    }
                }
            }
        }
    }

    public void ShowDialogue(Dialogue[] mdialogues, bool quest)
    {
        if (cameraController.isUIActiveCount <= 0)
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
            AcceptBtnAlphaSetting(0); // 수락 버튼 숨기기
            CloseBtnAlphaSetting(0); // 닫기 버튼 숨기기
        }
        else
        {
            CloseBtnAlphaSetting(0); // 닫기 버튼 숨기기
        }
        
        impossibleNext = false;
    }

    IEnumerator TypeWriter()
    {
        SettingUI(true);

        string replaceText = dialogues[lineCount].contexts[contextCount];
        replaceText = replaceText.Replace("'", ",");
        replaceText = replaceText.Replace("+", DataManager.instance.playerData.name);

        string replaceNameText = dialogues[lineCount].name;

        if (replaceNameText == "나")
        {
            replaceNameText = replaceNameText.Replace("나", DataManager.instance.playerData.name);
            replaceNameText = $"<color=#FF0000>{replaceNameText}</color>";
            dialogueNameText.text = replaceNameText;
        }
        else
        {
            string nameText = dialogues[lineCount].name;
            nameText = $"<color=#00FFFF>{nameText}</color>";
            dialogueNameText.text = nameText;
        }

        bool textWhite = false, textRed = false;
        bool textIgnore = false;

        for (int i=0;i< replaceText.Length;i++)
        {
            switch(replaceText[i])
            {
                case 'ⓦ':
                    textWhite = true;
                    textRed = false;
                    textIgnore = true;
                    break;
                case 'ⓡ':
                    textWhite = false;
                    textRed = true;
                    textIgnore = true;
                    break;
            }

            string textLetter = replaceText[i].ToString();

            if(!textIgnore)
            {
                if(textWhite)
                {
                    textLetter = "<color=#ffffff>" + textLetter + "</color>";
                }
                else if(textRed)
                {
                    textLetter = "<color=#FF0000>" + textLetter + "</color>";
                }
                dialogueText.text += textLetter;
            }
            textIgnore = false;

            yield return new WaitForSeconds(textDelay);
        }

        isNext = true;

        // 타이핑 코루틴이 끝났을 때 닫기 버튼 보이기
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
            acceptText.text = "수락";
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
                closeText.text = "닫기";
            }
            else
            {
                closeText.text = "";
            }
        }
        else
            if (alpha == 1)
        {
            closeText.text = "거절";
        }
        else
        {
            closeText.text = "";
        }
    }


}
