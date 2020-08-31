using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ラベルと値表示
/// </summary>
public class LabelValue : MonoBehaviour
{
    /// <summary>
    /// ラベル表示
    /// </summary>
    [SerializeField]
    protected Text _labelText = null;

    /// <summary>
    /// 値表示先
    /// </summary>
    [SerializeField]
    protected Text _valueText = null;

    /// <summary>
    /// 値
    /// </summary>
    [SerializeField]
    protected int _value = 0;


    public virtual string Label
    {
        set
        {
            _labelText.text = value;
        }
    }

    public virtual int Value
    {
        set
        {
            if (value != _value)
            {
                _value = value;
            }
        }
        get { return _value; }
    }


    private void Reset()
    {
        _labelText = transform.Find("Label")?.GetComponent<Text>();
        _valueText = transform.Find("Value")?.GetComponent<Text>();
    }

    private void Start()
    {
        _valueText.text = _value.ToString();
    }


    /// <summary>
    /// 表示セット
    /// </summary>
    /// <param name="label"></param>
    /// <param name="value"></param>
    public void Set(string label, int value)
    {
        _labelText.text = label;
        _valueText.text = value.ToString();
    }

#if UNITY_EDITOR
    /// <summary>
    /// 子要素を生成
    /// </summary>
    [ContextMenu("Create")]
    private void Create()
    {
        float labelAreaRatio = 0.3f;

        var rectTransform = GetComponent<RectTransform>();
        float totalWidth = rectTransform.rect.width;

        var label = new GameObject("Label", new System.Type[] { typeof(RectTransform) });
        var labelRectTransform = label.GetComponent<RectTransform>();
        labelRectTransform.SetParent(transform);
        labelRectTransform.anchorMin = Vector2.zero;
        labelRectTransform.anchorMax = new Vector2(0f, 1f);
        labelRectTransform.anchoredPosition = new Vector2((totalWidth * labelAreaRatio) / 2f, 0f);
        labelRectTransform.sizeDelta = new Vector2(totalWidth * labelAreaRatio, 0f);
        labelRectTransform.localScale = Vector2.one;

        _labelText = label.AddComponent<Text>();
        _labelText.text = "Label";
        _labelText.alignment = TextAnchor.MiddleCenter;

        var value = new GameObject("Value", new System.Type[] { typeof(RectTransform) });
        var valueRectTransform = value.GetComponent<RectTransform>();

        valueRectTransform.SetParent(transform);
        valueRectTransform.anchorMin = Vector2.zero;
        valueRectTransform.anchorMax = Vector2.one;
        valueRectTransform.anchoredPosition = new Vector2((totalWidth * labelAreaRatio) / 2f, 0f);
        valueRectTransform.sizeDelta = new Vector2(-(totalWidth * labelAreaRatio), 0f);
        valueRectTransform.localScale = Vector2.one;

        _valueText = value.AddComponent<Text>();
        _valueText.text = "Value";
        _valueText.alignment = TextAnchor.MiddleCenter;
    }
#endif
}
