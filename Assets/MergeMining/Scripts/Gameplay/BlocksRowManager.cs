using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BlocksRowManager : MonoBehaviour
{
    public static BlocksRowManager Instance { get; private set; }

    [SerializeField] private List<RectTransform> blockSlots;
    [SerializeField] private GameObject blockPrefabRoot;
    [SerializeField] private GameObject blockDestroyEffectPrefab;
    [SerializeField] private GameObject coinBurstParticlePrefab;
    [SerializeField] private RectTransform effectsLayer;
    [SerializeField] private float respawnDelay = 0.35f;
    [SerializeField] private int coinsBurstCount = 6;

    private const string TOTAL_DESTROYED_KEY = "total_blocks_destroyed";

    private List<Block> activeBlocks = new List<Block>();
    private int totalDestroyed = 0;
    private RectTransform coinsHudTarget;

    public IReadOnlyList<Block> ActiveBlocks => activeBlocks;
    public int TotalDestroyed => totalDestroyed;

    private void Awake()
    {
        Instance = this;
        totalDestroyed = PlayerPrefs.GetInt(TOTAL_DESTROYED_KEY, 0);
    }

    private void Start()
    {
        FindCoinsHudTarget();
        SpawnInitialRow();
    }

    private void FindCoinsHudTarget()
    {
        GameObject hud = GameObject.Find("CoinsIcon");
        if (hud != null) coinsHudTarget = hud.GetComponent<RectTransform>();
    }

    private void SpawnInitialRow()
    {
        for (int i = 0; i < blockSlots.Count; i++)
        {
            activeBlocks.Add(null);
            SpawnBlockAt(i);
        }
    }

    private void SpawnBlockAt(int index)
    {
        if (index < 0 || index >= blockSlots.Count) return;

        BlockTypeData type = BlockConfigProvider.Config.GetTypeForBlock(totalDestroyed + index);
        float hp = BlockConfigProvider.Config.CalcHP(totalDestroyed + index);
        int reward = BlockConfigProvider.Config.CalcReward(totalDestroyed + index);

        GameObject go = Instantiate(blockPrefabRoot, blockSlots[index]);
        go.name = "Block_" + type.id;
        RectTransform rt = go.transform as RectTransform;
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = Vector2.zero;
        rt.sizeDelta = new Vector2(200f, 260f);
        go.SetActive(true);

        Block block = go.GetComponent<Block>();
        block.Setup(type, hp, reward);
        block.OnDestroyed += OnBlockDestroyed;
        activeBlocks[index] = block;
    }

    private void OnBlockDestroyed(Block block)
    {
        int index = activeBlocks.IndexOf(block);
        if (index < 0) return;

        activeBlocks[index] = null;

        if (blockDestroyEffectPrefab != null && effectsLayer != null)
        {
            GameObject fx = Instantiate(blockDestroyEffectPrefab, effectsLayer);
            fx.SetActive(true);
            RectTransform fxRt = fx.transform as RectTransform;
            fxRt.position = block.RectTransform.position;
            BlockDestroyEffect bde = fx.GetComponent<BlockDestroyEffect>();
            if (bde != null) bde.Play(block.TypeData.color);
        }

        Camera cam = Camera.main;
        if (cam != null)
        {
            cam.transform.DOKill();
            cam.transform.DOShakePosition(0.25f, 0.2f, 12, 90f);
        }

        if (HapticManager.Instance != null) HapticManager.Instance.Medium();

        SpawnCoinBurst(block.RectTransform.position, block.RewardCoins);

        totalDestroyed++;
        PlayerPrefs.SetInt(TOTAL_DESTROYED_KEY, totalDestroyed);

        DOVirtual.DelayedCall(respawnDelay, () => SpawnBlockAt(index));
    }

    private void SpawnCoinBurst(Vector3 worldPos, int totalReward)
    {
        if (coinBurstParticlePrefab == null || effectsLayer == null || coinsHudTarget == null)
        {
            if (CurrencyManager.Instance != null) CurrencyManager.Instance.AddCoins(totalReward);
            return;
        }

        int n = coinsBurstCount;
        int per = Mathf.Max(1, totalReward / n);
        int remainder = totalReward - per * n;

        for (int i = 0; i < n; i++)
        {
            GameObject coin = Instantiate(coinBurstParticlePrefab, effectsLayer);
            coin.SetActive(true);
            RectTransform crt = coin.transform as RectTransform;
            crt.position = worldPos;

            int amount = per + (i == n - 1 ? remainder : 0);
            float delay = i * 0.04f;

            Vector2 randomOffset = Random.insideUnitCircle * 120f;
            randomOffset.y = Mathf.Abs(randomOffset.y) + 60f;
            Vector3 burstTarget = worldPos + (Vector3)randomOffset;

            Sequence seq = DOTween.Sequence();
            seq.AppendInterval(delay);
            seq.Append(crt.DOMove(burstTarget, 0.25f).SetEase(Ease.OutQuad));
            seq.Join(crt.DOScale(1.3f, 0.25f).SetEase(Ease.OutQuad));
            seq.AppendInterval(0.05f);
            seq.Append(crt.DOMove(coinsHudTarget.position, 0.45f).SetEase(Ease.InQuad));
            seq.Join(crt.DOScale(0.8f, 0.45f).SetEase(Ease.InQuad));
            seq.AppendCallback(() =>
            {
                if (CurrencyManager.Instance != null) CurrencyManager.Instance.AddCoins(amount);
                if (HapticManager.Instance != null && amount == per + remainder) HapticManager.Instance.Light();
                Destroy(coin);
            });
        }
    }

    public Block GetRandomAliveBlock()
    {
        List<Block> alive = new List<Block>();
        foreach (var b in activeBlocks)
        {
            if (b != null && b.IsAlive) alive.Add(b);
        }
        if (alive.Count == 0) return null;
        return alive[Random.Range(0, alive.Count)];
    }
}
