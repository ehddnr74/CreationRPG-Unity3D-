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

    private MonsterSpawner monsterSpawner; // ���� ������ ����
    private Inventory inv;
    private DropItemInfoPool dropItemInfoPool;

    public int maxHealth = 200; // �ִ� ü��
    private int currentHealth;

    public List<DropItem> dropTable; // ��� ���̺� �߰�
    public DropItemPool dropItemPool; // ������ Ǯ ����

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
            mouthCollider.enabled = true; // �ݶ��̴� Ȱ��ȭ
        }
    }
    public void DisableBasicAttack()
    {
        if (mouthCollider != null)
        {
            mouthCollider.enabled = false; // �ݶ��̴� Ȱ��ȭ
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
    ////        // �÷��̾�� ������ ������ ��
    ////    }
    ////}
    ///
    private IEnumerator DieCoroutine()
    {
        mouthCollider.enabled = false; // �ݶ��̴� ��Ȱ��ȭ
        bodyCol.enabled = false;

         yield return new WaitForSeconds(1.5f);
        mouthCollider.enabled = true;
        currentHealth = maxHealth; // �ʱ� ü���� �ִ� ü������ ����
        DropItems(); // ������ ��� ���� �߰�
        if (QuestManager.instance.questData.quests[0].status == "������")
        {
            if (QuestManager.instance.questData.quests[0].currentKillCount < QuestManager.instance.questData.quests[0].targetKillCount)
            {
                QuestManager.instance.questData.quests[0].currentKillCount++;
                QuestManager.instance.SaveQuests();
            }
        }
        monsterSpawner.DespawnMonster(gameObject); // ���͸� Ǯ�� ��ȯ
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
