using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// リザルト表示ウインドウ
/// </summary>
public class ResultWindow : MonoBehaviour
{
    /// <summary>
    /// タイトル表示先
    /// </summary>
    [SerializeField]
    private Text _titleText = null;

    /// <summary>
    /// プレイ時間表示先
    /// </summary>
    [SerializeField]
    private Text _playTimeText = null;

    [SerializeField]
    private Graphic _highScoreGraphic = null;

    /// <summary>
    /// スコアカウンタ
    /// </summary>
    [SerializeField]
    private ScoreCounter[] _scoreCounters = null;

    /// <summary>
    /// コントロールUI
    /// </summary>
    [SerializeField]
    private Selectable[] _selectableUis = null;


    private void OnEnable()
    {
        for (int i = 0; i < _selectableUis.Length; i++)
        {
            _selectableUis[i].gameObject.SetActive(false);
        }

        for (int i = 0; i < _scoreCounters.Length; i++)
        {
            _scoreCounters[i].gameObject.SetActive(false);
        }

        _highScoreGraphic.gameObject.SetActive(false);


        StartCoroutine(DoShow(_scoreLabels, _scoreValues, 1f, _showHighScore));
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private string[] _scoreLabels = null;
    private int[] _scoreValues = null;
    private bool _showHighScore = false;

    /// <summary>
    /// 内容をセット
    /// </summary>
    public void Prepare(string title, string[] scoreLabels, int[] scoreValues, float playTime, bool showHighScore)
    {
        // タイトル表示
        _titleText.text = title;

        // プレイ時間表示
        if (_playTimeText != null)
        {
            TimeSpan playTimeSpan = new TimeSpan(0, 0, Mathf.CeilToInt(playTime));
            _playTimeText.text = playTimeSpan.ToString(@"mm\:ss");
        }

#if UNITY_EDITOR
        Debug.Assert(scoreLabels.Length == _scoreCounters.Length && scoreValues.Length == _scoreCounters.Length);
#endif

        _scoreLabels = scoreLabels;
        _scoreValues = scoreValues;
        _showHighScore = showHighScore;
    }

    private IEnumerator DoShow(string[] labels, int[] values, float interval, bool showHighScore)
    {
        // 表示
        for (int i = 0; i < _scoreCounters.Length; i++)
        {
            _scoreCounters[i].gameObject.SetActive(true);
            _scoreCounters[i].Label = labels[i];
            _scoreCounters[i].Value = values[i];

            yield return new WaitForSeconds(interval);
        }

        _highScoreGraphic.gameObject.SetActive(showHighScore);

        for (int i = 0; i < _selectableUis.Length; i++)
        {
            _selectableUis[i].gameObject.SetActive(true);
        }
    }
}
