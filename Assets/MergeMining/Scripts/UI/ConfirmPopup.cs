using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ConfirmPopup : BasePopup
{
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private Button cancelButton;
    [SerializeField] private Button confirmButton;
    [SerializeField] private TextMeshProUGUI cancelLabel;
    [SerializeField] private TextMeshProUGUI confirmLabel;
    [SerializeField] private Image confirmButtonImage;

    private Action onConfirmCallback;

    protected override void Awake()
    {
        base.Awake();
        cancelButton.onClick.AddListener(OnCancel);
        confirmButton.onClick.AddListener(OnConfirm);
    }

    public void Setup(string title, string message, string cancelText, string confirmText, Color confirmColor, Action onConfirm)
    {
        titleText.text = title;
        messageText.text = message;
        cancelLabel.text = cancelText;
        confirmLabel.text = confirmText;
        confirmButtonImage.color = confirmColor;
        onConfirmCallback = onConfirm;
    }

    private void OnCancel()
    {
        onConfirmCallback = null;
        Hide();
    }

    private void OnConfirm()
    {
        Action cb = onConfirmCallback;
        onConfirmCallback = null;
        Hide();
        cb?.Invoke();
    }
}
