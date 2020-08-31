using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// シーン遷移ボタン
/// </summary>
[RequireComponent(typeof(Button))]
public class SceneButton : MonoBehaviour
{
    /// <summary>
    /// 遷移するシーン
    /// （何も入れなければ現在のシーンリロード）
    /// </summary>
    [SerializeField]
    private string _sceneName = string.Empty;


    private Button _button = null;

    /// <summary>
    /// シーン管理クラス
    /// </summary>
    private GameScene _gameScene = null;

    /// <summary>
    /// ラムダ式登録用
    /// </summary>
    private UnityAction _onClickAction = null;

    private void Awake()
    {
        _button = GetComponent<Button>();
        _gameScene = FindObjectOfType<GameScene>();
    }

    private void OnEnable()
    {
        if (string.IsNullOrEmpty(_sceneName))
        {
            _onClickAction = () => _gameScene.Reload();
        }
        else
        {
            _onClickAction = () => _gameScene.GoTo(_sceneName);
        }
        _button.onClick.AddListener(_onClickAction);
    }

    private void OnDisable()
    {
        _button.onClick.RemoveListener(_onClickAction);
        _onClickAction = null;
    }
}
