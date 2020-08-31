using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ゲームステージ制御
/// </summary>
public class Stage : MonoBehaviour
{
    [Header("Area")]
    [SerializeField]
    private PlayArea _playArea = null;

    /// <summary>
    /// デフォルトのエリア矩形
    /// </summary>
    [SerializeField]
    private Rect _defaultAreaRect = new Rect(-5f, -5f, 10f, 10f);

    /// <summary>
    /// 一回当たり狭める量
    /// </summary>
    [SerializeField]
    private float _shrinkAmmount = 1f;

    private int _shrinkCounter = 0;

    [Header("Victim Speed")]
    /// <summary>
    /// ベース移動速度
    /// </summary>
    [SerializeField]
    private float _defaultSpeed = 5f;

    /// <summary>
    /// 一回当たり遅くする量
    /// </summary>
    [SerializeField]
    private float _slowAmmount = 1f;

    private int _slowCounter = 0;

    [Header("Victim Ammount")]
    [SerializeField]
    private Victim _victimPrefab = null;

    /// <summary>
    /// 一回当たり追加する数
    /// </summary>
    [SerializeField]
    private int _addVictimCount = 5;

    [Header("Slicer")]
    [SerializeField]
    private Slicer _slicer = null;


    /// <summary>
    /// 一段階エリアを狭める
    /// </summary>
    public void ShrinkArea()
    {
        _shrinkCounter++;

        Rect rect = new Rect(
            _defaultAreaRect.xMin + (_shrinkAmmount * _shrinkCounter) / 2f,
            _defaultAreaRect.yMin + (_shrinkAmmount * _shrinkCounter) / 2f,
            _defaultAreaRect.width - (_shrinkAmmount * _shrinkCounter),
            _defaultAreaRect.height - (_shrinkAmmount * _shrinkCounter)
            );

        _playArea.SetRect(rect);
    }

    /// <summary>
    /// 対象を追加投入する
    /// </summary>
    public void AddVictim()
    {
        var margin = _victimPrefab.GetComponent<CircleCollider2D>().radius + 0.1f;

        for (int i = 0; i < _addVictimCount; i++)
        {
            var position = _playArea.GetRandomPositionInRect(margin);
            var rotation = Quaternion.AngleAxis(Random.Range(0f, 360f), Vector3.forward);
            var add = Instantiate(_victimPrefab, position, rotation);

            float speed = Mathf.Clamp(_defaultSpeed - (_slowAmmount * _slowCounter), 0f, float.MaxValue);
            add.SetSpeed(speed);
            add.StartMove(add.transform.right * speed);
        }
    }

    /// <summary>
    /// 一段階対象の移動を遅くする
    /// </summary>
    public void SlowVictim()
    {
        _slowCounter++;
        float speed = Mathf.Clamp(_defaultSpeed - (_slowAmmount * _slowCounter), 0f, float.MaxValue);

        var planarias = GameObject.FindGameObjectsWithTag("Planaria");
        for (int i = 0; i < planarias.Length; i++)
        {
            var victim = planarias[i].GetComponent<Victim>();
            if (victim != null)
            {
                victim.SetSpeed(speed);
            }
        }
    }

    /// <summary>
    /// 刃を追加する
    /// </summary>
    public void AddBlade()
    {
        _slicer.AddBlade();
    }

    private void Update()
    {
#if UNITY_EDITOR    // テストコード
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ShrinkArea();
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            AddVictim();
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SlowVictim();
        }

        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            AddBlade();
        }
#endif
    }
}
