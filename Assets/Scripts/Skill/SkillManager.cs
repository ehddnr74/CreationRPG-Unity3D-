using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillManager : MonoBehaviour
{
    public static SkillManager instance;

    private string path;
    private string skillDataFilename = "Skill";  // ��ų ������ ���ϸ�
    public SkillCollection skillCollection;

    public TextMeshProUGUI skillPointText;
    public TextMeshProUGUI lightningStrikeText;
    public TextMeshProUGUI lightningStrikeLevel;
    public TextMeshProUGUI atrocitiesText;
    public TextMeshProUGUI atrocitiesLevel;
    public TextMeshProUGUI naturalDisasterText;
    public TextMeshProUGUI naturalDisasterLevel;

    public Button lightningStrikeBtn;
    public Button atrocitiesBtn;
    public Button naturalDisasterBtn;

    public GameObject lightningStrikeIcon;
    public GameObject atrocitiesIcon;
    public GameObject naturalDisasterIcon;

    SkillDT lightningStrikeSkillDT;
    SkillDT atrocitiesSkillDT;
    SkillDT naturalDisasterSkillDT;

    public GameObject skillPanel;

    public bool visibleSkill = false;
    public bool itemChanged = false;

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

        skillCollection = LoadSkillData();
        AddListener(); // ��ų ���� �� ��ư ������ �߰� 
        LoadSkillDT();
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

    private void AddListener()
    {
        lightningStrikeBtn.onClick.AddListener(OnLightningStrikeButtonClick);
        atrocitiesBtn.onClick.AddListener(OnAtrocitiesButtonClick);
        naturalDisasterBtn.onClick.AddListener(onNaturalDisasterButtonClick);
    }
    private void LoadSkillDT()
    {
        lightningStrikeSkillDT = lightningStrikeIcon.GetComponent<SkillDT>();
        atrocitiesSkillDT = atrocitiesIcon.GetComponent<SkillDT>();
        naturalDisasterSkillDT = naturalDisasterIcon.GetComponent<SkillDT>();
    }
    private void OnLightningStrikeButtonClick()
    {
        skillCollection.skillPoint--;
        skillCollection.skills[0].skillLevel++;
        itemChanged = true;
    }
    private void OnAtrocitiesButtonClick()
    {
        skillCollection.skillPoint--;
        skillCollection.skills[1].skillLevel++;
        itemChanged = true;
    }
    private void onNaturalDisasterButtonClick()
    {
        skillCollection.skillPoint--;
        skillCollection.skills[2].skillLevel++;
        itemChanged = true;
    }

    public void UpdateSkillUI()
    {
        UpdateSkillPointText();

        if (skillCollection.skills.Count > 0)
        {
            UpdateSkillUIElement(skillCollection.skills[0], lightningStrikeLevel, lightningStrikeSkillDT);
            UpdateSkillUIElement(skillCollection.skills[1], atrocitiesLevel, atrocitiesSkillDT);
            UpdateSkillUIElement(skillCollection.skills[2], naturalDisasterLevel, naturalDisasterSkillDT);
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
    private void UpdateSkillUIElement(SkillData skill, TextMeshProUGUI skillLevelText, SkillDT skillDT)
    {
        if (skillLevelText != null)
        {
            skillLevelText.text = skill.skillLevel.ToString();
        }
        if (skill.skillLevel > 0 && skillDT != null)
        {
            skillDT.skillLevel = skill.skillLevel;
        }
    }
    private void UpdateButtonVisibility()
    {
        if (skillCollection.skillPoint <= 0)
        {
            // ��ų ����Ʈ�� 0�̸� ��ư ��Ȱ��ȭ
            SetButtonVisibility(lightningStrikeBtn, false);
            SetButtonVisibility(atrocitiesBtn, false);
            SetButtonVisibility(naturalDisasterBtn, false);
        }
        else
        {
            // ��ų ����Ʈ�� ������ �� ��ų ������ ���� ��ư ���ü� ����
            SetButtonVisibility(lightningStrikeBtn, skillCollection.skills[0].skillLevel < 5);
            SetButtonVisibility(atrocitiesBtn, skillCollection.skills[1].skillLevel < 5);
            SetButtonVisibility(naturalDisasterBtn, skillCollection.skills[2].skillLevel < 5);
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

}
