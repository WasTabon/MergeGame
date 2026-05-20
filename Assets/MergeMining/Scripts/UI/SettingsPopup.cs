using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsPopup : BasePopup
{
    [SerializeField] private Toggle sfxToggle;
    [SerializeField] private Toggle musicToggle;
    [SerializeField] private Toggle hapticToggle;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Button closeButton;
    [SerializeField] private Button restartProgressButton;
    [SerializeField] private TextMeshProUGUI titleText;

    protected override void Awake()
    {
        base.Awake();
        closeButton.onClick.AddListener(Hide);
        restartProgressButton.onClick.AddListener(OnRestartProgress);
        sfxToggle.onValueChanged.AddListener(OnSfxToggle);
        musicToggle.onValueChanged.AddListener(OnMusicToggle);
        hapticToggle.onValueChanged.AddListener(OnHapticToggle);
        sfxSlider.onValueChanged.AddListener(OnSfxSlider);
        musicSlider.onValueChanged.AddListener(OnMusicSlider);
    }

    protected override void OnShow()
    {
        if (SoundManager.Instance != null)
        {
            sfxToggle.SetIsOnWithoutNotify(!SoundManager.Instance.SfxMuted);
            musicToggle.SetIsOnWithoutNotify(!SoundManager.Instance.MusicMuted);
            sfxSlider.SetValueWithoutNotify(SoundManager.Instance.SfxVolume);
            musicSlider.SetValueWithoutNotify(SoundManager.Instance.MusicVolume);
        }
        if (HapticManager.Instance != null)
        {
            hapticToggle.SetIsOnWithoutNotify(HapticManager.Instance.HapticsEnabled);
        }
    }

    private void OnSfxToggle(bool on)
    {
        if (SoundManager.Instance != null) SoundManager.Instance.SetSfxMuted(!on);
    }

    private void OnMusicToggle(bool on)
    {
        if (SoundManager.Instance != null) SoundManager.Instance.SetMusicMuted(!on);
    }

    private void OnHapticToggle(bool on)
    {
        if (HapticManager.Instance != null) HapticManager.Instance.SetEnabled(on);
    }

    private void OnSfxSlider(float v)
    {
        if (SoundManager.Instance != null) SoundManager.Instance.SetSfxVolume(v);
    }

    private void OnMusicSlider(float v)
    {
        if (SoundManager.Instance != null) SoundManager.Instance.SetMusicVolume(v);
    }

    private void OnRestartProgress()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        Debug.Log("Progress restarted. Reload scene to see effect.");
    }
}
