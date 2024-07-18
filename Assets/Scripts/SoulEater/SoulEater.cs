using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class SoulEater : MonoBehaviour
{
    public GameObject mouthSocket;
    public Collider mouthCollider;
    public Collider bodyCol;
    public Animator mAnimator;

    private MonsterSpawner monsterSpawner; // 몬스터 스포너 참조
    private Inventory inv;
    private DropItemInfoPool dropItemInfoPool;

    public int maxHealth = 200; // 최대 체력
    private int currentHealth;

    public List<DropItem> dropTable; // 드랍 테이블 추가
    public DropItemPool dropItemPool; // 아이템 풀 참조

    private void OnEnable()
    {
        mouthCollider = mouthSocket.GetComponent<Collider>();
        mouthCollider.enabled = false;
        bodyCol = GetComponent<Collider>();
        bodyCol.enabled = true;
        currentHealth = maxHealth;
        monsterSpawner = GameObject.Find("SoulEaterSpawner").GetComponent<MonsterSpawner>();
        dropItemPool = GameObject.Find("DropItemManager").GetComponent<DropItemPool>();
        dropItemInfoPool = GameObject.Find("DropItemInfoCanvas").GetComponent<DropItemInfoPool>();
        inv = GameObject.Find("Inventory").GetComponent<Inventory>();
    }

    public void EnableBasicAttack()
    {
        if(mouthCollider != null)
        {
            mouthCollider.enabled = true; // 콜라이더 활성화
        }
    }
    public void DisableBasicAttack()
    {
        if (mouthCollider != null)
        {
            mouthCollider.enabled = false; // 콜라이더 활성화
        }
    }




    public void TakeDamege(int damageAmount)
    {
        currentHealth -= damageAmount;
        if (currentHealth <= 0)
        {
            mAnimator.SetTrigger("Die");
            StartCoroutine(DieCoroutine());
        }
        else
        {
            mAnimator.SetTrigger("Hit");
        }
    }
    ////private void OnTriggerEnter(Collider other)
    ////{
    ////    if (other.CompareTag("Weapon"))
    ////    {
    ////        Debug.Log("Weapon Hit!");
    ////        // 플레이어에게 데미지 입히기 등
    ////    }
    ////}
    ///
    private IEnumerator DieCoroutine()
    {
        mouthCollider.enabled = false; // 콜라이더 비활성화
        bodyCol.enabled = false;

         yield return new WaitForSeconds(1.5f);
        mouthCollider.enabled = true;
        currentHealth = maxHealth; // 초기 체력을 최대 체력으로 설정
        DropItems(); // 아이템 드랍 로직 추가
        if (QuestManager.instance.questData.quests[0].status == "진행중")
        {
            if (QuestManager.instance.questData.quests[0].currentKillCount < QuestManager.instance.questData.quests[0].targetKillCount)
            {
                QuestManager.instance.questData.quests[0].currentKillCount++;
                QuestManager.instance.SaveQuests();
            }
        }
        monsterSpawner.DespawnMonster(gameObject); // 몬스터를 풀로 반환
    }

    private void DropItems()
    {
        List<Item> droppedItems = new List<Item>();
        foreach (var dropItem in dropTable)
        {
            if (Random.value <= dropItem.dropChance)
            {
                Item item = ItemDataBase.instance.FetchItemByID(dropItem.itemId);
                if (item != null)
                {
                    if (item.Name != "Gold")
                    {
                        inv.AddItem(item.ID);
                        droppedItems.Add(item);
                    }
                    else
                    {
                        int randomAddGold = Random.Range(100, 300);
                        item.gold = randomAddGold;
                        DataManager.instance.AddGold(randomAddGold);
                        droppedItems.Add(item);
                    }
                }
            }
        }
        foreach (Item item in droppedItems)
        {
            DropItemInfoManager.instance.AddDroppedItem(item);
        }
    }

}
