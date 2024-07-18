using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.UI;

public class PlayerController : MonoBehaviour
{
    public Animator mAnimator;
    Camera mCamera;
    CharacterController mController;
    CameraController mCameraController;
    private EquipmentManager equipmentManager;

    public float speed = 5f;
    public float runSpeed = 8f;
    public float finalSpeed;
    public float stepHeight = 0.3f; // 계단 높이
    public float stepSmooth = 2f; // 계단 이동의 부드러움
    public float gravity = -9.81f; // 중력
    private Vector3 velocity;

    public bool toggleCameraRotation;
    public bool run;
    public bool isAttacking;

    public float smoothness = 10f;

    public GameObject currentWeapon;
    public GameObject currentShield;

    private void Start()
    {
        mAnimator = GetComponentInChildren<Animator>();
        mCamera = Camera.main;
        mController = GetComponent<CharacterController>();
        mCameraController = GameObject.Find("Camera").GetComponent<CameraController>();
        equipmentManager = GetComponent<EquipmentManager>();
    }

    private void Update()
    {
        /////////////////////////////////
        if (Input.GetKeyDown(KeyCode.LeftAlt))
            toggleCameraRotation = !toggleCameraRotation; // toggle 값 전환
        
        /////////////////////////////////
        if (Input.GetKey(KeyCode.LeftShift))
            run = true; // 달리기 활성화
        else                                       
            run = false; // 달리기 비활성화
        ////////////////////////////////////

        if (!mCameraController.interaction && !isAttacking)
        {
            InputMovement();
        }

        // 중력 적용
        if (!mController.isGrounded)
        {
            velocity.y += gravity * Time.deltaTime;
            mController.Move(velocity * Time.deltaTime);
        }
        else
        {
            velocity.y = 0f;
        }

    }

    private void LateUpdate()
    {
        if (!toggleCameraRotation && !mCameraController.interaction)
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

        finalSpeed = HandleStairs(finalSpeed);

        Vector3 moveDirection = (forward * vertical + right * horizontal).normalized;

        mController.Move(moveDirection * finalSpeed * Time.deltaTime);

        mAnimator.SetFloat("Horizontal", horizontal, 0.1f, Time.deltaTime);
        mAnimator.SetFloat("Vertical", vertical, 0.1f, Time.deltaTime);

        float percent = ((run) ? 1 : 0) * moveDirection.magnitude;
        mAnimator.SetFloat("Speed", percent, 0.1f, Time.deltaTime);
    }

    private float HandleStairs(float finalSpeed)
    {
        if (mController.isGrounded)
        {
            RaycastHit hit;

            // 캐릭터의 발 아래로 레이캐스트를 쏴서 계단을 감지
            if (Physics.Raycast(transform.position, transform.forward, out hit, mController.radius + 0.1f))
            {
                if (hit.normal.y < 0.6f)
                {
                    // 계단 앞쪽으로 추가 레이캐스트를 쏴서 계단의 높이를 감지
                    RaycastHit stepHit;
                    Vector3 stepRayOrigin = transform.position + new Vector3(0, stepHeight, 0);
                    if (Physics.Raycast(stepRayOrigin, transform.forward, out stepHit, mController.radius + 0.2f))
                    {
                        // 계단을 올라가기 위한 위치 조정
                        mController.Move(Vector3.up * stepSmooth * Time.deltaTime);
                    }
                }
            }
        }
        return finalSpeed;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("SoulEater"))
        {
            Debug.Log("Player hit by Monster!");
            // 플레이어에게 데미지 입히기 등
        }
    }

    public void EquipWeapon(Item item, GameObject weaponPrefab, string prefabPath)
    {
        equipmentManager.EquipItem(item,weaponPrefab, "Weapon", prefabPath);
        equipmentManager.SaveEquippedItems();
    }
    public void EquipShield(Item item, GameObject shieldPrefab, string prefabPath)
    {
        equipmentManager.EquipItem(item, shieldPrefab, "Shield", prefabPath);
        equipmentManager.SaveEquippedItems();
    }

}
