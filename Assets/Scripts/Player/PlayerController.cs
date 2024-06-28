using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Animator mAnimator;
    Camera mCamera;
    CharacterController mController;

    public float speed = 5f;
    public float runSpeed = 8f;
    public float finalSpeed;

    public bool toggleCameraRotation;
    public bool run;

    public float smoothness = 10f;

    public bool isAttacking;

    private void Start()
    {
        mAnimator = GetComponentInChildren<Animator>();
        mCamera = Camera.main;
        mController = GetComponent<CharacterController>();  
    }

    private void Update()
    {

        /////////////////////////////////
        if(Input.GetKey(KeyCode.LeftAlt))
            toggleCameraRotation = true; // 둘러보기 활성화
        else
            toggleCameraRotation = false; // 둘러보기 비활성화
        /////////////////////////////////
        if (Input.GetKey(KeyCode.LeftShift))
            run = true; // 달리기 활성화
        else                                       
            run = false; // 달리기 비활성화
        ////////////////////////////////////

        if(!isAttacking)
        InputMovement();

    }

    private void LateUpdate()
    {
        if(!toggleCameraRotation)
        {
            Vector3 playerRotate = Vector3.Scale(mCamera.transform.forward, new Vector3(1, 0, 1));
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(playerRotate), Time.deltaTime * smoothness);
        }
    }

    private void InputMovement()
    {
        finalSpeed = (run) ? runSpeed : speed;

        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 moveDirection = (forward * vertical + right * horizontal).normalized;

        mController.Move(moveDirection * finalSpeed * Time.deltaTime);

        mAnimator.SetFloat("Horizontal", horizontal, 0.1f, Time.deltaTime);
        mAnimator.SetFloat("Vertical", vertical, 0.1f, Time.deltaTime);

        float percent = ((run) ? 1 : 0) * moveDirection.magnitude;
        mAnimator.SetFloat("Speed", percent, 0.1f, Time.deltaTime);
    }
}
