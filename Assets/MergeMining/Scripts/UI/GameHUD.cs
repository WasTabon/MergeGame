using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class GameHUD : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI coinsText;
    [SerializeField] private TextMeshProUGUI gemsText;
    [SerializeField] private Button pauseButton;
    [SerializeField] private RectTransform coinsIcon;
    [SerializeField] private RectTransform gemsIcon;

    private int displayedCoins;
    private int displayedGems;
    private Tweener coinsTween;
    private Tweener gemsTween;

    private void OnEnable()
    {
        SubscribeToEvents();
        RefreshImmediate();
    }

    private void OnDisable()
    {
        UnsubscribeFromEvents();
    }

    private void Start()
    {
        pauseButton.onClick.AddListener(OnPauseClicked);
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

    private void RefreshImmediate()
    {
        if (CurrencyManager.Instance == null) return;
        displayedCoins = CurrencyManager.Instance.Coins;
        displayedGems = CurrencyManager.Instance.Gems;
        coinsText.text = displayedCoins.ToString();
        gemsText.text = displayedGems.ToString();
    }

    private void OnCoinsChanged(int newValue)
    {
        coinsTween?.Kill();
        int start = displayedCoins;
        coinsTween = DOTween.To(() => start, x => { displayedCoins = x; coinsText.text = x.ToString(); }, newValue, 0.4f)
            .SetEase(Ease.OutQuad);

        coinsIcon.DOKill();
        coinsIcon.localScale = Vector3.one;
        coinsIcon.DOPunchScale(Vector3.one * 0.25f, 0.25f, 6, 0.5f);
    }

    private void OnGemsChanged(int newValue)
    {
        gemsTween?.Kill();
        int start = displayedGems;
        gemsTween = DOTween.To(() => start, x => { displayedGems = x; gemsText.text = x.ToString(); }, newValue, 0.4f)
            .SetEase(Ease.OutQuad);

        gemsIcon.DOKill();
        gemsIcon.localScale = Vector3.one;
        gemsIcon.DOPunchScale(Vector3.one * 0.25f, 0.25f, 6, 0.5f);
    }

    private void OnPauseClicked()
    {
        if (HapticManager.Instance != null) HapticManager.Instance.Light();
    }
}
