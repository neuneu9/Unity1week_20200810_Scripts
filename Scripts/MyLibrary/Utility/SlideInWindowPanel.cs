using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;

/// <summary>
/// 画面外からスライドインするパネル
/// </summary>
public class SlideInWindowPanel : PanelBehaviour
{
    /// <summary>
    /// 方向タイプ種類
    /// </summary>
    private enum OuterPointType
    {
        Up,
        Down,
        Left,
        Right,
    }

    /// <summary>
    /// 対象のウインドウ
    /// </summary>
    [SerializeField]
    private RectTransform _window = null;

    /// <summary>
    /// エントリー方向
    /// </summary>
    [SerializeField]
    private OuterPointType _entryPoint = OuterPointType.Right;

    /// <summary>
    /// イグジット方向
    /// </summary>
    [SerializeField]
    private OuterPointType _exitPoint = OuterPointType.Right;

    /// <summary>
    /// イージングタイプ
    /// </summary>
    [SerializeField]
    private Ease _ease = Ease.OutQuart;

    /// <summary>
    /// イージング時間
    /// </summary>
    [SerializeField]
    private float _easeTime = 1f;

    /// <summary>
    /// 画面外に収納されるときのアンカーポジション
    /// </summary>
    private Vector2 GetOuterAnchoredPosition(OuterPointType entryPointType)
    {
        switch (entryPointType)
        {
            case OuterPointType.Up:
                return new Vector2(0f, _rectTransform.rect.height);
            case OuterPointType.Down:
                return new Vector2(0f, -_rectTransform.rect.height);
            case OuterPointType.Left:
                return new Vector2(-_rectTransform.rect.width, 0f);
            case OuterPointType.Right:
                return new Vector2(_rectTransform.rect.width, 0f);
            default:
                return Vector2.zero;
        }
    }


    /// <summary>
    /// パネルを開く動作
    /// </summary>
    protected override void OpenProcess(UnityAction onPrepared = null, UnityAction onCompleted = null)
    {
        gameObject.SetActive(true);

        onPrepared?.Invoke();

        // スライドイン
        _window.anchoredPosition = GetOuterAnchoredPosition(_entryPoint);
        _window.DOAnchorPos(Vector2.zero, _easeTime)
            .SetEase(_ease)
            .SetUpdate(true)
            .OnComplete(() =>
            {
                _canvasGroup.interactable = true;
                onCompleted?.Invoke();
                _onOpened?.Invoke();
            });
    }

    /// <summary>
    /// パネルを閉じる動作
    /// </summary>
    protected override void CloseProcess(UnityAction onPrepared = null, UnityAction onCompleted = null)
    {
        onPrepared?.Invoke();
        _canvasGroup.interactable = false;

        // スライドアウト
        _window.DOAnchorPos(GetOuterAnchoredPosition(_exitPoint), _easeTime)
            .SetEase(_ease)
            .SetUpdate(true)
            .OnComplete(() =>
            {
                onCompleted?.Invoke();
                _onClosed?.Invoke();
                gameObject.SetActive(false);
            });
    }
}
