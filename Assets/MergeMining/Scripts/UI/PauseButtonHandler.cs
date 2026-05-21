using UnityEngine;
using UnityEngine.UI;

public class PauseButtonHandler : MonoBehaviour
{
    [SerializeField] private Button pauseButton;
    [SerializeField] private PausePopup pausePopup;

    private void Start()
    {
        if (pauseButton != null) pauseButton.onClick.AddListener(OnPauseClicked);
    }

    private void OnPauseClicked()
    {
        if (pausePopup != null) pausePopup.Show();
    }
}
