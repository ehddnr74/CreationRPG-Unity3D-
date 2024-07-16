using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventForwarder : MonoBehaviour
{
    private CameraController cameraController;
    private PlayerController parentController;
    private Animator mAnimator;

    public int comboStep = 0;
    private float comboTimer = 0f;
    public float comboMaxDelay = 1f; // 콤보 입력 가능 최대 시간

    public bool canAttack = true;

    public bool swordSkill = false;
    public bool lightningstrike = false; // 벽력일섬
    public bool atrocities = false; // 극악무도
    public bool naturaldisaster = false; //천재지변



    private void Start()
    {
        cameraController = GameObject.Find("Camera").GetComponent<CameraController>();
        parentController = GetComponentInParent<PlayerController>();
        mAnimator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && canAttack && cameraController.isUIActiveCount <= 0 && !cameraController.pressTab) // UI활성화 중에는 공격 안되게 막음
        {
            parentController.isAttacking = true;
            ComboAttackTrigger();
        }

        // 콤보 타이머 업데이트
        if (comboStep > 0)
        {
            comboTimer += Time.deltaTime;
            if (comboTimer > comboMaxDelay)
            {
                ResetCombo();
            }
        }
    }

    public void ComboAttackTrigger()
    {
        canAttack = false; // 공격 중일 때는 공격 불가능하도록 설정
        comboTimer = 0f; // 콤보 타이머 초기화

        if (comboStep == 0)
        {
            mAnimator.SetTrigger("Attack1");
            comboStep = 1;
        }
        else if (comboStep == 1)
        {
            mAnimator.SetTrigger("Attack2");
        }
        else if (comboStep == 2)
        {
            if (lightningstrike)
            {
                lightningstrike = false;
                mAnimator.SetTrigger("FinishAttack1");
            }
            else if(naturaldisaster)
            {
                naturaldisaster = false;
                mAnimator.SetTrigger("FinishAttack2");
            }
            else if(atrocities)
            {
                atrocities = false;
                mAnimator.SetTrigger("FinishAttack3");
            }
        }
    }

    public void ResetCombo()
    {
        swordSkill = false;
        comboStep = 0;
        comboTimer = 0f;
        mAnimator.ResetTrigger("Attack1");
        mAnimator.ResetTrigger("Attack2");
        mAnimator.ResetTrigger("FinishAttack1");
        mAnimator.ResetTrigger("FinishAttack2");
        mAnimator.ResetTrigger("FinishAttack3");
        canAttack = true;
        parentController.isAttacking = false;
    }

    public void EnableCombo()
    {
        canAttack = true; // 현재 공격이 끝나면 다시 공격 가능하도록 설정
    }

    public void EnableWeaponCollider()
    {
        if (parentController.currentWeapon != null)
        {
            parentController.currentWeapon.GetComponent<Collider>().enabled = true;
        }
    }

    public void DisableWeaponCollider()
    {
        if (parentController.currentWeapon != null)
        {
            parentController.currentWeapon.GetComponent<Collider>().enabled = false;
        }
    }

    public void EnableWeaponTrail()
    {
        parentController.currentWeapon.GetComponentInChildren<TrailRenderer>().enabled = true;
    }

    public void DisableWeaponTrail()
    {
        parentController.currentWeapon.GetComponentInChildren<TrailRenderer>().enabled = false;
    }

    public void EnableSwordSkill()
    {
        swordSkill = true;
    }
}
