using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class UiMove : MonoBehaviour
{
    private RectTransform _rectTransform = null;

    [SerializeField]
    private Vector3 _offset = Vector3.up;

    [SerializeField]
    private float _duration = 1f;


    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
    }

    private void Start()
    {
        _rectTransform.DOLocalMove(_rectTransform.localPosition + _offset, _duration).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo).SetLink(gameObject);
    }
}
