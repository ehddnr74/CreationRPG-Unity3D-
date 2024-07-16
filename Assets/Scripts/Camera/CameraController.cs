using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform followTarget;  // 따라다닐 타겟 (플레이어)
    public float followSpeed = 10f;
    public float sensitivity = 100f;
    public float clampAngle = 70f;

    private float rotX;
    private float rotY;

    public Transform realCamera;
    public Vector3 dirNormalized;
    public Vector3 finalDir;
    public float minDistance;
    public float maxDistance;
    public float finalDistance;
    public float smoothness = 10f;

    public int isUIActiveCount = 0;

    public bool interaction;
    public bool pressTab = false;

    Vector3 originPos;
    Quaternion originRot;

    private void Start()
    {
        rotX = transform.localRotation.eulerAngles.x;
        rotY = transform.localRotation.eulerAngles.y;

        dirNormalized = realCamera.localPosition.normalized;
        finalDistance = realCamera.localPosition.magnitude;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Tab) && isUIActiveCount < 1)
        {
            pressTab = !pressTab;

            if (pressTab)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }

        if (isUIActiveCount < 1 && !interaction && !pressTab) // UI가 활성화된 경우 회전하지 않음
        {
            rotX += -(Input.GetAxisRaw("Mouse Y")) * sensitivity * Time.deltaTime;
            rotY += Input.GetAxisRaw("Mouse X") * sensitivity * Time.deltaTime;

            rotX = Mathf.Clamp(rotX, -clampAngle, clampAngle);
            Quaternion rot = Quaternion.Euler(rotX, rotY, 0);
            transform.rotation = rot;
        }
    }

    private void LateUpdate()
    {
        if (!interaction)
        {
            transform.position = Vector3.MoveTowards(transform.position, followTarget.position, followSpeed * Time.deltaTime);

            finalDir = transform.TransformPoint(dirNormalized * maxDistance);

            RaycastHit hit;

            if (Physics.Linecast(transform.position, finalDir, out hit))
            {
                if (hit.collider.tag != "SoulEater" && hit.collider.tag != "Weapon")
                {
                    finalDistance = Mathf.Clamp(hit.distance, minDistance, maxDistance);
                }
            }
            else
            {
                finalDistance = maxDistance;
            }
            realCamera.localPosition = Vector3.Lerp(realCamera.localPosition, dirNormalized * finalDistance, Time.deltaTime * smoothness);
        }
    }

    // UI 활성화 상태를 설정하는 메서드
    public void SetUIActiveCount(bool active)
    {
        if (active)
            isUIActiveCount++;
        else
        {
            isUIActiveCount--;
            if(isUIActiveCount == 0)
            {
                pressTab = false;
            }
        }


        if (isUIActiveCount > 0)  
        {
            // UI가 활성화되면 커서 보이기
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            // UI가 비활성화되면 커서 숨기기
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    public void CameraTargetting(Transform target, float camSpeed = 0.05f)
    {
        if (target != null)
        {
            StopAllCoroutines();
            interaction = true;
            StartCoroutine(CameraTargettingCoroutine(target, camSpeed));
        }
    }
    public void CameraReset()
    {
        StopAllCoroutines();
        StartCoroutine(CameraResetCoroutine());
    }

    public void CameraOriginSetting()
    {
        originPos = transform.position;
        originRot = transform.rotation;
    }

    IEnumerator CameraTargettingCoroutine(Transform target, float camSpeed = 0.15f)
    {
        Vector3 targetPos = target.position;
        Vector3 targetFrontPos = targetPos + (target.forward);
        Vector3 direction = (targetPos - targetFrontPos).normalized;

        while (Vector3.Distance(transform.position, targetFrontPos) > 0.1f || Quaternion.Angle(transform.rotation, Quaternion.LookRotation(direction)) >= 0.5f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetFrontPos, camSpeed);
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(direction), camSpeed);
            yield return null;// Raycast를 사용하여 카메라와 플레이어 사이의 충돌 감지
        }
    }
    IEnumerator CameraResetCoroutine(float camSpeed = 0.08f)
    {
        yield return new WaitForSeconds(0.3f);

        while (Vector3.Distance(transform.position, originPos) > 0.1f || Quaternion.Angle(transform.rotation, originRot) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, originPos, camSpeed);
            transform.rotation = Quaternion.Lerp(transform.rotation, originRot, camSpeed);
            yield return null;// Raycast를 사용하여 카메라와 플레이어 사이의 충돌 감지
        }
        interaction = false;
    }


}
