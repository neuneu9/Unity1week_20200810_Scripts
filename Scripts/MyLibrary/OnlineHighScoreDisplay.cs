using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// オンラインのデータからハイスコアを表示
/// </summary>
public class OnlineHighScoreDisplay : MonoBehaviour
{
    [SerializeField]
    private Text _text = null;

    [SerializeField]
    private string _format = "HIGH SCORE: {0}";


    private GameRanking _gameRanking = null;


    private void Reset()
    {
        _text = GetComponent<Text>();
    }

    private void Awake()
    {
        _gameRanking = FindObjectOfType<GameRanking>();
    }

    private void OnEnable()
    {
        _text.enabled = false;

        _gameRanking.LoadHighScore((x) =>
        {
            if (x != null)
            {
                (var name, var highScore) = _gameRanking.GetInformation(x);

                _text.text = string.Format(_format, highScore.TextForDisplay);
                _text.enabled = true;
            }
            else
            {
                _text.enabled = false;
            }
        });
    }
}
