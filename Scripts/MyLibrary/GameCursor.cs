using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ゲームカーソル設定
/// </summary>
public class GameCursor : MonoBehaviour
{
    /// <summary>
    /// 表示フラグ
    /// </summary>
    [SerializeField]
    private bool _visible = true;

    /// <summary>
    /// ロック状態
    /// </summary>
    [SerializeField]
    private CursorLockMode _lockState = CursorLockMode.None;

    /// <summary>
    /// カーソルテクスチャ
    /// </summary>
    [SerializeField]
    private Texture2D _cursorTexture = null;

    /// <summary>
    /// カーソルのホットスポット
    /// （左上が原点）
    /// </summary>
    [SerializeField]
    private Vector2 _hotspot = Vector2.zero;

    private void OnEnable()
    {
        Cursor.visible = _visible;
        Cursor.lockState = _lockState;

        // カーソル画像を変更
        if (_cursorTexture != null)
        {
            Cursor.SetCursor(_cursorTexture, _hotspot, CursorMode.ForceSoftware);
        }
    }

    private void OnDisable()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }

    private void Awake()
    {
        Application.focusChanged += ApplicationFocusChangedAction;
    }

    private void OnDestroy()
    {
        Application.focusChanged -= ApplicationFocusChangedAction;
    }

    /// <summary>
    /// アプリケーションのフォーカスが変更されたときの処理
    /// OnEnable/OnDisableのタイミングでアプリケーションからフォーカスが外れていると変更が適用されないため用意
    /// </summary>
    /// <param name="focus"></param>
    private void ApplicationFocusChangedAction(bool focus)
    {
        if (focus)
        {
            if (enabled)
            {
                OnEnable();
            }
            else
            {
                OnDisable();
            }
        }
    }
}
