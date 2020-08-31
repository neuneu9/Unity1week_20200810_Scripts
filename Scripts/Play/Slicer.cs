using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 切断刃（プレイヤー操作キャラクター）
/// </summary>
public class Slicer : MonoBehaviour
{
    /// <summary>
    /// 刃判定コライダー
    /// </summary>
    [SerializeField]
    private List<CapsuleCollider2D> _bladeCollider2ds = null;

    /// <summary>
    /// 刃複数時の配置間隔
    /// </summary>
    [SerializeField]
    private float _bladeIntervel = 1f;

    /// <summary>
    /// 刃を振るエフェクトプレハブ
    /// </summary>
    [SerializeField]
    private GameObject _sweepEffectPrefab = null;

    /// <summary>
    /// 刃を振るSE
    /// </summary>
    [SerializeField]
    private AudioClip _sweepSe = null;

    /// <summary>
    /// 切断エフェクトプレハブ
    /// </summary>
    [SerializeField]
    private GameObject _sliceEffectPrefab = null;

    /// <summary>
    /// 切断エフェクトSE
    /// </summary>
    [SerializeField]
    private AudioClip _sliceSe = null;


    /// <summary>
    /// OverlapCollider結果格納バッファ
    /// </summary>
    private List<Collider2D> _colliderBuffer = new List<Collider2D>();

    private AudioSource _audioSource = null;

    private UnityEvent _onSliceStarted = new UnityEvent();
    private UnityEvent _onSliceEnded = new UnityEvent();

    /// <summary>
    /// 切断開始前のイベント
    /// </summary>
    public UnityEvent OnSliceStarted => _onSliceStarted;

    /// <summary>
    /// 切断終了後のイベント
    /// （1つも切断できなくても呼ばれる）
    /// </summary>
    public UnityEvent OnSliceEnded => _onSliceEnded;


    private void OnDrawGizmosSelected()
    {
        foreach (var item in _bladeCollider2ds)
        {
            GetEdgePositions(out Vector2 startPosition, out Vector2 endPosition, item);

            Gizmos.color = Color.red;
            Gizmos.DrawLine(startPosition, endPosition);
        }
    }

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }


    /// <summary>
    /// 刃を追加
    /// </summary>
    public void AddBlade()
    {
        // コピーを作成
        var clone = Instantiate(_bladeCollider2ds[0], transform);
        _bladeCollider2ds.Add(clone);

        // 整列
        float width = (_bladeCollider2ds.Count - 1) * _bladeIntervel;
        for (int i = 0; i < _bladeCollider2ds.Count; i++)
        {
            _bladeCollider2ds[i].transform.localPosition = new Vector2(i * _bladeIntervel - width / 2f, 0f);
        }
    }

    /// <summary>
    /// 刃の始点と終点を取得
    /// </summary>
    /// <param name="startPosition"></param>
    /// <param name="endPosition"></param>
    /// <param name="capsuleCollider2d"></param>
    private static void GetEdgePositions(out Vector2 startPosition, out Vector2 endPosition, CapsuleCollider2D capsuleCollider2d)
    {
        var center = (Vector2)capsuleCollider2d.transform.position + capsuleCollider2d.offset;
        var length = capsuleCollider2d.size.y;

        startPosition = center + (Vector2)(capsuleCollider2d.transform.rotation * new Vector2(0f, capsuleCollider2d.size.y / 2f));
        endPosition = center + (Vector2)(capsuleCollider2d.transform.rotation * new Vector2(0f, -capsuleCollider2d.size.y / 2f));
    }

    /// <summary>
    /// 切断開始
    /// </summary>
    private void Slice()
    {
        StartCoroutine(DoSlice());
    }

    private IEnumerator DoSlice()
    {
        _onSliceStarted?.Invoke();

        var intervalWait = new WaitForSecondsRealtime(0.1f);
        foreach (var item in _bladeCollider2ds)
        {
            GetEdgePositions(out Vector2 startPosition, out Vector2 endPosition, item);

            // 振るエフェクト生成
            Instantiate(_sweepEffectPrefab, item.transform.position, item.transform.rotation);

            _audioSource.PlayOneShot(_sweepSe);

            yield return new WaitForSecondsRealtime(0.4f);

            // 所定間隔でエフェクトを生成しつつ分断
            int count = item.OverlapCollider(default, _colliderBuffer);
            for (int i = 0; i < count; i++)
            {
                var victim = _colliderBuffer[i].GetComponent<Victim>();
                if (victim != null)
                {
                    // エフェクト生成
                    var sliceEffect = Instantiate(_sliceEffectPrefab, victim.transform.position, Quaternion.identity);

                    _audioSource.PlayOneShot(_sliceSe);

                    yield return intervalWait;

                    victim.Sliced(startPosition, endPosition);
                }
            }
        }

        yield return new WaitForSecondsRealtime(0.2f);

        _onSliceEnded?.Invoke();
    }


    #region Input
    /// <summary>
    /// ダブルクリックと判定する時間
    /// </summary>
    //[SerializeField]
    private float _doubleClickTime = 0.3f;

    /// <summary>
    /// 刃の回転スピード
    /// </summary>
    [SerializeField]
    private float _rotateSpeed = 90f;


    private float _lastClickTime = 0f;
    private bool _isDoubleClicked = false;
    private bool _enableInput = false;

    /// <summary>
    /// 入力を受け付けるか
    /// </summary>
    public bool EnableInput
    {
        get => _enableInput;
        set
        {
            _enableInput = value;

            // レンダラのOFF
            foreach (var item in _bladeCollider2ds)
            {
                item.GetComponent<LineRenderer>().enabled = _enableInput;
            }

            if (_enableInput)
            {
                // 初期位置の修正
                Update();
            }
        }
    }


    private void Update()
    {
        if (_enableInput)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Slice();
            }

            if (Input.GetMouseButtonDown(1))
            {
                // ダブルクリック判定
                if (Time.unscaledTime - _lastClickTime < _doubleClickTime)
                {
                    _isDoubleClicked = true;
                }
                else
                {
                    _isDoubleClicked = false;
                    _lastClickTime = Time.unscaledTime;
                }
            }

            if (Input.GetMouseButton(1))
            {
                // シングルかダブルかが未定なうちは何もしない
                if (Time.unscaledTime - _lastClickTime < _doubleClickTime)
                {
                    // NOP
                }
                else
                {
                    float sign = _isDoubleClicked ? 1f : -1f;
                    transform.Rotate(Vector3.forward, sign * _rotateSpeed * Time.deltaTime, Space.Self);
                }
            }

            // 位置
            var position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            position.z = 0f;
            transform.position = position;
        }
    }
    #endregion
}
