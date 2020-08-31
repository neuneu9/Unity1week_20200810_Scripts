using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ボーナス選択パネル
/// </summary>
[RequireComponent(typeof(CanvasGroup))]
public class BonusWindow : MonoBehaviour
{
    [SerializeField]
    private Button[] _bonusButtons = null;

    [SerializeField]
    private Text[] _bonusRemainTexts = null;

    [SerializeField]
    private SlideInWindowPanel _slideInWindowPanel = null;

    [SerializeField]
    private int[] _maxCounts = new int[]
    {
        3, 3, 3, int.MaxValue,
    };

    private CanvasGroup _canvasGroup = null;


    private void Reset()
    {
        _slideInWindowPanel = GetComponentInParent<SlideInWindowPanel>();
        _bonusButtons = GetComponentsInChildren<Button>();
    }

    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();

        // ボーナスボタンのイベントを設定
        for (int i = 0; i < _bonusButtons.Length; i++)
        {
            int index = i;

            _bonusButtons[i].onClick.AddListener(() =>
            {
                if (_maxCounts[index] < int.MaxValue)
                {
                    _maxCounts[index]--;
                }

                StartCoroutine(WaitAndClose());
            });
        }
    }

    private void OnEnable()
    {
        for (int i = 0; i < _bonusButtons.Length; i++)
        {
            if (_maxCounts[i] < int.MaxValue)
            {
                _bonusRemainTexts[i].text = string.Format("あと{0}回可能", _maxCounts[i]);
            }
            else
            {
                _bonusRemainTexts[i].text = "回数制限なし";
            }

            _bonusButtons[i].interactable = (_maxCounts[i] > 0);
        }

        _canvasGroup.interactable = true;
    }

    private IEnumerator WaitAndClose()
    {
        _canvasGroup.interactable = false;

        yield return new WaitForSecondsRealtime(0.5f);

        _slideInWindowPanel.Close();
    }
}
