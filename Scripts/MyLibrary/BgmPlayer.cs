using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// BGMプレイヤークラス
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class BgmPlayer : MonoBehaviour
{
    /// <summary>
    /// 登録BGM
    /// </summary>
    [SerializeField]
    private BgmTable _bgmTable = null;

    /// <summary>
    /// 現在再生中のBGMタイトル
    /// </summary>
    [SerializeField]
    private string _currentTitle = string.Empty;

    /// <summary>
    /// BGM再生チャンネル
    /// </summary>
    private AudioSource _audioSource = null;

    /// <summary>
    /// タイトルでアクセスできるようにするためのキャッシュ
    /// </summary>
    private Dictionary<string, BgmData> _bgmCache = new Dictionary<string, BgmData>();

    /// <summary>
    /// BGM変更時のイベント
    /// </summary>
    private UnityEvent<string, AudioClip> _onBgmChanged = new UnityEventOnBgmChanged();
    private class UnityEventOnBgmChanged : UnityEvent<string, AudioClip> { }

    /// <summary>
    /// フェードコルーチン
    /// </summary>
    private Coroutine _fadeCoroutine = null;

    /// <summary>
    /// ポーズ前の音量
    /// </summary>
    private float _volumeOnPaused = 1f;


    /// <summary>
    /// 現在再生中のBGM
    /// </summary>
    public BgmData CurrentBgm
    {
        get
        {
            if (_bgmCache.TryGetValue(_currentTitle, out BgmData data))
            {
                return data;
            }
            else
            {
                return null;
            }
        }
    }

    /// <summary>
    /// BGM変更時のイベント
    /// </summary>
    public UnityEvent<string, AudioClip> OnBgmChanged
    {
        get { return _onBgmChanged; }
    }



    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();

        if (_bgmTable != null)
        {
            _bgmCache = _bgmTable.Bgms.ToDictionary(x => x.Title);
        }
    }

    /// <summary>
    /// BGMを鳴らす
    /// </summary>
    /// <param name="title"></param>
    /// <param name="volume"></param>
    /// <param name="enableReplay"></param>
    public void Play(string title, float volume = 1f, float duration = 0f, bool enableReplay = false)
    {
        if (!enableReplay)
        {
            // 再生BGMが同じなら何もしない
            if (_currentTitle == title)
            {
                return;
            }
        }

        if (_fadeCoroutine != null)
        {
            StopCoroutine(_fadeCoroutine);
        }

        if (duration > 0f)
        {
            _fadeCoroutine = StartCoroutine(FadeIn(volume, duration, () => PlayBgm(title, volume)));
        }
        else
        {
            PlayBgm(title, volume);
        }
    }

    /// <summary>
    /// BGMを停止
    /// </summary>
    public void Stop(float duration = 0f)
    {
        if (_fadeCoroutine != null)
        {
            StopCoroutine(_fadeCoroutine);
        }

        if (duration > 0f)
        {
            _fadeCoroutine = StartCoroutine(FadeOut(duration, () => PlayBgm(string.Empty, 0f)));
        }
        else
        {
            PlayBgm(string.Empty, 0f);
        }
    }

    public void Pause(float duration = 0f)
    {
        if (_fadeCoroutine != null)
        {
            StopCoroutine(_fadeCoroutine);
        }

        _volumeOnPaused = _audioSource.volume;

        if (duration > 0f)
        {
            _fadeCoroutine = StartCoroutine(FadeOut(duration, () => _audioSource.Pause()));
        }
        else
        {
            _audioSource.Pause();
        }
    }

    public void Resume(float duration = 0f)
    {
        if (_fadeCoroutine != null)
        {
            StopCoroutine(_fadeCoroutine);
        }

        if (duration > 0f)
        {
            _fadeCoroutine = StartCoroutine(FadeIn(_volumeOnPaused, duration, () => _audioSource.UnPause()));
        }
        else
        {
            _audioSource.UnPause();
        }
    }

    /// <summary>
    /// フェードイン
    /// </summary>
    /// <returns></returns>
    private IEnumerator FadeIn(float volume, float duration, UnityAction onPrepared)
    {
        onPrepared?.Invoke();

        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.unscaledDeltaTime;

            _audioSource.volume = Mathf.Clamp01(timer / duration) * volume;

            yield return null;
        }

        _audioSource.volume = volume;
    }

    /// <summary>
    /// フェードアウト
    /// </summary>
    /// <returns></returns>
    private IEnumerator FadeOut(float duration, UnityAction onCompleted)
    {
        float timer = 0f;
        float volume = _audioSource.volume;

        while (timer < duration)
        {
            timer += Time.unscaledDeltaTime;

            _audioSource.volume = (1f - Mathf.Clamp01(timer / duration)) * volume;

            yield return null;
        }

        onCompleted?.Invoke();
    }


    /// <summary>
    /// BGMを再生
    /// すでになっているBGMは終了
    /// </summary>
    /// <param name="clip"></param>
    private void PlayBgm(string title, float volume)
    {
        _currentTitle = title;

        if (_audioSource.isPlaying)
        {
            _audioSource.Stop();
        }

        if (!string.IsNullOrEmpty(title))
        {
            if (_bgmCache.TryGetValue(title, out BgmData data))
            {
                _audioSource.clip = data.Clip;
                _audioSource.volume = volume;
                _audioSource.Play();
            }
            else
            {
                Debug.LogWarning(string.Format("BGMデータ[{0}]が見つかりません。", title));
            }
        }
        else
        {
            _audioSource.clip = null;
            _audioSource.volume = volume;
        }

        _onBgmChanged?.Invoke(title, _audioSource.clip);
    }
}
