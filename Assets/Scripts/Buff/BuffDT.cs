using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BuffDT : MonoBehaviour, IPointerClickHandler
{
    public string buffName;
    void Start()
    {
        
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            BuffManager.instance.DeactivateBuffOfMouseClick(buffName);
        }
    }
}
