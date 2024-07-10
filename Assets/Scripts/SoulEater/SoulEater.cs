using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoulEater : MonoBehaviour
{
    public GameObject mouthSocket;
    public Collider mouthCollider;
    public Animator mAnimator;

    public int HP = 100;

    private void Start()
    {
        mouthCollider = mouthSocket.GetComponent<Collider>();
        mouthCollider.enabled = false;
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
        HP -= damageAmount;
        if (HP <= 0)
        {
            mAnimator.SetTrigger("Die");
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
}
