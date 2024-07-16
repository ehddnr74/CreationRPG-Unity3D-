using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SkillToolTip : MonoBehaviour
{
    private GameObject skillTooltip;

    public TextMeshProUGUI skillNameText;
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

    public void Activate(string skillName, int skillMinLevel, int skillMasterLevel, int currentLevel)
    {
        skillNameText.text = skillName;
        skillMasterLevelText.text = $"[�����ͷ���: {skillMasterLevel}]";
        skillTooltip.SetActive(true);
        skillTooltip.transform.position = Input.mousePosition;

        if (currentLevel == skillMinLevel) // ���� ��ų������ �ش� ��ų�� �ּҷ����� ������ (=0�� ��)
        {
            skillCurrentLevelText.text = $"[���緹��: {currentLevel}]";
            skillCurrentLevelDescriptionText.text = "��ų��� �Ұ���";
            int nextLevel = currentLevel + 1;
            skillNextLevelText.text = $"[��������: {nextLevel}]";
            skillNextLevelDescriptionText.text = "������������!";
        }
        else if (currentLevel == skillMasterLevel) // ���� ��ų������ �ش� ��ų�� �����ͷ����� ���� ��
        {
            skillCurrentLevelText.text = "[Skill Master]";
            skillCurrentLevelDescriptionText.text = "���緹������!";
            skillNextLevelText.text = "";
            skillNextLevelDescriptionText.text = "";
        }
        else if(currentLevel != skillMinLevel && currentLevel < skillMasterLevel) // ���� ��ų������ �ּҷ������� ���� �����ͷ������� ���� ��
        {                                                                         // �� (0���� ũ�� �����ͷ������� ���� ��)
            skillCurrentLevelText.text = $"[���緹��: {currentLevel}]";
            skillCurrentLevelDescriptionText.text = "���緹������!";
            int nextLevel = currentLevel + 1;
            skillNextLevelText.text = $"[��������: {nextLevel}]";
            skillNextLevelDescriptionText.text = "������������!";
        }
    }

    public void Deactivate()
    {
        skillTooltip.SetActive(false);
    }
}
