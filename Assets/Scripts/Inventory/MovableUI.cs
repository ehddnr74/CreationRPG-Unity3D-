using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary> 헤더 드래그 앤 드롭에 의한 UI 이동 </summary>
public class MovableUI : MonoBehaviour, IPointerDownHandler, IDragHandler
{

    [SerializeField]
    private Transform _targetTr; // 이동될 UI

    private Vector2 _startingPoint;
    private Vector2 _moveBegin;
    private Vector2 _moveOffset;
    private Canvas _canvas;
    private RectTransform _canvasRectTransform;

    public static int _globalSortingOrderCounter = 0;

    private void Awake()
    {
        // 이동 대상 UI를 지정하지 않은 경우, 자동으로 부모로 초기화
        if (_targetTr == null)
            _targetTr = transform.parent;

        // Canvas 컴포넌트 참조
        _canvas = _targetTr.parent.GetComponent<Canvas>();
    }

    // 드래그 시작 위치 지정
    void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
    {
        // 드래그 시작 시 sorting order 최상단으로 설정
        _globalSortingOrderCounter++;
        _canvas.sortingOrder = _globalSortingOrderCounter;

        _startingPoint = _targetTr.position;
        _moveBegin = eventData.position;
    }

    // 드래그 : 마우스 커서 위치로 이동
    void IDragHandler.OnDrag(PointerEventData eventData)
    {
        _moveOffset = eventData.position - _moveBegin;
        _targetTr.position = _startingPoint + _moveOffset;
    }
}

    
