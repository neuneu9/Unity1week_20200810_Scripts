using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

/// <summary>
/// ゲームサウンド管理クラス
/// </summary>
[RequireComponent(typeof(DontDestroyOnLoadBehaviour))]
public class GameSound : MonoBehaviour
{
    /// <summary>
    /// オーディオミキサー
    /// </summary>
    [SerializeField]
    private AudioMixer _mixer = null;

    /// <summary>
    /// BGM音量
    /// </summary>
    [SerializeField, Range(0f, 1f)]
    private float _bgmVolume = 1f;

    /// <summary>
    /// SE音量
    /// </summary>
    [SerializeField, Range(0f, 1f)]
    private float _seVolume = 1f;

    /// <summary>
    /// ボイス音量
    /// </summary>
    [SerializeField, Range(0f, 1f)]
    private float _voiceVolume = 1f;


    /// <summary>
    /// デシベル変換
    /// 0, 1の音圧→-80, 0のデシベル
    /// </summary>
    /// <param name="pa01"></param>
    /// <returns></returns>
    private static float Pa01ToDb(float pa01)
    {
        pa01 = Mathf.Clamp(pa01, 0.0001f, 1f);
        return 20f * Mathf.Log10(pa01);
    }

    /// <summary>
    /// BGM音量
    /// </summary>
    public float BgmVolume
    {
        get { return _bgmVolume; }
        set
        {
            if (_bgmVolume != value)
            {
                _bgmVolume = value;
                _mixer.SetFloat("BgmVolume", Pa01ToDb(value));
            }
        }
    }

    /// <summary>
    /// SE音量
    /// </summary>
    public float SeVolume
    {
        get { return _seVolume; }
        set
        {
            if (_seVolume != value)
            {
                _seVolume = value;
                _mixer.SetFloat("SeVolume", Pa01ToDb(value));
            }
        }
    }

    /// <summary>
    /// ボイス音量
    /// </summary>
    public float VoiceVolume
    {
        get { return _voiceVolume; }
        set
        {
            if (_voiceVolume != value)
            {
                _voiceVolume = value;
                _mixer.SetFloat("VoiceVolume", Pa01ToDb(value));
            }
        }
    }


    /// <summary>
    /// BGMプレイヤー
    /// </summary>
    [SerializeField]
    private BgmPlayer _bgmPlayer = null;

    /// <summary>
    /// SEプレイヤー
    /// </summary>
    [SerializeField]
    private AudioSource _sePlayer = null;

    /// <summary>
    /// SEを再生
    /// </summary>
    /// <param name="clip"></param>
    public void PlaySe(AudioClip clip)
    {
        if (clip != null)
        {
            _sePlayer.PlayOneShot(clip);
        }
    }

    /// <summary>
    /// BGMを変更
    /// </summary>
    /// <param name="title"></param>
    /// <param name="enableReplay"></param>
    public void ChangeBgm(string title, float volume = 1f, float duration = 0f, bool enableReplay = false)
    {
        _bgmPlayer.Play(title, volume, duration, enableReplay);
    }

    /// <summary>
    /// BGMを停止
    /// </summary>
    public void StopBgm(float duration = 0f)
    {
        _bgmPlayer.Stop(duration);
    }
}
