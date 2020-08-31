using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// シンプルなボタンSE付与
/// </summary>
[RequireComponent(typeof(Button))]
public class ButtonSe : MonoBehaviour
{
    /// <summary>
    /// SEクリップ
    /// </summary>
    [SerializeField]
    private AudioClip _clip = null;

    /// <summary>
    /// 再生チャンネルのタグ
    /// </summary>
    [SerializeField]
    private string _audioSourceTag = "SystemSE";

    [Header("Option")]
    /// <summary>
    /// 再生チャンネル（自動で検出する場合は指定不要）
    /// </summary>
    [SerializeField]
    private AudioSource _specifiedAudioSource = null;

    /// <summary>
    /// 再生チャンネルがない場合にAudioSource動的生成するか
    /// </summary>
    [SerializeField]
    private bool _enableFallback = false;


    private Button _button = null;
    private AudioSource _audioSource = null;


    private void Awake()
    {
        _button = GetComponent<Button>();

        // AudioSourceの設定
        if (_specifiedAudioSource != null)
        {
            _audioSource = _specifiedAudioSource;
        }
        else
        {
            GameObject audioSourceObject = GameObject.FindWithTag(_audioSourceTag);
            if (audioSourceObject != null)
            {
                _audioSource = audioSourceObject.GetComponent<AudioSource>();
            }
        }
#if UNITY_EDITOR
        Debug.Assert(_audioSource != null && !_enableFallback, "AudioSource is not found.");
#endif
    }

    private void OnEnable()
    {
        _button.onClick.AddListener(PlaySe);
    }

    private void OnDisable()
    {
        _button.onClick.RemoveListener(PlaySe);
    }


    /// <summary>
    /// SEを鳴らす
    /// </summary>
    private void PlaySe()
    {
        if (_clip != null)
        {
            if (_audioSource != null)
            {
                _audioSource.PlayOneShot(_clip);
            }
            else if (_enableFallback)
            {
                AudioSource.PlayClipAtPoint(_clip, transform.position);
            }
        }
    }
}
