using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// unityroom用Tweetボタン
/// </summary>
[RequireComponent(typeof(Button))]
public class UnityroomTweetButton : MonoBehaviour
{
    /// <summary>
    /// unityroomゲーム固有ID
    /// ref. https://github.com/naichilab/unityroom-tweet
    /// </summary>
    public static string UnityroomGameId = "YOUR-GAMEID";

    /// <summary>
    /// ツイートメッセージ
    /// </summary>
    public static string TweetText = "サンプルツイートです。";

    /// <summary>
    /// ハッシュタグ
    /// </summary>
    public static string[] HashTags = new string[] { "unityroom", "unity1week" };


    private Button _button = null;


    private void Awake()
    {
        _button = GetComponent<Button>();

        _button.onClick.AddListener(() =>
        {
            OpenTweet();
        });
    }

    private void OpenTweet()
    {
        naichilab.UnityRoomTweet.Tweet(UnityroomGameId, TweetText, HashTags);
    }
}
