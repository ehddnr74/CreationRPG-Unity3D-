using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuffCoolDown : MonoBehaviour
{
    public Image coolDownImage;
    public float coolDownDuration;

    private bool isCoolDown = false;
    private bool resetTrigger = false;

    private float coolDownTimer;


    void Update()
    {
        if (isCoolDown)
        {
            if(resetTrigger) // 스킬 재사용 시 쿨다운 리셋
            {
                ResetCooldown();
            }
            coolDownTimer -= Time.deltaTime;
            coolDownImage.fillAmount = coolDownTimer / coolDownDuration;

            if (coolDownTimer <= 0)
            {
                isCoolDown = false;
                coolDownImage.fillAmount = 0;
            }
        }
    }

    public void StartCooldown(float cooldownDuration)
    {
        coolDownDuration = cooldownDuration;
        if (!isCoolDown)
        {
            isCoolDown = true;
            coolDownTimer = coolDownDuration;
            coolDownImage.fillAmount = 1;
        }
    }

    public void ResetCooldown()
    {
        resetTrigger = false;

        coolDownTimer = coolDownDuration;
        coolDownImage.fillAmount = 1;
    }
}
