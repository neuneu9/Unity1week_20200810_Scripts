using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// ゲームシーン管理クラス
/// </summary>
[RequireComponent(typeof(DontDestroyOnLoadBehaviour))]
public class GameScene : MonoBehaviour
{
    /// <summary>
    /// シーン遷移カバー
    /// </summary>
    [SerializeField]
    private TransitionFader _fade = null;

    /// <summary>
    /// フェード時間
    /// </summary>
    [SerializeField]
    private float _fadeDuration = 1f;

    /// <summary>
    /// ローディングウインドウ
    /// </summary>
    [SerializeField]
    private RectTransform _loadingWindow = null;

    /// <summary>
    /// ローディングプログレスバー
    /// </summary>
    [SerializeField]
    private Image _loadingProgress = null;

    /// <summary>
    /// シーン遷移開始時のイベント
    /// </summary>
    private UnityEvent _onSceneChangeStarted = new UnityEvent();


    public UnityEvent OnSceneChangeStarted => _onSceneChangeStarted;


    private void OnEnable()
    {
        _loadingWindow.gameObject.SetActive(false);
    }


    /// <summary>
    /// 現在のシーンを再読み込み
    /// </summary>
    public void Reload()
    {
        GoTo(SceneManager.GetActiveScene().name);
    }

    /// <summary>
    /// シーン遷移
    /// </summary>
    /// <param name="sceneName"></param>
    /// <param name="autoFadeIn">遷移が完了したら自動でフェードを解除するか</param>
    /// <param name="showProgress">ローディングプログレスバーを表示するか</param>
    public void GoTo(string sceneName, bool autoFadeIn = true, bool showProgress = true)
    {
        _onSceneChangeStarted?.Invoke();

        if (_fade != null)
        {
            _fade.FadeOut(_fadeDuration, () =>
            {
                var asyncOperation = SceneManager.LoadSceneAsync(sceneName);

                if (showProgress)
                {
                    StartCoroutine(DoShowLoadingProgress(asyncOperation, () => { if (autoFadeIn) { _fade.FadeIn(_fadeDuration); } }));
                }
                else
                {
                    asyncOperation.completed += (x) => { if (autoFadeIn) { _fade.FadeIn(_fadeDuration); } };
                }
            });
        }
        else
        {
            SceneManager.LoadScene(sceneName);
        }
    }

    /// <summary>
    /// プログレスバーを表示
    /// ref. https://docs.unity3d.com/ja/current/ScriptReference/AsyncOperation-progress.html
    /// </summary>
    /// <param name="asyncOperation"></param>
    /// <param name="onCompleted"></param>
    /// <returns></returns>
    private IEnumerator DoShowLoadingProgress(AsyncOperation asyncOperation, Action onCompleted)
    {
        _loadingWindow.gameObject.SetActive(true);

        asyncOperation.allowSceneActivation = false;

        while (!asyncOperation.isDone)
        {
            _loadingProgress.fillAmount = Mathf.Clamp01(asyncOperation.progress / 0.9f);

            if (asyncOperation.progress >= 0.9f)
            {
                asyncOperation.allowSceneActivation = true;
            }

            yield return null;
        }

        _loadingProgress.fillAmount = 1f;

        // 読み込みが終わったことを示すため少しだけ待たせる
        yield return new WaitForSecondsRealtime(0.2f);

        _loadingWindow.gameObject.SetActive(false);

        onCompleted?.Invoke();
    }
}
