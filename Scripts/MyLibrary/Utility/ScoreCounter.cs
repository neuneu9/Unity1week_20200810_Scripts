using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// スコア表示UI
/// </summary>
public class ScoreCounter : LabelValue
{
    /// <summary>
    /// 更新時間
    /// </summary>
    [SerializeField]
    private float _duration = 1f;

    [Header("Option")]
    /// <summary>
    /// 増減値表示先
    /// （不要ならNoneでOK）
    /// </summary>
    [SerializeField]
    private Text _deltaText = null;

    /// <summary>
    /// 更新演出中の値
    /// </summary>
    private int _updatingValue = 0;

    /// <summary>
    /// タイマ
    /// </summary>
    private float _timer = 0f;

    /// <summary>
    /// 変更開始時の値
    /// </summary>
    private int _startScore = 0;


    public override int Value
    {
        set
        {
            if (value != _value)
            {
                if (_deltaText != null)
                {
                    _deltaText.text = (value - _value).ToString("+#;-#");
                }

                _value = value;
                _timer = 0f;
                _startScore = _updatingValue;
            }
        }
        get { return _value; }
    }

    public bool IsVisible
    {
        set
        {
            if (value)
            {
                if (_deltaText != null)
                {
                    _deltaText.gameObject.SetActive(value);
                }

                _valueText.gameObject.SetActive(value);
            }
        }
    }


    private void LateUpdate()
    {
        if (_updatingValue != _value)
        {
            if (_timer < _duration)
            {
                _timer += Time.deltaTime;

                float timeRate = Mathf.Clamp01(_timer / _duration);
                _updatingValue = Mathf.FloorToInt((_value - _startScore) * timeRate + _startScore);
            }
            else
            {
                _updatingValue = _value;
            }

            _valueText.text = _updatingValue.ToString();
        }
    }


    /// <summary>
    /// 値をアニメーションさせるコルーチン
    /// </summary>
    /// <param name="startScore"></param>
    /// <param name="endScore"></param>
    /// <param name="duration"></param>
    /// <returns></returns>
    private IEnumerator DoCount(int startScore, int endScore, float duration)
    {
        // 開始時間
        float startTime = Time.time;

        // 終了時間
        float endTime = startTime + duration;

        do
        {
            // 現在の時間の割合
            float timeRate = Mathf.Clamp01((Time.time - startTime) / duration);

            // 数値を更新
            int updateValue = Mathf.FloorToInt((endScore - startScore) * timeRate + startScore);

            // テキストの更新
            _valueText.text = updateValue.ToString();

            // 1フレーム待つ
            yield return null;

        } while (Time.time < endTime);

        // 最終値
        _valueText.text = endScore.ToString();
    }
}
