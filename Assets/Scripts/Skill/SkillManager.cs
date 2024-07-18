using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// Json에 추가한대로 스킬 동적으로 생성되도록 구현 (slotAmount는 인스펙터에서 변경 가능(동적 생성))
/// </summary>
public class SkillManager : MonoBehaviour
{
    public static SkillManager instance;

    public int slotAmount;
    public GameObject skillSlot;
    public GameObject content;

    private string path;
    private string skillDataFilename = "Skill";  // 스킬 데이터 파일명
    public SkillCollection skillCollection;

    public TextMeshProUGUI skillPointText;

    public GameObject skillPanel;

    public bool visibleSkill = false;
    public bool itemChanged = false;

    private Dictionary<string, (TextMeshProUGUI NameText, TextMeshProUGUI levelText, SkillDT skillDT, Button skillButton)> skillUIElements = new Dictionary<string, (TextMeshProUGUI,TextMeshProUGUI, SkillDT, Button)>();
    public List<GameObject> slots = new List<GameObject>();

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
        path = Application.persistentDataPath + "/";

        CreateSlot(); 
        skillCollection = LoadSkillData();
        AddSkill();
        UpdateSkillUI();
        SaveSkillData(skillCollection);

        skillPanel.SetActive(visibleSkill);
    }

    // Update is called once per frame
    void Update()
    {
        if (itemChanged)
        {
            itemChanged = false;
            SaveSkillData(skillCollection);
            UpdateSkillUI();
        }
        if(Input.GetKeyDown(KeyCode.L))
        {
            skillCollection.skillPoint++;
            itemChanged = true;
        }
    }

    private void CreateSlot()
    {
        for(int i=0;i<slotAmount;i++)
        {
            slots.Add(Instantiate(skillSlot));
            slots[i].transform.SetParent(content.transform, false);
        }
    }
    private void AddSkill()
    {
        //////////////////// Json 스킬 수 만큼 slot에 스킬 생성하는 과정 
        int index = 0;
        foreach (var skill in skillCollection.skills)
        {
            if (index >= slots.Count) break; // 슬롯보다 스킬이 많은 경우 종료

            SkillDT skillData = slots[index].GetComponentInChildren<SkillDT>(); 
            if(skillData != null)
            {
                //////////////////// SkillData에 Icon,Name,Level 업데이트를 위해 저장 
                Sprite Icon = Resources.Load<Sprite>("Items/" + skill.Value.iconPath);
                skillData.skillName = skill.Value.skillName;
                skillData.skillType = skill.Value.skillType;
                skillData.skillDescription = skill.Value.skillDescription;
                skillData.skillMinLevel = skill.Value.minLevel;
                skillData.skillMasterLevel = skill.Value.maxLevel;
                skillData.skillLevel = skill.Value.skillLevel;

                // skillLevelDescription 리스트 크기 초기화
                skillData.skillLevelDescription = new List<string>(new string[skillData.skillMasterLevel]);

                // levelEffects의 키는 1부터 시작
                for (int i = 1; i <= skillData.skillMasterLevel; i++)
                {
                    if (skillCollection.skills[skill.Value.skillName].levelEffects.ContainsKey(i))
                    {
                        skillData.skillLevelDescription[i - 1] = skillCollection.skills[skill.Value.skillName].levelEffects[i].descrption;
                    }
                }
                skillData.skillIcon = Icon;
                ////////////////////

                //////////////////// 스킬의 진짜 이미지 씌우기
                Image nonDragableSkillImg = skillData.nonDragableskillIcon.GetComponent<Image>();
                nonDragableSkillImg.sprite = Icon;

                Color tempColor = nonDragableSkillImg.color;
                tempColor.a = 1f;
                nonDragableSkillImg.color = tempColor;

                Image skillImg = skillData.gameObject.GetComponent<Image>();
                skillImg.sprite = Icon;
                ////////////////////
   
                ///////////////////// 스킬 요소에 Name,Lv.,Level,UpBtn 담아두기 
                TextMeshProUGUI skillNameText = skillData.skillNameText.GetComponent<TextMeshProUGUI>();
                TextMeshProUGUI skillLvText = skillData.skillLvText.GetComponent<TextMeshProUGUI>();
                skillLvText.text = "Lv.";
                TextMeshProUGUI skillLevelText = skillData.skillLevelText.GetComponent<TextMeshProUGUI>();
                skillLevelText.text = skill.Value.skillLevel.ToString();
                Button skillUpBtn = skillData.skillUpBtn.GetComponent<Button>();
                /////////////////////
                
                ///////////////////// 스킬 요소 추가 후 버튼 리스너 추가
                skillUIElements.Add(skill.Value.skillName, (skillNameText,skillLevelText, skillData, skillUpBtn));
                skillUpBtn.onClick.AddListener(() => OnSkillButtonClick(skill.Key));
                /////////////////////
            }
            index++;
        }
    }

    private void OnSkillButtonClick(string skillName)
    {
        skillCollection.skillPoint--;
        skillCollection.skills[skillName].skillLevel++;
        itemChanged = true;
    }

    public void UpdateSkillUI()
    {
        UpdateSkillPointText();

        foreach (var skill in skillCollection.skills)
        {
            if (skillUIElements.TryGetValue(skill.Key, out var uiElements))
            {
                UpdateSkillUIElement(skill.Value, uiElements.NameText, uiElements.levelText, uiElements.skillDT);
            }
        }

        UpdateButtonVisibility();
    }
    private void UpdateSkillPointText()
    {
        if (skillPointText != null)
        {
            skillPointText.text = skillCollection.skillPoint.ToString();
        }
    }
    private void UpdateSkillUIElement(SkillData skill, TextMeshProUGUI skillNameText, TextMeshProUGUI skillLevelText, SkillDT skillDT)
    {
        if (skillNameText != null)
        {
            skillNameText.text = skill.skillName;
        }
        if (skillLevelText != null)
        {
            skillLevelText.text = skill.skillLevel.ToString();
        }
        if (skill.skillLevel > skill.minLevel && skillDT != null)
        {
            skillDT.skillLevel = skill.skillLevel;
        }
    }
    private void UpdateButtonVisibility()
    {
        if (skillCollection.skillPoint <= 0)
        {
            foreach (var uiElements in skillUIElements.Values)
            {
                SetButtonVisibility(uiElements.skillButton, false);
            }
        }
        else
        {
            foreach (var skill in skillCollection.skills)
            {
                if (skillUIElements.TryGetValue(skill.Key, out var uiElements))
                {
                    SetButtonVisibility(uiElements.skillButton, skill.Value.skillLevel < skill.Value.maxLevel);
                }
            }
        }
    }
    private void SetButtonVisibility(Button button, bool isVisible)
    {
        Image buttonImage = button.GetComponent<Image>();
        Color buttonColor = buttonImage.color;
        buttonColor.a = isVisible ? 1f : 0f;
        buttonImage.color = buttonColor;
        button.interactable = isVisible; // 버튼의 인터랙션 가능 여부도 설정
    }

    public void SaveSkillData(SkillCollection skillCollection)
    {
        string json = JsonConvert.SerializeObject(skillCollection, Formatting.Indented);
        File.WriteAllText(path + "Skill" + ".json", json);
    }

    public SkillCollection LoadSkillData()
    {
        string skillDataPath = path + skillDataFilename + ".json";

        if (File.Exists(skillDataPath))
        {
            string skillDatajson = File.ReadAllText(skillDataPath);

            SkillCollection loadedData = JsonConvert.DeserializeObject<SkillCollection>(skillDatajson);
            return loadedData;
        }
        else
        {
            return new SkillCollection(); // 데이터가 없으면 빈 SkillCollection 반환
        }
    }
    public SkillData GetSkillDataByName(string skillName)
    {
        if (skillCollection.skills.ContainsKey(skillName))
        {
            return skillCollection.skills[skillName];
        }
        return null;
    }

}
