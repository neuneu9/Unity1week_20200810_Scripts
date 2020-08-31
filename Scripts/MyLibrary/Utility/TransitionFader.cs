using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// トランジションフェード制御
/// </summary>
[RequireComponent(typeof(IFadable))]
public class TransitionFader : MonoBehaviour
{
    /// <summary>
    /// フェードアウトで使うイージング曲線
    /// </summary>
    [SerializeField]
    private AnimationCurve _fadeOutEasingCurve = null;

    /// <summary>
    /// フェードインで使うイージング曲線
    /// </summary>
    [SerializeField]
    private AnimationCurve _fadeInEasingCurve = null;


    private IFadable _fadable = null;


    private void Reset()
    {
        _fadeOutEasingCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
        _fadeInEasingCurve = AnimationCurve.Linear(0f, -1f, 1f, 0f);
    }

    private void Awake()
    {
        _fadable = GetComponent<IFadable>();
    }

    private void Start()
    {
        _fadable.Opacity = 0f;
    }

    private IEnumerator DoFadeIn(float duration, Action onCompleted)
    {
        float startTime = Time.unscaledTime;

        while (Time.unscaledTime <= startTime + duration)
        {
            _fadable.Opacity = _fadeInEasingCurve.Evaluate(Mathf.Clamp01((Time.unscaledTime - startTime) / duration));
            yield return null;
        }

        _fadable.Opacity = 0f;

        onCompleted?.Invoke();
    }

    private IEnumerator DoFadeOut(float duration, Action onCompleted)
    {
        float startTime = Time.unscaledTime;

        while (Time.unscaledTime <= startTime + duration)
        {
            _fadable.Opacity = _fadeOutEasingCurve.Evaluate(Mathf.Clamp01((Time.unscaledTime - startTime) / duration));
            yield return null;
        }

        _fadable.Opacity = 1f;

        onCompleted?.Invoke();
    }

    /// <summary>
    /// フェードイン
    /// </summary>
    /// <param name="duration"></param>
    /// <param name="onCompleted"></param>
    /// <returns></returns>
    public Coroutine FadeIn(float duration, Action onCompleted = null)
    {
        StopAllCoroutines();
        return StartCoroutine(DoFadeIn(duration, onCompleted));
    }

    /// <summary>
    /// フェードアウト
    /// </summary>
    /// <param name="duration"></param>
    /// <param name="onCompleted"></param>
    /// <returns></returns>
    public Coroutine FadeOut(float duration, Action onCompleted = null)
    {
        StopAllCoroutines();
        return StartCoroutine(DoFadeOut(duration, onCompleted));
    }
}
