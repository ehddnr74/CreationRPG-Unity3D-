using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventForwarder : MonoBehaviour
{
    private PlayerController parentController;

    private void Start()
    {
        parentController = GetComponentInParent<PlayerController>();
    }

    //public void ResetAttack()
    //{
    //    if (parentController != null)
    //    {
    //        parentController.ResetAttack();
    //    }
    //}
}
