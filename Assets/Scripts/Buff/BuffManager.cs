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
    public GameObject buffIconPrefab; // ���� ������ ������
    public ObjectPool buffIconPool; // ������Ʈ Ǯ
    private Dictionary<string, GameObject> activeBuffs = new Dictionary<string, GameObject>(); // Ȱ��ȭ�� ���� ����
    private Dictionary<string, Coroutine> activeBuffCoroutines = new Dictionary<string, Coroutine>(); // Coroutine �ν��Ͻ� ����

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

            // �ڽ� �ؽ�Ʈ ã��
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
            // ������ �̹� Ȱ��ȭ�� ��� ���� �ڷ�ƾ�� ���߰� ���ο� �ڷ�ƾ ���� 
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

            if (buffName == "�ݰ��ұ�")
            {
                StatManager.instance.UpdateStatDeactiveHyperBody();
            }
            if (buffName == "ȭ������")
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

            if (buffName == "�ݰ��ұ�")
            {
                StatManager.instance.UpdateStatDeactiveHyperBody();
            }
            if (buffName == "ȭ������")
            {
                StatManager.instance.UpdateStatDeactiveCriticalSkill();
            }
        }
    }
}
