using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// パネルのベースクラス
/// </summary>
[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(CanvasGroup))]
public abstract class PanelBehaviour : MonoBehaviour
{
    protected RectTransform _rectTransform = null;
    protected CanvasGroup _canvasGroup = null;

    protected virtual void Reset()
    {
        // 上位のキャンバスに合わせる
        var rectTransform = GetComponent<RectTransform>();
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.anchoredPosition = Vector2.zero;
        rectTransform.sizeDelta = Vector2.zero;
    }

    protected virtual void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _canvasGroup = GetComponent<CanvasGroup>();
    }


    /// <summary>
    /// 本パネルを開く
    /// </summary>
    public virtual void Open()
    {
        OpenProcess(null, null);
    }

    /// <summary>
    /// 本パネルを閉じる
    /// </summary>
    public virtual void Close()
    {
        CloseProcess(null, null);
    }

    /// <summary>
    /// 本パネルを開く
    /// </summary>
    public virtual void Open(UnityAction onPrepared = null, UnityAction onCompleted = null)
    {
        OpenProcess(onPrepared, onCompleted);
    }

    /// <summary>
    /// 本パネルを閉じる
    /// </summary>
    public virtual void Close(UnityAction onPrepared = null, UnityAction onCompleted = null)
    {
        CloseProcess(onPrepared, onCompleted);
    }


    /// <summary>
    /// パネルを開く動作
    /// </summary>
    protected virtual void OpenProcess(UnityAction onPrepared = null, UnityAction onCompleted = null)
    {
        onPrepared?.Invoke();

        _canvasGroup.interactable = true;
        gameObject.SetActive(true);

        onCompleted?.Invoke();

        OnOpened?.Invoke();
    }

    /// <summary>
    /// パネルを閉じる動作
    /// </summary>
    protected virtual void CloseProcess(UnityAction onPrepared = null, UnityAction onCompleted = null)
    {
        onPrepared?.Invoke();

        _canvasGroup.interactable = false;
        gameObject.SetActive(false);

        onCompleted?.Invoke();

        OnClosed?.Invoke();
    }


    protected UnityEvent _onOpened = new UnityEvent();
    protected UnityEvent _onClosed = new UnityEvent();

    /// <summary>
    /// パネルを開いた後の処理
    /// </summary>
    public UnityEvent OnOpened => _onOpened;

    /// <summary>
    /// パネルを閉じた後の処理
    /// </summary>
    public UnityEvent OnClosed => _onClosed;
}
