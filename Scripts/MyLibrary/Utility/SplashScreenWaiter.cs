using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;

/// <summary>
/// スプラッシュスクリーンが終わるまで入力を無視する
/// </summary>
public class SplashScreenWaiter : MonoBehaviour
{
    /// <summary>
    /// エディタでスプラッシュスクリーンを表示するか
    /// </summary>
    [SerializeField]
    private bool _simulateInEditor = false;


    private EventSystem _eventSystem = null;

    private void Awake()
    {
        if (Application.isEditor)
        {
            if (_simulateInEditor)
            {
                SplashScreen.Begin();
            }
        }

        _eventSystem = FindObjectOfType<EventSystem>();
        _eventSystem.enabled = false;
    }

    private IEnumerator Start()
    {
        yield return new WaitUntil(() => SplashScreen.isFinished);

        _eventSystem.enabled = true;
    }
}
