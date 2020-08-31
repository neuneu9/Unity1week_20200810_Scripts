using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// インゲームマネージャ
/// </summary>
public class PlayScene : MonoBehaviour
{
    /// <summary>
    /// プレイヤー操作
    /// </summary>
    [SerializeField]
    private Slicer _slicer = null;

    /// <summary>
    /// ステージ
    /// </summary>
    [SerializeField]
    private Stage _stage = null;

    /// <summary>
    /// ボーナス選択パネル
    /// </summary>
    [SerializeField]
    private PanelBehaviour _bonusPanel = null;

    /// <summary>
    /// リザルトパネル
    /// </summary>
    [SerializeField]
    private PanelBehaviour _resultPanel = null;

    /// <summary>
    /// スコア表示先
    /// </summary>
    [SerializeField]
    private Text _scoreCounter = null;

    /// <summary>
    /// 切断可能数
    /// </summary>
    [SerializeField]
    private int _sliceCountMax = 5;

    /// <summary>
    /// 残り切断可能数表示先
    /// </summary>
    [SerializeField]
    private Text _sliceCountRemainText = null;

    /// <summary>
    /// カーソルコントロール
    /// </summary>
    [SerializeField]
    private GameCursor _gameCursor = null;

    /// <summary>
    /// 得点
    /// </summary>
    private int _totalScore = 0;

    /// <summary>
    /// ゲーム開始時の時間
    /// </summary>
    private float _startTime = 0f;

    /// <summary>
    /// 残り切断可能数
    /// </summary>
    private int _sliceCountRemain = 0;


    /// <summary>
    /// ゲームプレイ時間
    /// </summary>
    public float PlayTime
    {
        get { return (Time.time - _startTime); }
    }


    private void OnEnable()
    {
        _slicer.OnSliceStarted.AddListener(() =>
        {
            _sliceCountRemain--;
            _sliceCountRemainText.text = _sliceCountRemain.ToString();

            Time.timeScale = 0f;
            _slicer.EnableInput = false;
        });


        _slicer.OnSliceEnded.AddListener(() =>
        {
            Time.timeScale = 1f;

            StartCoroutine(WaitAndGoToNextPhase());
        });

        _bonusPanel.OnClosed.AddListener(() =>
        {
            Time.timeScale = 1f;

            _slicer.EnableInput = true;
        });
    }

    private void OnDisable()
    {
        // タイムスケールは抜けるときに必ず元に戻す
        Time.timeScale = 1f;
    }

    private void Start()
    {
        _startTime = Time.time;

        _sliceCountRemain = _sliceCountMax;
        _sliceCountRemainText.text = _sliceCountRemain.ToString();

        _slicer.EnableInput = true;

        _stage.AddVictim();
    }

    private void LateUpdate()
    {
        // 毎周期タグ検索してるけど許して
        var planarias = GameObject.FindGameObjectsWithTag("Planaria");

        if (_totalScore != planarias.Length)
        {
            _totalScore = planarias.Length;
            _scoreCounter.text = _totalScore.ToString();
        }
    }

    /// <summary>
    /// フェーズ進行
    /// </summary>
    /// <returns></returns>
    private IEnumerator WaitAndGoToNextPhase()
    {
        yield return new WaitForSeconds(1.2f);

        // まだ残り回数あり
        if (_sliceCountRemain > 0)
        {
            Time.timeScale = 0f;

            _bonusPanel.Open();
        }
        // 残り回数が尽きた
        else
        {
            // ハイスコアチェック
            int highScore = PlayerPrefs.GetInt("HighScore", 0);
            bool isHighScore = _totalScore > highScore;
            if (isHighScore)
            {
                PlayerPrefs.SetInt("HighScore", _totalScore);
            }

            UnityroomTweetButton.TweetText = string.Format("プラナリアを{0}匹まで増やすことに成功したよ！", _totalScore);

            var resultWindow = _resultPanel.GetComponentInChildren<ResultWindow>(true);
            resultWindow.Prepare("結果", new string[] { "最終生息数" }, new int[] { _totalScore }, PlayTime, isHighScore);
            _resultPanel.Open(onCompleted: () =>
            {
                _gameCursor.enabled = false;
            });
        }
    }


    private GameRanking _gameRanking = null;

    /// <summary>
    /// ランキング登録
    /// </summary>
    public void OpenRankingPanel()
    {
#if UNITY_1WEEK
        _gameRanking = FindObjectOfType<GameRanking>();
        _gameRanking.SaveScore(_totalScore);
        _gameRanking.OpenRanking();
#endif
    }
}
