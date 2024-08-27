using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Quest
{
    public int id;
    public int possibleLevel; // 시작 가능 레벨
    public string npcName; // 퀘스트를 받을 NPC 이름
    public bool alreadyAccept; // 받은적 있는 퀘스트인지 
    public string name; // 퀘스트 이름 
    public string description; // 설명 
    public string objective; // 해야할 것  
    public int currentKillCount; // 사냥 의뢰 시 현재 잡은 마릿 수 
    public int targetKillCount; // 사냥 의뢰 시 잡아야하는 마릿 수
    public int currentCollectionCount; // 수집 의뢰 시 현재 모은 갯수
    public int maxCollectionCount; // 수집 의뢰 시 모아야하는 갯수
    public Reward reward;
    public string status; // 시작 가능 , 진행 중 , 완료
}

[System.Serializable]
public class Reward
{
    public int experience;
    public int gold;
    public List<RewardItem> items;
}

[System.Serializable]
public class RewardItem
{
    public int itemId;
    public int amount;
}

[System.Serializable]
public class QuestData
{
    public List<Quest> quests;
}

