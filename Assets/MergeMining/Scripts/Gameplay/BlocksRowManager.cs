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
    [SerializeField] private ZoneCompletePopup zoneCompletePopup;
    [SerializeField, HideInInspector] private int blocksPerZone = 10;

    private const string TOTAL_DESTROYED_KEY = "total_blocks_destroyed";

    private List<Block> activeBlocks = new List<Block>();
    private int totalDestroyed = 0;
    private RectTransform coinsHudTarget;

    private LevelDefinition activeLevel;
    private int spawnIndexCounter;
    private int remainingBlocksToSpawn;

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
        for (int i = 0; i < blockSlots.Count; i++) activeBlocks.Add(null);
    }

    private void FindCoinsHudTarget()
    {
        GameObject hud = GameObject.Find("CoinsIcon");
        if (hud != null) coinsHudTarget = hud.GetComponent<RectTransform>();
    }

    public void BeginLevelBattle(LevelDefinition level)
    {
        activeLevel = level;
        spawnIndexCounter = 0;
        remainingBlocksToSpawn = level.blocksToDestroy;

        for (int i = 0; i < activeBlocks.Count; i++) activeBlocks[i] = null;

        int initialBlocks = Mathf.Min(blockSlots.Count, remainingBlocksToSpawn);
        for (int i = 0; i < initialBlocks; i++)
        {
            SpawnBlockAt(i);
        }
    }

    private void SpawnBlockAt(int index)
    {
        if (index < 0 || index >= blockSlots.Count) return;
        if (activeLevel == null) return;
        if (remainingBlocksToSpawn <= 0) return;

        List<int> seq = activeLevel.blockSequence;
        if (seq == null || seq.Count == 0) seq = new List<int> { 0 };

        BlockTypeData type = BlockConfigProvider.Config.GetTypeForBlock(spawnIndexCounter, seq);
        float hp = activeLevel.blockHP * (type != null ? type.hpMultiplier : 1f);
        if (LevelManager.Instance != null && LevelManager.Instance.ActiveModifier != null)
        {
            hp *= LevelManager.Instance.ActiveModifier.BlockHPMultiplier;
        }
        int reward = Mathf.Max(1, Mathf.RoundToInt(3f * (type != null ? type.rewardMultiplier : 1f)));

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
        block.SetRegen(activeLevel.blockRegenDelay, activeLevel.blockRegenPerSec);

        float descendSpeed = activeLevel.blockDescendSpeed;
        if (LevelManager.Instance != null && LevelManager.Instance.ActiveModifier != null)
        {
            descendSpeed *= LevelManager.Instance.ActiveModifier.DescendSpeedMultiplier;
        }
        if (descendSpeed > 0f && PickaxeGridManager.Instance != null && PickaxeGridManager.Instance.DragLayer != null)
        {
            float dangerY = ComputeDangerLineY();
            block.SetDescend(descendSpeed, dangerY);
        }
        block.OnDestroyed += OnBlockDestroyed;
        activeBlocks[index] = block;

        spawnIndexCounter++;
        remainingBlocksToSpawn--;
    }

    private float ComputeDangerLineY()
    {
        if (PickaxeGridManager.Instance == null) return float.NegativeInfinity;
        GameObject gridGo = GameObject.Find("PickaxeGrid");
        if (gridGo == null) return float.NegativeInfinity;
        RectTransform gridRt = gridGo.transform as RectTransform;
        Vector3[] corners = new Vector3[4];
        gridRt.GetWorldCorners(corners);
        return corners[1].y;
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
        if (SfxLibrary.Instance != null) SfxLibrary.Instance.Play(SfxLibrary.Instance.blockExplode);

        int reward = block.RewardCoins;
        Debug.Log("[BLOCK DESTROYED] " + block.TypeData.id + " reward=" + reward + " FlySpawner=" + (FlyEffectSpawner.Instance != null) + " CurrencyMgr=" + (CurrencyManager.Instance != null));
        if (reward > 0 && FlyEffectSpawner.Instance != null)
        {
            FlyEffectSpawner.Instance.FlyCoins(block.RectTransform.position, reward, 5);
        }
        else if (reward > 0 && CurrencyManager.Instance != null)
        {
            CurrencyManager.Instance.AddCoins(reward);
        }

        totalDestroyed++;
        PlayerPrefs.SetInt(TOTAL_DESTROYED_KEY, totalDestroyed);

        if (LevelManager.Instance != null) LevelManager.Instance.NotifyBlockDestroyed();
        if (AchievementManager.Instance != null) AchievementManager.Instance.CheckAll();

        DOVirtual.DelayedCall(respawnDelay, () => SpawnBlockAt(index));
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
