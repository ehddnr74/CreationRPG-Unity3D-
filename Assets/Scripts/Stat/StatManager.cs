using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.UI;
using UnityEngine.UI;

public class StatManager : MonoBehaviour
{
    public static StatManager instance;
    public StatData statData;

    private QuickSlot quickSlot;

    public Slider hpBar;
    public Slider mpBar;

    public TextMeshProUGUI nickNameText;
    public TextMeshProUGUI StatusBarLevelText;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI hpText;
    public TextMeshProUGUI mpText;
    public TextMeshProUGUI experienceText;
    public TextMeshProUGUI attackpowerText;
    public TextMeshProUGUI defenseText;
    public TextMeshProUGUI criticalProbabilityText;


    public GameObject statPanel;

    public bool visibleStat = false;
    public bool hyperBody = false;

    string path;
    string StatDataFilename = "Stat"; // ���� ������ ���ϸ�


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

        path = Application.persistentDataPath + "/";
    }
    // Start is called before the first frame update
    void Start()
    {
        quickSlot = GameObject.Find("QuickSlot").GetComponent<QuickSlot>();

        LoadStatData();
        InitializeStat();
        statPanel.SetActive(false);
    }

    public void InitializeStat()
    {
        // UI ���� 
        nickNameText.text = DataManager.instance.playerData.name; // �г��� 
        levelText.text = DataManager.instance.playerData.level.ToString(); // ����

        statData.hp = statData.maxHp;
        statData.mp = statData.maxMp;

        hpText.text = $"{statData.hp} / {statData.maxHp}"; // HP
        mpText.text = $"{statData.mp} / {statData.maxMp}"; // MP

        if (hpBar != null)
            hpBar.value = (float)statData.hp / statData.maxHp;

        if (mpBar != null)
            mpBar.value = (float)statData.mp / statData.maxMp;

        // ����ġ
        experienceText.text = $"{DataManager.instance.playerData.experience} / {DataManager.instance.playerData.experienceTable[DataManager.instance.playerData.level]}";
        // ���ݷ�
        statData.extraAttackPower = 0;
        UpdateStatAttackPower();
        // ����
        statData.extraDefense = 0;
        UpdateStatDefense();
        // ũ��Ƽ�� Ȯ��
        criticalProbabilityText.text =  $"{statData.originCriticalProbability}%";
    }

    public void UpdateStatLevel()
    {
        if(StatusBarLevelText != null)
        {
            StatusBarLevelText.text = DataManager.instance.playerData.level.ToString();
        }
        levelText.text = DataManager.instance.playerData.level.ToString(); // ����
    }

    public void UpdateStatStatus()
    {
        hpText.text = $"{statData.hp} / {statData.maxHp}"; // HP
        mpText.text = $"{statData.mp} / {statData.maxMp}"; // MP

        if (hpBar != null)
            hpBar.value = (float)statData.hp / statData.maxHp;

        if (mpBar != null)
            mpBar.value = (float)statData.mp / statData.maxMp;

        SaveStat();
    }
    public void UpdateStatStatusHyperBody()
    {
        hpText.text = $"{statData.hp} / {statData.buffMaxHp}"; // HP
        mpText.text = $"{statData.mp} / {statData.buffMaxMp}"; // MP

        if (hpBar != null)
            hpBar.value = (float)statData.hp / statData.maxHp;

        if (mpBar != null)
            mpBar.value = (float)statData.mp / statData.maxMp;

        SaveStat();
    }

    public void UpdateStatActiveHyperBody()
    {
        hyperBody = true;

        int level = SkillManager.instance.skillCollection.skills["�ݰ��ұ�"].skillLevel;

        statData.buffMaxHp = Mathf.CeilToInt(statData.maxHp * SkillManager.instance.skillCollection.skills["�ݰ��ұ�"].levelEffects[level].hpIncrease);
        statData.buffMaxMp = Mathf.CeilToInt(statData.maxMp * SkillManager.instance.skillCollection.skills["�ݰ��ұ�"].levelEffects[level].mpIncrease);

        hpText.text = $"{statData.hp} / {statData.buffMaxHp}"; // HP
        mpText.text = $"{statData.mp} / {statData.buffMaxMp}"; // MP

        if (hpBar != null)
            hpBar.value = (float)statData.hp / statData.maxHp;

        if (mpBar != null)
            mpBar.value = (float)statData.mp / statData.maxMp;

        SaveStat();
    }

    public void UpdateStatDeactiveHyperBody()
    {
        hyperBody = false;

        statData.hp = statData.maxHp;
        statData.mp = statData.maxMp;

        hpText.text = $"{statData.hp} / {statData.maxHp}"; // HP
        mpText.text = $"{statData.mp} / {statData.maxMp}"; // MP

        if (hpBar != null)
            hpBar.value = (float)statData.hp / statData.maxHp;

        if (mpBar != null)
            mpBar.value = (float)statData.mp / statData.maxMp;

        SaveStat();
    }

    public void UpdateStatExperience()
    {
        experienceText.text = $"{DataManager.instance.playerData.experience} / {DataManager.instance.playerData.experienceTable[DataManager.instance.playerData.level]}";
        SaveStat();
    }
    public void UpdateStatAttackPower()
    {
        attackpowerText.text = $"���ݷ� : {statData.attackPower} + (<color=red>{statData.extraAttackPower}</color>)";
        SaveStat();
    }
    public void UpdateStatDefense()
    {
        defenseText.text = $"���� : {statData.defense} + (<color=red>{statData.extraDefense}</color>)";
        SaveStat();
    }
    public void UpdateStatActiveCriticalSkill()
    {
        criticalProbabilityText.text = $"{statData.criticalProbability}%";
        SaveStat();
    }
    public void UpdateStatDeactiveCriticalSkill()
    {
        statData.criticalProbability = statData.originCriticalProbability;
        criticalProbabilityText.text = $"{statData.criticalProbability}%";
        SaveStat();
    }



    public void AddHP(int hpamount)
    {
        statData.hp += hpamount;
        if (!hyperBody)
        {
            CheckStatus();
            UpdateStatStatus();
        }
        else
        {
            CheckHyperBodyStatus();
            UpdateStatStatusHyperBody();
        }
        SaveStat();
    }
    public void RemoveHP(int hpamount)
    {
        statData.hp -= hpamount;
        if (!hyperBody)
        {
            CheckStatus();
            UpdateStatStatus();
        }
        else
        {
            CheckHyperBodyStatus();
            UpdateStatStatusHyperBody();
        }
        SaveStat();
    }

    public void AddMP(int mpamount)
    {
        statData.mp += mpamount;
        if (!hyperBody)
        {
            CheckStatus();
            UpdateStatStatus();
        }
        else
        {
            CheckHyperBodyStatus();
            UpdateStatStatusHyperBody();
        }
        SaveStat();
    }
    public void RemoveMP(int mpamount)
    {
        statData.mp -= mpamount;
        if (!hyperBody)
        {
            CheckStatus();
            UpdateStatStatus();
        }
        else
        {
            CheckHyperBodyStatus();
            UpdateStatStatusHyperBody();
        }
        SaveStat();
    }

    private void CheckStatus() // HP , MP �ʰ� or �̸� üũ 
    {
        if (statData.hp >= statData.maxHp)
            statData.hp = statData.maxHp;

        if (statData.mp >= statData.maxMp)
            statData.mp = statData.maxMp;

        if (statData.hp <= 0)
            statData.hp = 0;

        if (statData.mp <= 0)
            statData.mp = 0;
    }
    private void CheckHyperBodyStatus()
    {
        if (statData.hp >= statData.buffMaxHp)
            statData.hp = statData.buffMaxMp;

        if (statData.mp >= statData.buffMaxMp)
            statData.mp = statData.buffMaxMp;

        if (statData.hp <= 0)
            statData.hp = 0;

        if (statData.mp <= 0)
            statData.mp = 0;
    }









    public void SaveStat()
    {
        string data = JsonUtility.ToJson(statData, true);
        File.WriteAllText(path + "Stat" + ".json", data);
    }
    public void LoadStatData()
    {
        string StatDataPath = path + StatDataFilename + ".json";

        if (File.Exists(StatDataPath))
        {
            string StatdataJson = File.ReadAllText(StatDataPath);
            statData = JsonUtility.FromJson<StatData>(StatdataJson);
        }
    }
}
