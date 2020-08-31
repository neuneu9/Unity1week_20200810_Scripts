using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


/// <summary>
/// ゲームマネージャ
/// </summary>
public class GameManager : MonoBehaviour
{
    /// <summary>
    /// 起動時の初期化メソッド
    /// </summary>
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void StartUp()
    {
        InstantiateIfPrefabExist<GameSound>("Prefabs/GameSound");
        InstantiateIfPrefabExist<GameScene>("Prefabs/GameScene");
        InstantiateIfPrefabExist<GameOption>("Prefabs/GameOption");

#if UNITY_1WEEK
        // ランキング関連
        InstantiateIfPrefabExist<GameRanking>("Prefabs/GameRanking");

        // シェア関連
        // ここはゲームごとに書き換える
        UnityroomTweetButton.UnityroomGameId = "planaria";
        UnityroomTweetButton.HashTags = new string[] { "unityroom", "unity1week" };
        UnityroomTweetButton.TweetText = "プラナリアを切って増やすゲーム！";
#endif
    }

    /// <summary>
    /// プレハブがあればインスタンスを生成
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="prefabPath"></param>
    /// <param name="onInstantiated"></param>
    private static void InstantiateIfPrefabExist<T>(string prefabPath, UnityAction<T> onInstantiated = null) where T : Object
    {
        var prefab = Resources.Load<T>(prefabPath);

        if (prefab != null)
        {
            var instance = Instantiate(prefab);

            onInstantiated?.Invoke(instance);
        }
    }
}
