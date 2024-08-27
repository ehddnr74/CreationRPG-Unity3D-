using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropItemData : MonoBehaviour
{
    public Item item; // ����� �������� ������
    private DropItemPool dropItemPool;

    private Coroutine autoDestroyCoroutine; // ������ �ڵ� �Ҹ� �ڷ�ƾ

    public void Initialize(Item newItem, DropItemPool pool)
    {
        item = newItem;
        dropItemPool = pool;

        // ������ �ڵ� �Ҹ� �ڷ�ƾ ����
        if (autoDestroyCoroutine != null)
        {
            StopCoroutine(autoDestroyCoroutine);
        }
        autoDestroyCoroutine = StartCoroutine(AutoDestroy());
    }

    private IEnumerator AutoDestroy()
    {
        yield return new WaitForSeconds(20f); // 20�� ���
        ReturnToPool(); // �������� Ǯ�� ��ȯ
    }

    public void ReturnToPool()
    {
        if (dropItemPool != null)
        {
            dropItemPool.ReturnItem(gameObject);
        }
    }

    private void OnDisable()
    {
        if (dropItemPool != null)
        {
            dropItemPool.ReturnItem(gameObject);
        }
    }
}
