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
        skillMasterLevelText.text = $"[마스터레벨: {skillMasterLevel}]";
        skillTooltip.SetActive(true);
        skillTooltip.transform.position = Input.mousePosition;

        if (currentLevel == skillMinLevel) // 현재 스킬레벨이 해당 스킬의 최소레벨과 같을때 (=0일 때)
        {
            skillCurrentLevelText.text = $"[현재레벨: {currentLevel}]";
            skillCurrentLevelDescriptionText.text = "스킬사용 불가능";
            int nextLevel = currentLevel + 1;
            skillNextLevelText.text = $"[다음레벨: {nextLevel}]";
            skillNextLevelDescriptionText.text = "다음레벨설명!";
        }
        else if (currentLevel == skillMasterLevel) // 현재 스킬레벨이 해당 스킬의 마스터레벨과 같을 때
        {
            skillCurrentLevelText.text = "[Skill Master]";
            skillCurrentLevelDescriptionText.text = "현재레벨설명!";
            skillNextLevelText.text = "";
            skillNextLevelDescriptionText.text = "";
        }
        else if(currentLevel != skillMinLevel && currentLevel < skillMasterLevel) // 현재 스킬레벨이 최소레벨보다 높고 마스터레벨보다 작을 때
        {                                                                         // 즉 (0보다 크고 마스터레벨보다 작을 때)
            skillCurrentLevelText.text = $"[현재레벨: {currentLevel}]";
            skillCurrentLevelDescriptionText.text = "현재레벨설명!";
            int nextLevel = currentLevel + 1;
            skillNextLevelText.text = $"[다음레벨: {nextLevel}]";
            skillNextLevelDescriptionText.text = "다음레벨설명!";
        }
    }

    public void Deactivate()
    {
        skillTooltip.SetActive(false);
    }
}
