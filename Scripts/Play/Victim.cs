using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

/// <summary>
/// 切断されて増えていくオブジェクト
/// </summary>
public class Victim : MonoBehaviour
{
    /// <summary>
    /// ベース移動速度
    /// </summary>
    [SerializeField]
    private float _defaultSpeed = 5f;

    /// <summary>
    /// 移動速度ブレ幅率
    /// </summary>
    [SerializeField]
    private float _speedSpreadRate = 0.3f;

    /// <summary>
    /// 角度ブレ幅
    /// </summary>
    //[SerializeField]
    private float _angleSpread = 60f;

    /// <summary>
    /// スプライト
    /// </summary>
    [SerializeField]
    private SpriteRenderer _spriteRenderer = null;

    private Rigidbody2D _rigidbody2d = null;
    private Tween _separateTween = null;

    private void Awake()
    {
        _rigidbody2d = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (_separateTween == null || !_separateTween.IsActive())
        {
            // 進行方向に向くように回転
            Vector3 up = Vector3.Cross(Vector3.forward, _rigidbody2d.velocity);
            _spriteRenderer.transform.rotation = Quaternion.LookRotation(Vector3.forward, up);
        }
    }

    /// <summary>
    /// 移動速度を設定
    /// </summary>
    /// <param name="speed"></param>
    public void SetSpeed(float speed)
    {
        _defaultSpeed = speed;

        // Rigidbody2Dの速度も更新
        var velocity = _rigidbody2d.velocity.normalized * (_defaultSpeed + _defaultSpeed * Random.Range(-_speedSpreadRate, _speedSpreadRate));
        _rigidbody2d.velocity = velocity;
    }

    /// <summary>
    /// 移動開始
    /// </summary>
    /// <param name="velocity"></param>
    public void StartMove(Vector2 velocity)
    {
        _rigidbody2d.velocity = velocity;
    }

    /// <summary>
    /// 被切断
    /// </summary>
    /// <param name="enterPoint"></param>
    /// <param name="exitPoint"></param>
    public void Sliced(Vector2 enterPoint, Vector2 exitPoint, float duration = 0.3f)
    {
        Vector2 sliceVector = exitPoint - enterPoint;
        Vector2 sliceRight = new Vector2(sliceVector.y, -sliceVector.x).normalized;

        Vector2 basePosition = transform.position;

        // 自分
        float speed = _defaultSpeed + _defaultSpeed * Random.Range(-_speedSpreadRate, _speedSpreadRate);
        Vector2 direction = Quaternion.AngleAxis(Random.Range(-_angleSpread, _angleSpread), Vector3.forward) * sliceRight;
        _separateTween = _rigidbody2d.DOMove(basePosition + sliceRight, duration).OnComplete(() => StartMove(direction * speed));

        // 自分のコピーを作成
        // →反対方向に移動開始
        float cloneSpeed = _defaultSpeed + _defaultSpeed * Random.Range(-_speedSpreadRate, _speedSpreadRate);
        Vector2 cloneDirection = Quaternion.AngleAxis(Random.Range(-_angleSpread, _angleSpread), Vector3.forward) * -sliceRight;
        var clone = Instantiate(this, transform.position, transform.rotation);
        clone._separateTween = clone._rigidbody2d.DOMove(basePosition - sliceRight, duration).OnComplete(() => clone.StartMove(cloneDirection * cloneSpeed));
    }
}
