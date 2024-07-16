using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

[System.Serializable]
public class PlayerData
{
    public string name;
    public int level;
    public int gold;
    public int experience;
    public Dictionary<int, int> experienceTable; // 레벨별 경험치 요구량
    public Dictionary<int, int> baseHPTable; // 레벨별 기본 HP
    public Dictionary<int, int> baseMPTable; // 레벨별 기본 MP
}