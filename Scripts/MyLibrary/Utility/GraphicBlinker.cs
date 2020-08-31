using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// UIグラフィックを点滅させる
/// </summary>
[RequireComponent(typeof(CanvasGroup))]
public class GraphicBlinker : MonoBehaviour
{
    /// <summary>
    /// アルファ値推移カーブ
    /// </summary>
    [SerializeField]
    private AnimationCurve _alphaCurve = null;


    private CanvasGroup _canvasGroup = null;


    private void Reset()
    {
        Keyframe[] keys = new Keyframe[]
        {
            new Keyframe(0f, 1f),
            new Keyframe(1f, 0.5f),
            new Keyframe(2f, 1f)
        };

        _alphaCurve = new AnimationCurve(keys);
    }

    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
    }

    private void OnEnable()
    {
        StartCoroutine(DoBlink());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private IEnumerator DoBlink()
    {
        _canvasGroup.alpha = _alphaCurve.keys[0].value;

        float duration = _alphaCurve.keys[_alphaCurve.length - 1].time;
        float startTime = Time.time;

        while (true)
        {
            float alphaRate = _alphaCurve.Evaluate(Mathf.Repeat(Time.time - startTime, duration));

            _canvasGroup.alpha = alphaRate;

            yield return null;
        }
    }
}
