using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary> ��� �巡�� �� ��ӿ� ���� UI �̵� </summary>
public class MovableUI : MonoBehaviour, IPointerDownHandler, IDragHandler
{

    [SerializeField]
    private Transform _targetTr; // �̵��� UI

    private Vector2 _startingPoint;
    private Vector2 _moveBegin;
    private Vector2 _moveOffset;
    private Canvas _canvas;
    private RectTransform _canvasRectTransform;

    public static int _globalSortingOrderCounter = 0;

    private void Awake()
    {
        // �̵� ��� UI�� �������� ���� ���, �ڵ����� �θ�� �ʱ�ȭ
        if (_targetTr == null)
            _targetTr = transform.parent;

        // Canvas ������Ʈ ����
        _canvas = _targetTr.parent.GetComponent<Canvas>();
    }

    // �巡�� ���� ��ġ ����
    void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
    {
        // �巡�� ���� �� sorting order �ֻ������ ����
        _globalSortingOrderCounter++;
        _canvas.sortingOrder = _globalSortingOrderCounter;

        _startingPoint = _targetTr.position;
        _moveBegin = eventData.position;
    }

    // �巡�� : ���콺 Ŀ�� ��ġ�� �̵�
    void IDragHandler.OnDrag(PointerEventData eventData)
    {
        _moveOffset = eventData.position - _moveBegin;
        _targetTr.position = _startingPoint + _moveOffset;
    }
}

    
