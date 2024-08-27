using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SkillData
{
    public string skillName; // 스킬 이름
    public string skillType; // 스킬 타입 (공격,패시브,버프 등)
    public string skillDescription; // 스킬 설명
    public int skillLevel; // 스킬 레벨
    public int minLevel; // 스킬 최소 레벨
    public int maxLevel; // 스킬 최대 레벨
    public string iconPath; // 스킬 아이콘 경로
    public Dictionary<int, SkillEffect> levelEffects; // 스킬 레벨별 효과
}

[System.Serializable]
public class SkillCollection
{
    public Dictionary<string, SkillData> skills = new Dictionary<string, SkillData>(); // 스킬 목록
    public int skillPoint;
}

[System.Serializable]
public class SkillEffect
{
    public string descrption; // 레벨 단위 스킬 효과 설명
    public int hpReduction; // HP 소모량
    public int mpReduction; // MP 소모량
    public float damageIncrease; // 데미지 증가량
    public float coolDown; //쿨타임
    public float buffDuration; // 버프 지속시간
    public float cooldownReduction; // 쿨타임 감소량
    public float duration; // 지속 시간
    public float hpIncrease; // HP 증가량
    public float mpIncrease; // MP 증가량
    public float speedIncrease; // 속도 증가량
    public float jumpDistanceIncrease; // 점프 증가량
    public float attackSpeedIncrease; // 공격 속도 증가량
    public float attackPowerIncrease; // 공격력 증가량
    public float defenseIncrease; // 방어력 증가량
    public float criticalChanceIncrease; // 크리티컬 확률 증가량
    public float criticalDamageIncrease; // 크리티컬 데미지 증가량
}

