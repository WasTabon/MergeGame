using UnityEngine;
using UnityEngine.UI;

public class AchievementsButtonHandler : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private AchievementsListPopup popup;

    private void Start()
    {
        if (button != null) button.onClick.AddListener(OnClicked);
    }

    private void OnClicked()
    {
        if (popup != null) popup.Show();
    }
}
