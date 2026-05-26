using UnityEngine;
using DG.Tweening;

public class ModifierChoiceStarter : MonoBehaviour
{
    [SerializeField] private ModifierChoicePopup popup;
    [SerializeField] private float delaySeconds = 0.6f;

    private bool shown = false;
    private float tryAt = -1f;

    private void Start()
    {
        tryAt = Time.unscaledTime + delaySeconds;
    }

    private void Update()
    {
        if (shown) return;
        if (popup == null) return;
        if (LevelManager.Instance == null) return;

        if (!TextTutorialPopup.IsDone()) { tryAt = Time.unscaledTime + 0.3f; return; }
        if (PopupManager.Instance != null && PopupManager.Instance.IsAnyPopupOpen) { tryAt = Time.unscaledTime + 0.3f; return; }

        if (Time.unscaledTime < tryAt) return;

        shown = true;
        popup.ShowChoice();
    }
}
