using naichilab;
using NCMB;
using NCMB.Extensions;
using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// ランキング情報の格納
/// naichilabさんのRankingLoaderのカスタマイズ
/// </summary>
public class GameRanking : MonoBehaviour
{
    /// <summary>
    /// NCMBSettingsのプレハブ
    /// </summary>
    [SerializeField]
    private NCMBSettings _ncmbSettingsPrefab = null;

    /// <summary>
    /// リーダーボード一覧
    /// </summary>
    [SerializeField]
    private RankingBoards _rankingBoards = null;

    /// <summary>
    /// 表示対象のボードのインデックス
    /// </summary>
    [SerializeField]
    private int _boardIndex = 0;

    /// <summary>
    /// ランキングビューア
    /// </summary>
    [SerializeField]
    private SlideInWindowPanel _rankingPanel = null;

    /// <summary>
    /// 現在のランキングボード
    /// </summary>
    public RankingInfo CurrentRanking => _rankingBoards.GetRankingInfo(_boardIndex);

    /// <summary>
    /// 直前のプレイで獲得したスコア
    /// </summary>
    private IScore _latestScore = null;

    public IScore LatestScore => _latestScore;

    /// <summary>
    /// ハイスコアのキャッシュ
    /// </summary>
    private IScore _highScore = null;


    private void Awake()
    {
        _rankingPanel.gameObject.SetActive(false);

        var ncmbSettings = Instantiate(_ncmbSettingsPrefab);
        // ※NCMBSettingsはオブジェクト名が"NCMBSettings"でないと正しく動作しないので書き換える
        //   念のため相手コードの側はいじらないことにする
        ncmbSettings.name = "NCMBSettings";
    }

    private void Start()
    {
        // Class名重複をチェック
        _rankingBoards.CheckDuplicateClassName();
    }

    private void OnEnable()
    {
        Debug.Log(BoardIdPlayerPrefsKey + ": " + NcmbObjectId);
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }


    public void OpenRanking()
    {
        _rankingPanel.Open();
    }

    public void CloseRanking()
    {
        _rankingPanel.Close();
    }

    // ロード処理関連
    #region Loader
    private const string ObjectId = "objectId";
    private const string ColumnScore = "score";
    private const string ColumnName = "name";

    /// <summary>
    /// ランキングボードで読み込む数
    /// </summary>
    [SerializeField]
    private int _rankingLimit = 50;


    /// <summary>
    /// ObjectIDのPlayerPrefsキー
    /// </summary>
    private string BoardIdPlayerPrefsKey
    {
        get { return string.Format("{0}_{1}_{2}", "board", CurrentRanking.ClassName, ObjectId); }
    }

    /// <summary>
    /// ローカルに保存しておくObjectID
    /// </summary>
    private string NcmbObjectId
    {
        get { return PlayerPrefs.GetString(BoardIdPlayerPrefsKey, null); }
        set { PlayerPrefs.SetString(BoardIdPlayerPrefsKey, value); }
    }

    /// <summary>
    /// ハイスコアを更新しているか
    /// （＝直近のスコアを送信する意味があるか）
    /// オンライン上のハイスコアを取得する前にアクセスする場合は注意
    /// オンライン上のハイスコアがない＝更新した扱いで判断するので
    /// </summary>
    public bool IsBetteredHighScore
    {
        get
        {
            // 直近のスコアがある
            if (_latestScore != null)
            {
                if (_highScore == null)
                {
                    return true;
                }
                else
                {
                    // 既存のハイスコアを更新しているか調べる
                    if (CurrentRanking.Order == ScoreOrder.OrderByAscending)
                    {
                        // 数値が低い方が高スコア
                        return (_latestScore.Value < _highScore.Value);
                    }
                    else
                    {
                        // 数値が高い方が高スコア
                        return (_highScore.Value < _latestScore.Value);
                    }
                }
            }
            else
            {
                return false;
            }
        }
    }

    /// <summary>
    /// NCMBObjectから名前とスコアを取得
    /// </summary>
    /// <param name="ncmbObject"></param>
    /// <returns></returns>
    public (string, IScore) GetInformation(NCMBObject ncmbObject)
    {
        var name = ncmbObject[ColumnName].ToString();
        var score = CurrentRanking.BuildScore(ncmbObject[ColumnScore].ToString());

        return (name, score);
    }

    /// <summary>
    /// NCMBObjectがユーザーのものか判定
    /// </summary>
    /// <param name="ncmbObject"></param>
    /// <returns></returns>
    public bool IsOwned(NCMBObject ncmbObject)
    {
        return (ncmbObject.ObjectId == NcmbObjectId);
    }

    /// <summary>
    /// 時間型スコアの格納
    /// </summary>
    /// <param name="time"></param>
    public void SaveScore(TimeSpan time)
    {
        var board = _rankingBoards.GetRankingInfo(_boardIndex);
        var timeScore = new TimeScore(time, board.CustomFormat);

        if (board.Type != timeScore.Type)
        {
            throw new ArgumentException("スコアの型が違います。");
        }

        _latestScore = timeScore;
    }

    /// <summary>
    /// 数値型スコアの格納
    /// </summary>
    /// <param name="score"></param>
    public void SaveScore(double score)
    {
        var board = _rankingBoards.GetRankingInfo(_boardIndex);
        var numberScore = new NumberScore(score, board.CustomFormat);

        if (board.Type != numberScore.Type)
        {
            throw new ArgumentException("スコアの型が違います。");
        }

        _latestScore = numberScore;
    }

    /// <summary>
    /// ランキングボードを取得する（非同期）
    /// </summary>
    /// <param name="onCompleted"></param>
    public void LoadRankingBoard(UnityAction<YieldableNcmbQuery<NCMBObject>> onCompleted)
    {
        StartCoroutine(DoLoadRankingBoard(CurrentRanking, NcmbObjectId, onCompleted));
    }

    /// <summary>
    /// 直近のスコアを送信する（非同期）
    /// </summary>
    /// <param name="userName"></param>
    /// <param name="onCompleted"></param>
    public void SendScore(string userName, UnityAction<NCMBObject> onCompleted)
    {
        IScore score = _latestScore;

        var ncmbRecord = new NCMBObject(CurrentRanking.ClassName);
        ncmbRecord.ObjectId = NcmbObjectId;
        ncmbRecord[ColumnName] = userName;
        ncmbRecord[ColumnScore] = score.Value;

        StartCoroutine(DoSendScore(CurrentRanking, ncmbRecord, onCompleted));
    }

    /// <summary>
    /// 登録されているハイスコアを取得する（非同期）
    /// </summary>
    /// <param name="onCompleted"></param>
    public void LoadHighScore(UnityAction<NCMBObject> onCompleted)
    {
        StartCoroutine(DoGetHighScore(CurrentRanking, NcmbObjectId, onCompleted));
    }

    /// <summary>
    /// スコア送信コルーチン
    /// </summary>
    /// <param name="rankingBoard"></param>
    /// <param name="ncmbObject"></param>
    /// <param name="onCompleted"></param>
    /// <returns></returns>
    private IEnumerator DoSendScore(RankingInfo rankingBoard, NCMBObject ncmbObject, UnityAction<NCMBObject> onCompleted)
    {
        Debug.Log("スコア送信中...");

        NCMBException errorResult = null;

        yield return ncmbObject.YieldableSaveAsync(error => errorResult = error);

        if (errorResult != null)
        {
            // NCMBのコンソールから直接削除した場合に、該当のobjectIdが無いので発生する（らしい）
            // 新規として送信
            ncmbObject.ObjectId = null;
            yield return ncmbObject.YieldableSaveAsync(error => errorResult = error);
        }

        // ObjectIDを保存して次に備える
        NcmbObjectId = ncmbObject.ObjectId;

        onCompleted?.Invoke(ncmbObject);

        Debug.Log("スコア送信完了");
    }

    /// <summary>
    /// ランキング取得コルーチン
    /// </summary>
    private IEnumerator DoLoadRankingBoard(RankingInfo rankingBoard, string userObjectId, UnityAction<YieldableNcmbQuery<NCMBObject>> onCompleted = null)
    {
        Debug.Log("ランキング情報取得中...");

        var ncmbQuery = new YieldableNcmbQuery<NCMBObject>(rankingBoard.ClassName);
        ncmbQuery.Limit = _rankingLimit;
        if (rankingBoard.Order == ScoreOrder.OrderByAscending)
        {
            ncmbQuery.OrderByAscending(ColumnScore);
        }
        else
        {
            ncmbQuery.OrderByDescending(ColumnScore);
        }

        yield return ncmbQuery.FindAsync();

        onCompleted?.Invoke(ncmbQuery);

        Debug.Log(string.Format("データ取得: {0}件", ncmbQuery.Count));
    }

    /// <summary>
    /// ハイスコア取得コルーチン
    /// </summary>
    /// <param name="rankingBoard"></param>
    /// <returns></returns>
    private IEnumerator DoGetHighScore(RankingInfo rankingBoard, string userObjectId, UnityAction<NCMBObject> onCompleted)
    {
        var highScoreCheck = new YieldableNcmbQuery<NCMBObject>(rankingBoard.ClassName);
        highScoreCheck.WhereEqualTo(ObjectId, userObjectId);
        yield return highScoreCheck.FindAsync();

        var ncmbRecord = highScoreCheck.Result.FirstOrDefault();

        // キャッシュ更新
        if (ncmbRecord != null)
        {
            (var name, var highScore) = GetInformation(ncmbRecord);
            _highScore = highScore;
        }

        onCompleted?.Invoke(ncmbRecord);
    }
    #endregion
}
