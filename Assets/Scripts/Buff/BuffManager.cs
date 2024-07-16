using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuffManager : MonoBehaviour
{
    public static BuffManager instance;

    public GameObject buffPanel;
    public GameObject buffIconPrefab; // 버프 아이콘 프리팹
    public ObjectPool buffIconPool; // 오브젝트 풀
    private Dictionary<string, GameObject> activeBuffs = new Dictionary<string, GameObject>(); // 활성화된 버프 관리
    private Dictionary<string, Coroutine> activeBuffCoroutines = new Dictionary<string, Coroutine>(); // Coroutine 인스턴스 저장

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ActivateBuff(string buffName, Sprite icon, float duration)
    {
        if (!activeBuffs.ContainsKey(buffName))
        {
            GameObject newBuffIcon = buffIconPool.GetObject();
            newBuffIcon.transform.SetParent(buffPanel.transform);
            Image buffImage = newBuffIcon.GetComponent<Image>();
            buffImage.sprite = icon;

            // 자식 텍스트 찾기
            TextMeshProUGUI durationText = newBuffIcon.GetComponentInChildren<TextMeshProUGUI>();
            durationText.text = Mathf.CeilToInt(duration).ToString();

            activeBuffs[buffName] = newBuffIcon;
            Coroutine buffCoroutine = StartCoroutine(UpdateText(buffName, durationText, duration));
            activeBuffCoroutines[buffName] = buffCoroutine;

            newBuffIcon.GetComponent<BuffDT>().buffName = buffName;
            BuffCoolDown buffCollDown = newBuffIcon.GetComponent<BuffCoolDown>();
            buffCollDown.StartCooldown(duration);
        }
        else
        {
            // 버프가 이미 활성화된 경우 기존 코루틴을 멈추고 새로운 코루틴 시작 
            if (activeBuffCoroutines.ContainsKey(buffName))
            {
                StopCoroutine(activeBuffCoroutines[buffName]);
            }
            TextMeshProUGUI durationText = activeBuffs[buffName].GetComponentInChildren<TextMeshProUGUI>();
            Coroutine buffCoroutine = StartCoroutine(UpdateText(buffName, durationText, duration));
            activeBuffCoroutines[buffName] = buffCoroutine;

            activeBuffs[buffName].GetComponent<BuffDT>().buffName = buffName;
            BuffCoolDown buffCollDown = activeBuffs[buffName].GetComponent<BuffCoolDown>();
            buffCollDown.ResetCooldown();
        }
    }

    private IEnumerator UpdateText(string buffName, TextMeshProUGUI durationText, float duration)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float remainingTime = duration - elapsedTime;
            durationText.text = Mathf.CeilToInt(remainingTime).ToString();
            yield return null;
        }

        if (activeBuffs.ContainsKey(buffName))
        {
            buffIconPool.ReturnObject(activeBuffs[buffName]);
            activeBuffs.Remove(buffName);
            activeBuffCoroutines.Remove(buffName);

            if (buffName == "금강불괴")
            {
                StatManager.instance.UpdateStatDeactiveHyperBody();
            }
            if (buffName == "화룡점정")
            {
                StatManager.instance.UpdateStatDeactiveCriticalSkill();
            }
        }
    }
    public void DeactivateBuffOfMouseClick(string buffName)
    {
        if (activeBuffs.ContainsKey(buffName))
        {
            buffIconPool.ReturnObject(activeBuffs[buffName]);
            activeBuffs.Remove(buffName);
            activeBuffCoroutines.Remove(buffName);

            if (buffName == "금강불괴")
            {
                StatManager.instance.UpdateStatDeactiveHyperBody();
            }
            if (buffName == "화룡점정")
            {
                StatManager.instance.UpdateStatDeactiveCriticalSkill();
            }
        }
    }
}
