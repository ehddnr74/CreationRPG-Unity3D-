using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillToolTip : MonoBehaviour
{
    private GameObject skillTooltip; 

    public GameObject skillImage;
    public TextMeshProUGUI skillNameText;
    public TextMeshProUGUI skillDescriptionText;
    public TextMeshProUGUI skillMasterLevelText;
    public TextMeshProUGUI skillCurrentLevelText;
    public TextMeshProUGUI skillNextLevelText;

    public TextMeshProUGUI skillCurrentLevelDescriptionText;
    public TextMeshProUGUI skillNextLevelDescriptionText;


    void Start()
    {
        skillTooltip = GameObject.Find("SkillToolTip");
        skillTooltip.SetActive(false);
    }

    private void Update()
    {
        if(!SkillManager.instance.visibleSkill)
        {
            Deactivate();
        }
    }

    public void Activate(Sprite skillIcon, string skillName, string skillDescription, int skillMinLevel, int skillMasterLevel, int currentLevel, List<string> skillLevelDescription)
    {
        skillTooltip.SetActive(true);
        Image skillImg = skillImage.GetComponent<Image>();
        skillImg.sprite = skillIcon;
        skillNameText.text = skillName;
        skillMasterLevelText.text = $"[�����ͷ���: {skillMasterLevel}]";
        skillDescriptionText.text = skillDescription;
        skillTooltip.transform.position = Input.mousePosition;

        if (currentLevel == skillMinLevel) // ���� ��ų������ �ش� ��ų�� �ּҷ����� ������ (=0�� ��)
        {
            skillCurrentLevelText.text = $"[���緹��: {currentLevel}]";
            skillCurrentLevelDescriptionText.text = "��ų��� �Ұ���";
            int nextLevel = currentLevel + 1;
            skillNextLevelText.text = $"[��������: {nextLevel}]";
            skillNextLevelDescriptionText.text = skillLevelDescription[currentLevel];
        }
        else if (currentLevel == skillMasterLevel) // ���� ��ų������ �ش� ��ų�� �����ͷ����� ���� ��
        {
            skillCurrentLevelText.text = "[Skill Master]";
            skillCurrentLevelDescriptionText.text = skillLevelDescription[currentLevel - 1];
            skillNextLevelText.text = "";
            skillNextLevelDescriptionText.text = "";
        }
        else if(currentLevel != skillMinLevel && currentLevel < skillMasterLevel) // ���� ��ų������ �ּҷ������� ���� �����ͷ������� ���� ��
        {                                                                         // �� (0���� ũ�� �����ͷ������� ���� ��)
            skillCurrentLevelText.text = $"[���緹��: {currentLevel}]";
            skillCurrentLevelDescriptionText.text = skillLevelDescription[currentLevel - 1];
            int nextLevel = currentLevel + 1;
            skillNextLevelText.text = $"[��������: {nextLevel}]";
            skillNextLevelDescriptionText.text = skillLevelDescription[nextLevel - 1];
        }
    }

    public void Deactivate()
    {
        skillTooltip.SetActive(false);
    }
}
