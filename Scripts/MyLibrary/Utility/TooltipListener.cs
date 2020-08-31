using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class TooltipListener : MonoBehaviour
{
    [SerializeField]
    private Text _tooltipText = null;


    private CanvasGroup _canvasGroup = null;


    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
    }

    private void OnEnable()
    {
        _canvasGroup.alpha = 0f;
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }


    public void ShowTooltip(string text)
    {
        if (_tooltipText.text != text)
        {
            _tooltipText.text = text;
        }

        StopAllCoroutines();
        StartCoroutine(ChangeVisible(true));
    }

    public void HideTooltip()
    {
        StopAllCoroutines();
        StartCoroutine(ChangeVisible(false));
    }

    /// <summary>
    /// 非表示／表示切り替え
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    private IEnumerator ChangeVisible(bool value, float duration = 0.2f)
    {
        float startTime = Time.unscaledTime;

        float startAlpha = value ? 0f : 1f;
        float endAlpha = value ? 1f : 0f;

        _canvasGroup.alpha = startAlpha;

        while (Time.unscaledTime < startTime + duration)
        {
            float progress = Mathf.Clamp01((Time.unscaledTime - startTime) / duration);

            _canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, progress);

            yield return null;
        }

        _canvasGroup.alpha = endAlpha;
    }
}
