using UnityEngine;

public class TextTutorialStarter : MonoBehaviour
{
    [SerializeField] private TextTutorialPopup popup;

    private void Start()
    {
        if (popup == null) return;
        popup.TryShowTutorial();
    }
}
