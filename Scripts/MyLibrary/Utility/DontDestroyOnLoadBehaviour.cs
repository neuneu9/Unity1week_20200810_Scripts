using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 開始時にDontDestroyOnLoadへ移動させるコンポーネント
/// </summary>
public class DontDestroyOnLoadBehaviour : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}
