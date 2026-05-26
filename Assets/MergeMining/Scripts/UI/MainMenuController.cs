using UnityEngine;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private Button playButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private SettingsPopup settingsPopup;

    private void Start()
    {
        playButton.onClick.AddListener(OnPlayClicked);
        settingsButton.onClick.AddListener(OnSettingsClicked);
    }

    private void OnPlayClicked()
    {
        TransitionManager.Instance.LoadScene("LevelSelect");
    }

    private void OnSettingsClicked()
    {
        settingsPopup.Show();
    }
}
