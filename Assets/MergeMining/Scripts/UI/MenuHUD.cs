using UnityEngine;
using TMPro;

public class MenuHUD : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI coinsText;
    [SerializeField] private TextMeshProUGUI gemsText;

    private void OnEnable()
    {
        SubscribeToEvents();
        Refresh();
    }

    private void OnDisable()
    {
        UnsubscribeFromEvents();
    }

    private void SubscribeToEvents()
    {
        if (CurrencyManager.Instance != null)
        {
            CurrencyManager.Instance.OnCoinsChanged -= OnCoinsChanged;
            CurrencyManager.Instance.OnCoinsChanged += OnCoinsChanged;
            CurrencyManager.Instance.OnGemsChanged -= OnGemsChanged;
            CurrencyManager.Instance.OnGemsChanged += OnGemsChanged;
        }
    }

    private void UnsubscribeFromEvents()
    {
        if (CurrencyManager.Instance != null)
        {
            CurrencyManager.Instance.OnCoinsChanged -= OnCoinsChanged;
            CurrencyManager.Instance.OnGemsChanged -= OnGemsChanged;
        }
    }

    private void Refresh()
    {
        if (CurrencyManager.Instance == null) return;
        coinsText.text = CurrencyManager.Instance.Coins.ToString();
        gemsText.text = CurrencyManager.Instance.Gems.ToString();
    }

    private void OnCoinsChanged(int v) { coinsText.text = v.ToString(); }
    private void OnGemsChanged(int v) { gemsText.text = v.ToString(); }
}
