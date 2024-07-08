using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventForwarder : MonoBehaviour
{
    private CameraController cameraController;
    private PlayerController parentController;
    private Animator mAnimator;

    private int comboStep = 0;
    private float comboTimer = 0f;
    public float comboMaxDelay = 1f; // �޺� �Է� ���� �ִ� �ð�

    public bool canAttack = true;

    private void Start()
    {
        cameraController = GameObject.Find("Camera").GetComponent<CameraController>();
        parentController = GetComponentInParent<PlayerController>();
        mAnimator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && canAttack && cameraController.isUIActiveCount <= 0) // UIȰ��ȭ �߿��� ���� �ȵǰ� ����
        {
            parentController.isAttacking = true;
            ComboAttackTrigger();
        }

        // �޺� Ÿ�̸� ������Ʈ
        if (comboStep > 0)
        {
            comboTimer += Time.deltaTime;
            if (comboTimer > comboMaxDelay)
            {
                ResetCombo();
            }
        }
    }

    private void ComboAttackTrigger()
    {
        canAttack = false; // ���� ���� ���� ���� �Ұ����ϵ��� ����
        comboTimer = 0f; // �޺� Ÿ�̸� �ʱ�ȭ

        if (comboStep == 0)
        {
            mAnimator.SetTrigger("Attack1");
            comboStep = 1;
        }
        else if (comboStep == 1)
        {
            mAnimator.SetTrigger("Attack2");
            comboStep = 2;
        }
        else if (comboStep == 2)
        {
            if (Random.value > 0.5f)
            {
                mAnimator.SetTrigger("FinishAttack1");
            }
            else
            {
                mAnimator.SetTrigger("FinishAttack2");
            }
            comboStep = 0; 
        }
    }

    public void ResetCombo()
    {
        comboStep = 0;
        comboTimer = 0f;
        mAnimator.ResetTrigger("Attack1");
        mAnimator.ResetTrigger("Attack2");
        mAnimator.ResetTrigger("FinishAttack1");
        mAnimator.ResetTrigger("FinishAttack2");
        canAttack = true;
        parentController.isAttacking = false;
    }

    public void EnableCombo()
    {
        canAttack = true; // ���� ������ ������ �ٽ� ���� �����ϵ��� ����
    }

}
