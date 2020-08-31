using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ゲームオプション管理クラス
/// </summary>
[RequireComponent(typeof(DontDestroyOnLoadBehaviour))]
public class GameOption : MonoBehaviour
{
    /// <summary>
    /// マウス感度
    /// </summary>
    [SerializeField]
    private float _mouseSensitivity = 1f;

    /// <summary>
    /// カメラ視野角
    /// </summary>
    [SerializeField]
    private float _cameraFov = 60f;

    /// <summary>
    /// ゲームサウンド
    /// </summary>
    private GameSound _gameSound = null;

    /// <summary>
    /// BGM音量
    /// </summary>
    public float BgmVolume
    {
        get => _gameSound.BgmVolume;
        set => _gameSound.BgmVolume = value;
    }

    /// <summary>
    /// SE音量
    /// </summary>
    public float SeVolume
    {
        get => _gameSound.SeVolume;
        set => _gameSound.SeVolume = value;
    }

    /// <summary>
    /// ボイス音量
    /// </summary>
    public float VoiceVolume
    {
        get => _gameSound.VoiceVolume;
        set => _gameSound.VoiceVolume = value;
    }


    /// <summary>
    /// マウス感度
    /// </summary>
    public float MouseSensitivity
    {
        get { return _mouseSensitivity; }
        set
        {
            if (_mouseSensitivity != value)
            {
                _mouseSensitivity = value;
            }
        }
    }

    /// <summary>
    /// カメラ視野角
    /// </summary>
    public float CameraFov
    {
        get { return _cameraFov; }
        set
        {
            if (_cameraFov != value)
            {
                _cameraFov = value;
            }
        }
    }


    private void Awake()
    {
        _gameSound = FindObjectOfType<GameSound>();
    }

    private void Start()
    {
        BgmVolume = PlayerPrefs.GetFloat("BgmVolume", BgmVolume);
        SeVolume = PlayerPrefs.GetFloat("SeVolume", SeVolume);
        VoiceVolume = PlayerPrefs.GetFloat("VoiceVolume", VoiceVolume);
        MouseSensitivity = PlayerPrefs.GetFloat("MouseSensitivity", MouseSensitivity);
        CameraFov = PlayerPrefs.GetFloat("CameraFov", CameraFov);
    }

    private void OnEnable()
    {
        _optionPanel.gameObject.SetActive(false);
    }

    /// <summary>
    /// セーブ
    /// </summary>
    public void SaveAll()
    {
        PlayerPrefs.SetFloat("BgmVolume", BgmVolume);
        PlayerPrefs.SetFloat("SeVolume", SeVolume);
        PlayerPrefs.SetFloat("VoiceVolume", VoiceVolume);
        PlayerPrefs.SetFloat("MouseSensitivity", MouseSensitivity);
        PlayerPrefs.SetFloat("CameraFov", CameraFov);
    }

    /// <summary>
    /// 初期値に戻す
    /// </summary>
    public void ResetAll()
    {
        BgmVolume = 1f;
        SeVolume = 1f;
        VoiceVolume = 1f;
        MouseSensitivity = 1f;
        CameraFov = 60f;
    }

    /// <summary>
    /// オプション設定パネル
    /// </summary>
    [SerializeField]
    private PanelBehaviour _optionPanel = null;

    public void OpenPanel()
    {
        _optionPanel.Open();
    }

    public void ClosePanel()
    {
        _optionPanel.Close();
    }
}
