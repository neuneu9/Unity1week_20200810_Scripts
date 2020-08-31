using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// マウスオーバーでツールチップを表示する
/// </summary>
public class TooltipSource : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public string text = string.Empty;

    private TooltipListener _listener = null;


    private void Awake()
    {
        _listener = FindObjectOfType<TooltipListener>();
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        StartHover();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        StopHover();
    }

    private void StartHover()
    {
        _listener.ShowTooltip(text);
    }

    private void StopHover()
    {
        _listener.HideTooltip();
    }
}
