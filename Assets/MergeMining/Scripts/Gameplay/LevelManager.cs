using System;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    [SerializeField] private LevelVictoryPopup victoryPopup;
    [SerializeField] private LevelHud levelHud;

    private const string CURRENT_LEVEL_KEY = "current_level";
    private const string MAX_PASSED_LEVEL_KEY = "max_passed_level";
    private const string PENDING_LEVEL_KEY = "pending_level_to_play";
    private const string LEVEL_STARS_PREFIX = "level_stars_";

    public int CurrentLevelNumber { get; private set; }
    public LevelDefinition CurrentLevel { get; private set; }
    public LevelPhase Phase { get; private set; } = LevelPhase.Setup;

    private int spentCoins;
    private int blocksDestroyed;

    public event Action<LevelPhase> OnPhaseChanged;
    public event Action<int, int> OnBlockProgress;

    private void Awake()
    {
        Instance = this;
        int requested = PlayerPrefs.GetInt(PENDING_LEVEL_KEY, 0);
        if (requested > 0)
        {
            PlayerPrefs.DeleteKey(PENDING_LEVEL_KEY);
            CurrentLevelNumber = requested;
        }
        else
        {
            CurrentLevelNumber = Mathf.Max(1, PlayerPrefs.GetInt(CURRENT_LEVEL_KEY, 1));
        }
        CurrentLevel = LevelConfigProvider.Config.GetLevel(CurrentLevelNumber);
    }

    private void Start()
    {
        BeginSetupPhase();
    }

    private void BeginSetupPhase()
    {
        Phase = LevelPhase.Setup;
        spentCoins = 0;
        blocksDestroyed = 0;

        if (CurrencyManager.Instance != null)
        {
            int currentCoins = CurrencyManager.Instance.Coins;
            int target = CurrentLevel.startingCoins;
            if (currentCoins < target) CurrencyManager.Instance.AddCoins(target - currentCoins);
            else if (currentCoins > target) CurrencyManager.Instance.SpendCoins(currentCoins - target);
        }

        OnPhaseChanged?.Invoke(Phase);
    }

    public void StartBattle()
    {
        if (Phase != LevelPhase.Setup) return;
        Phase = LevelPhase.Battle;
        if (BlocksRowManager.Instance != null) BlocksRowManager.Instance.BeginLevelBattle(CurrentLevel);
        OnPhaseChanged?.Invoke(Phase);
    }

    public void NotifyBlockDestroyed()
    {
        if (Phase != LevelPhase.Battle) return;
        blocksDestroyed++;
        OnBlockProgress?.Invoke(blocksDestroyed, CurrentLevel.blocksToDestroy);
        if (blocksDestroyed >= CurrentLevel.blocksToDestroy)
        {
            FinishLevel();
        }
    }

    public void NotifyCoinsSpent(int amount)
    {
        spentCoins += amount;
    }

    public int GetPickaxeBaseCost() => LevelConfigProvider.Config.pickaxeBaseCost;

    public int GetBlocksTotal() => CurrentLevel != null ? CurrentLevel.blocksToDestroy : 0;
    public int GetBlocksDestroyed() => blocksDestroyed;

    private void FinishLevel()
    {
        Phase = LevelPhase.Victory;

        int aliveCount = 0;
        if (PickaxeGridManager.Instance != null)
        {
            aliveCount = PickaxeGridManager.Instance.CountAlive();
        }
        int leftoverCoins = CurrencyManager.Instance != null ? CurrencyManager.Instance.Coins : 0;

        int stars = CalcStars(aliveCount, leftoverCoins);

        if (CurrentLevelNumber > PlayerPrefs.GetInt(MAX_PASSED_LEVEL_KEY, 0))
        {
            PlayerPrefs.SetInt(MAX_PASSED_LEVEL_KEY, CurrentLevelNumber);
        }
        int oldStars = PlayerPrefs.GetInt(LEVEL_STARS_PREFIX + CurrentLevelNumber, 0);
        if (stars > oldStars)
        {
            PlayerPrefs.SetInt(LEVEL_STARS_PREFIX + CurrentLevelNumber, stars);
        }

        int nextLevel = CurrentLevelNumber + 1;
        if (nextLevel > LevelConfigProvider.Config.Count) nextLevel = LevelConfigProvider.Config.Count;
        PlayerPrefs.SetInt(CURRENT_LEVEL_KEY, nextLevel);

        if (CurrencyManager.Instance != null) CurrencyManager.Instance.AddGems(CurrentLevel.gemsReward);

        OnPhaseChanged?.Invoke(Phase);
        if (victoryPopup != null) victoryPopup.ShowVictory(CurrentLevelNumber, stars, CurrentLevel.gemsReward);
    }

    private int CalcStars(int aliveCount, int leftoverCoins)
    {
        int budget = CurrentLevel.startingCoins;
        int approxPickaxes = Mathf.Max(1, budget / Mathf.Max(1, LevelConfigProvider.Config.pickaxeBaseCost));

        float aliveRatio = approxPickaxes > 0 ? (float)aliveCount / approxPickaxes : 0f;
        float coinsRatio = budget > 0 ? (float)leftoverCoins / budget : 0f;

        if (aliveRatio >= 0.7f || coinsRatio >= 0.5f) return 3;
        if (aliveRatio >= 0.4f || coinsRatio >= 0.25f) return 2;
        return 1;
    }

    public void RestartLevel()
    {
        PlayerPrefs.SetInt(PENDING_LEVEL_KEY, CurrentLevelNumber);
        if (PickaxeGridManager.Instance != null) PickaxeGridManager.Instance.ClearGridAndSave();
        if (TransitionManager.Instance != null) TransitionManager.Instance.LoadScene("Game");
    }

    public void GoToNextLevel()
    {
        int next = CurrentLevelNumber + 1;
        if (next > LevelConfigProvider.Config.Count) next = LevelConfigProvider.Config.Count;
        PlayerPrefs.SetInt(PENDING_LEVEL_KEY, next);
        if (PickaxeGridManager.Instance != null) PickaxeGridManager.Instance.ClearGridAndSave();
        if (TransitionManager.Instance != null) TransitionManager.Instance.LoadScene("Game");
    }

    public void GoToLevelSelect()
    {
        if (PickaxeGridManager.Instance != null) PickaxeGridManager.Instance.ClearGridAndSave();
        if (TransitionManager.Instance != null) TransitionManager.Instance.LoadScene("LevelSelect");
    }

    public int GetStars(int levelNumber)
    {
        return PlayerPrefs.GetInt(LEVEL_STARS_PREFIX + levelNumber, 0);
    }
}
