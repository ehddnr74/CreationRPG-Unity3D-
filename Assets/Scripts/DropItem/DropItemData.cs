using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropItemData : MonoBehaviour
{
    public Item item; // 드랍된 아이템의 데이터
    private DropItemPool dropItemPool;

    private Coroutine autoDestroyCoroutine; // 아이템 자동 소멸 코루틴

    public void Initialize(Item newItem, DropItemPool pool)
    {
        item = newItem;
        dropItemPool = pool;

        // 아이템 자동 소멸 코루틴 시작
        if (autoDestroyCoroutine != null)
        {
            StopCoroutine(autoDestroyCoroutine);
        }
        autoDestroyCoroutine = StartCoroutine(AutoDestroy());
    }

    private IEnumerator AutoDestroy()
    {
        yield return new WaitForSeconds(20f); // 20초 대기
        ReturnToPool(); // 아이템을 풀로 반환
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
