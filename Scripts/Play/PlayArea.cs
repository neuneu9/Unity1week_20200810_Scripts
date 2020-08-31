using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ゲームプレイ領域
/// </summary>
public class PlayArea : MonoBehaviour
{
    /// <summary>
    /// 範囲を決める矩形
    /// </summary>
    [SerializeField]
    private Rect _areaRect = new Rect(-5f, -5f, 10f, 10f);

    /// <summary>
    /// 壁の厚さ
    /// </summary>
    [SerializeField]
    private float _thickness = 1f;

    /// <summary>
    /// 壁コライダー
    /// </summary>
    [SerializeField]
    private BoxCollider2D _wallColliderLeft = null;
    [SerializeField]
    private BoxCollider2D _wallColliderRight = null;
    [SerializeField]
    private BoxCollider2D _wallColliderTop = null;
    [SerializeField]
    private BoxCollider2D _wallColliderBottom = null;

    /// <summary>
    /// 壁表示
    /// </summary>
    [SerializeField]
    private LineRenderer _lineRenderer = null;


    private void Reset()
    {
        _lineRenderer = GetComponent<LineRenderer>();
    }

    private void OnValidate()
    {
        if (_wallColliderLeft != null)
        {
            _wallColliderLeft.offset = new Vector2(_areaRect.xMin - _thickness / 2f, _areaRect.center.y);
            _wallColliderLeft.size = new Vector2(_thickness, _areaRect.height + _thickness * 2f);
        }

        if (_wallColliderRight != null)
        {
            _wallColliderRight.offset = new Vector2(_areaRect.xMax + _thickness / 2f, _areaRect.center.y);
            _wallColliderRight.size = new Vector2(_thickness, _areaRect.height + _thickness * 2f);
        }

        if (_wallColliderTop != null)
        {
            _wallColliderTop.offset = new Vector2(_areaRect.center.x, _areaRect.yMax + _thickness / 2f);
            _wallColliderTop.size = new Vector2(_areaRect.width + _thickness * 2f, _thickness);
        }

        if (_wallColliderBottom != null)
        {
            _wallColliderBottom.offset = new Vector2(_areaRect.center.x, _areaRect.yMin - _thickness / 2f);
            _wallColliderBottom.size = new Vector2(_areaRect.width + _thickness * 2f, _thickness);
        }

        // ラインレンダラで描画
        if (_lineRenderer != null)
        {
            _lineRenderer.useWorldSpace = true;
            _lineRenderer.positionCount = 4;
            _lineRenderer.loop = true;

            _lineRenderer.startWidth = _thickness;
            _lineRenderer.endWidth = _thickness;
            _lineRenderer.SetPosition(0, new Vector2(_areaRect.xMin - _thickness / 2f, _areaRect.yMin - _thickness / 2f));
            _lineRenderer.SetPosition(1, new Vector2(_areaRect.xMax + _thickness / 2f, _areaRect.yMin - _thickness / 2f));
            _lineRenderer.SetPosition(2, new Vector2(_areaRect.xMax + _thickness / 2f, _areaRect.yMax + _thickness / 2f));
            _lineRenderer.SetPosition(3, new Vector2(_areaRect.xMin - _thickness / 2f, _areaRect.yMax + _thickness / 2f));
        }
    }

    /// <summary>
    /// 矩形を設定
    /// </summary>
    /// <param name="rect"></param>
    public void SetRect(Rect rect)
    {
        _areaRect = rect;

        OnValidate();
    }

    /// <summary>
    /// 領域内のランダムな点を取得する
    /// </summary>
    /// <param name="margin">壁からのマージン</param>
    /// <returns></returns>
    public Vector2 GetRandomPositionInRect(float margin)
    {
        var x = Random.Range(_areaRect.xMin + margin, _areaRect.xMax - margin);
        var y = Random.Range(_areaRect.yMin + margin, _areaRect.yMax - margin);

        return new Vector2(x, y);
    }
}
