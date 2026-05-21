using UnityEngine;
using UnityEngine.UI;

public class PausePopup : BasePopup
{
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private GameSettingsPopup gameSettingsPopup;
    [SerializeField] private ConfirmPopup confirmPopup;

    protected override void Awake()
    {
        base.Awake();
        resumeButton.onClick.AddListener(OnResume);
        settingsButton.onClick.AddListener(OnSettings);
        mainMenuButton.onClick.AddListener(OnMainMenu);
    }

    private void OnResume()
    {
        Hide();
    }

    private void OnSettings()
    {
        HideWithoutResume();
        gameSettingsPopup.SetReturnPopup(this);
        gameSettingsPopup.Show();
    }

    private void HideWithoutResume()
    {
        bool prev = pausesGame;
        pausesGame = false;
        Hide();
        pausesGame = prev;
    }

    private void OnMainMenu()
    {
        confirmPopup.Setup(
            "QUIT TO MENU?",
            "Your progress is saved.",
            "CANCEL",
            "QUIT",
            UIColors.RED,
            () =>
            {
                Time.timeScale = 1f;
                if (TransitionManager.Instance != null)
                    TransitionManager.Instance.LoadScene("MainMenu");
            }
        );
        confirmPopup.Show();
    }
}
