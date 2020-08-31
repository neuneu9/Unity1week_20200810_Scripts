using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// ゲームシーンBGM
/// </summary>
public class SceneBgm : MonoBehaviour
{
    /// <summary>
    /// BGM名
    /// </summary>
    [SerializeField]
    private string _bgmTitle = string.Empty;


    private GameSound _gameSound = null;
    private GameScene _gameScene = null;

    private UnityAction _fadeOut = null;

    private void OnEnable()
    {
        _fadeOut = () => _gameSound.StopBgm(0.8f);
        _gameScene.OnSceneChangeStarted.AddListener(_fadeOut);
    }

    private void OnDisable()
    {
        _gameScene.OnSceneChangeStarted.RemoveListener(_fadeOut);
    }

    private void Awake()
    {
        _gameSound = FindObjectOfType<GameSound>();
        _gameScene = FindObjectOfType<GameScene>();
    }

    private void Start()
    {
        // BGMを鳴らす
        if (!string.IsNullOrEmpty(_bgmTitle))
        {
            _gameSound.ChangeBgm(_bgmTitle, enableReplay: true);
        }
        // BGMを止める
        else
        {
            _gameSound.StopBgm();
        }
    }
}
