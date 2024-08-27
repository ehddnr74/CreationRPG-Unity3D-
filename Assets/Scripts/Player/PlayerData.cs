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
    public Dictionary<int, int> experienceTable; // ������ ����ġ �䱸��
    public Dictionary<int, int> baseHPTable; // ������ �⺻ HP
    public Dictionary<int, int> baseMPTable; // ������ �⺻ MP
}