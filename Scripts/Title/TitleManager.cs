using UnityEngine;

/// <summary>
/// タイトルマネージャ
/// </summary>
public class TitleManager : MonoBehaviour
{
    private GameOption _gameOption = null;
    private GameRanking _gameRanking = null;


    private void Awake()
    {
        _gameOption = FindObjectOfType<GameOption>();
        _gameRanking = FindObjectOfType<GameRanking>();
    }

    public void OpenOptionPanel()
    {
        _gameOption.OpenPanel();
    }

    /// <summary>
    /// ランキングを開く
    /// </summary>
    public void OpenRankingPanel()
    {
#if UNITY_1WEEK
        _gameRanking.OpenRanking();
#endif
    }
}
