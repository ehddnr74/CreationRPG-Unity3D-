using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// Json�� �߰��Ѵ�� ��ų �������� �����ǵ��� ���� (slotAmount�� �ν����Ϳ��� ���� ����(���� ����))
/// </summary>
public class SkillManager : MonoBehaviour
{
    public static SkillManager instance;

    public int slotAmount;
    public GameObject skillSlot;
    public GameObject content;

    private string path;
    private string skillDataFilename = "Skill";  // ��ų ������ ���ϸ�
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
        //////////////////// Json ��ų �� ��ŭ slot�� ��ų �����ϴ� ���� 
        int index = 0;
        foreach (var skill in skillCollection.skills)
        {
            if (index >= slots.Count) break; // ���Ժ��� ��ų�� ���� ��� ����

            SkillDT skillData = slots[index].GetComponentInChildren<SkillDT>(); 
            if(skillData != null)
            {
                //////////////////// SkillData�� Icon,Name,Level ������Ʈ�� ���� ���� 
                Sprite Icon = Resources.Load<Sprite>("Items/" + skill.Value.iconPath);
                skillData.skillName = skill.Value.skillName;
                skillData.skillType = skill.Value.skillType;
                skillData.skillMinLevel = skill.Value.minLevel;
                skillData.skillMasterLevel = skill.Value.maxLevel;
                skillData.skillLevel = skill.Value.skillLevel;
                skillData.skillIcon = Icon;
                ////////////////////

                //////////////////// ��ų�� ��¥ �̹��� �����
                Image nonDragableSkillImg = skillData.nonDragableskillIcon.GetComponent<Image>();
                nonDragableSkillImg.sprite = Icon;

                Color tempColor = nonDragableSkillImg.color;
                tempColor.a = 1f;
                nonDragableSkillImg.color = tempColor;

                Image skillImg = skillData.gameObject.GetComponent<Image>();
                skillImg.sprite = Icon;
                ////////////////////
   
                ///////////////////// ��ų ��ҿ� Name,Lv.,Level,UpBtn ��Ƶα� 
                TextMeshProUGUI skillNameText = skillData.skillNameText.GetComponent<TextMeshProUGUI>();
                TextMeshProUGUI skillLvText = skillData.skillLvText.GetComponent<TextMeshProUGUI>();
                skillLvText.text = "Lv.";
                TextMeshProUGUI skillLevelText = skillData.skillLevelText.GetComponent<TextMeshProUGUI>();
                skillLevelText.text = skill.Value.skillLevel.ToString();
                Button skillUpBtn = skillData.skillUpBtn.GetComponent<Button>();
                /////////////////////
                
                ///////////////////// ��ų ��� �߰� �� ��ư ������ �߰�
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
        button.interactable = isVisible; // ��ư�� ���ͷ��� ���� ���ε� ����
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
            return new SkillCollection(); // �����Ͱ� ������ �� SkillCollection ��ȯ
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
