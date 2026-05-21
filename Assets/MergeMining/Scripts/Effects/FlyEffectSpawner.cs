using UnityEngine;

public class FlyEffectSpawner : MonoBehaviour
{
    public static FlyEffectSpawner Instance { get; private set; }

    [SerializeField] private CurrencyFlyEffect coinFlyTemplate;
    [SerializeField] private CurrencyFlyEffect gemFlyTemplate;
    [SerializeField] private RectTransform layer;
    [SerializeField] private RectTransform coinsTarget;
    [SerializeField] private RectTransform gemsTarget;

    private void Awake()
    {
        Instance = this;
    }

    public void FlyCoins(Vector3 worldPos, int amount, int count = 5)
    {
        SpawnBurst(coinFlyTemplate, worldPos, amount, count, CurrencyType.Coins, coinsTarget);
    }

    public void FlyGems(Vector3 worldPos, int amount, int count = 4)
    {
        SpawnBurst(gemFlyTemplate, worldPos, amount, count, CurrencyType.Gems, gemsTarget);
    }

    private void SpawnBurst(CurrencyFlyEffect template, Vector3 worldPos, int amount, int count, CurrencyType type, RectTransform target)
    {
        if (template == null || layer == null || target == null) return;
        if (amount <= 0 || count <= 0) return;

        int per = Mathf.Max(1, amount / count);
        int remainder = amount - per * count;

        for (int i = 0; i < count; i++)
        {
            CurrencyFlyEffect fx = Instantiate(template, layer);
            fx.gameObject.SetActive(true);
            int thisAmount = per + (i == count - 1 ? remainder : 0);
            float delay = i * 0.05f;
            CurrencyFlyEffect captured = fx;
            int captAmount = thisAmount;
            DG.Tweening.DOVirtual.DelayedCall(delay, () =>
            {
                if (captured != null) captured.Launch(worldPos, target, type, captAmount);
            }, false);
        }
    }
}
