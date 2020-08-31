using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class BgmData
{
    /// <summary>
    /// 楽曲名
    /// </summary>
    [SerializeField]
    public string Title = string.Empty;

    /// <summary>
    /// オーディオデータ
    /// </summary>
    [SerializeField]
    public AudioClip Clip = null;
}
