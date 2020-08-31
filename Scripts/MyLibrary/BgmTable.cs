using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ゲーム楽曲リスト
/// </summary>
[CreateAssetMenu]
public class BgmTable : ScriptableObject
{
    /// <summary>
    /// BGMリスト
    /// </summary>
    [SerializeField]
    public BgmData[] Bgms = null;
}
