using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SkillData
{
    public string skillName; // ��ų �̸�
    public string skillType; // ��ų Ÿ�� (����,�нú�,���� ��)
    public int skillLevel; // ��ų ����
    public int minLevel; // ��ų �ּ� ����
    public int maxLevel; // ��ų �ִ� ����
    public string iconPath; // ��ų ������ ���
    public Dictionary<int, SkillEffect> levelEffects; // ��ų ������ ȿ��
}

[System.Serializable]
public class SkillCollection
{
    public Dictionary<string, SkillData> skills = new Dictionary<string, SkillData>(); // ��ų ���
    public int skillPoint;
}

[System.Serializable]
public class SkillEffect
{
    public int hpReduction; // HP �Ҹ�
    public int mpReduction; // MP �Ҹ�
    public float damageIncrease; // ������ ������
    public float coolDown; //��Ÿ��
    public float buffDuration; // ���� ���ӽð�
    public float cooldownReduction; // ��Ÿ�� ���ҷ�
    public float duration; // ���� �ð�
    public float hpIncrease; // HP ������
    public float mpIncrease; // MP ������
    public float speedIncrease; // �ӵ� ������
    public float jumpDistanceIncrease; // ���� ������
    public float attackSpeedIncrease; // ���� �ӵ� ������
    public float attackPowerIncrease; // ���ݷ� ������
    public float defenseIncrease; // ���� ������
    public float criticalChanceIncrease; // ũ��Ƽ�� Ȯ�� ������
    public float criticalDamageIncrease; // ũ��Ƽ�� ������ ������
}

