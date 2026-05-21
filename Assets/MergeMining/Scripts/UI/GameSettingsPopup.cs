using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameSettingsPopup : BasePopup
{
    [SerializeField] private Toggle sfxToggle;
    [SerializeField] private Toggle musicToggle;
    [SerializeField] private Toggle hapticToggle;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Button closeButton;
    [SerializeField] private TextMeshProUGUI titleText;

    private BasePopup returnPopup;

    protected override void Awake()
    {
        base.Awake();
        closeButton.onClick.AddListener(OnClose);
        sfxToggle.onValueChanged.AddListener(OnSfxToggle);
        musicToggle.onValueChanged.AddListener(OnMusicToggle);
        hapticToggle.onValueChanged.AddListener(OnHapticToggle);
        sfxSlider.onValueChanged.AddListener(OnSfxSlider);
        musicSlider.onValueChanged.AddListener(OnMusicSlider);
    }

    public void SetReturnPopup(BasePopup popup)
    {
        returnPopup = popup;
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

    private void OnClose()
    {
        Hide();
        if (returnPopup != null)
        {
            returnPopup.Show();
            returnPopup = null;
        }
    }

    private void OnSfxToggle(bool on) { if (SoundManager.Instance != null) SoundManager.Instance.SetSfxMuted(!on); }
    private void OnMusicToggle(bool on) { if (SoundManager.Instance != null) SoundManager.Instance.SetMusicMuted(!on); }
    private void OnHapticToggle(bool on) { if (HapticManager.Instance != null) HapticManager.Instance.SetEnabled(on); }
    private void OnSfxSlider(float v) { if (SoundManager.Instance != null) SoundManager.Instance.SetSfxVolume(v); }
    private void OnMusicSlider(float v) { if (SoundManager.Instance != null) SoundManager.Instance.SetMusicVolume(v); }
}
