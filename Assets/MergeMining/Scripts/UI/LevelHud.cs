using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class LevelHud : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI phaseText;
    [SerializeField] private TextMeshProUGUI blocksProgressText;
    [SerializeField] private Image blocksProgressFill;
    [SerializeField] private Button startBattleButton;
    [SerializeField] private TextMeshProUGUI startButtonLabel;
    [SerializeField] private Button restartButton;

    private bool subscribed = false;

    private void Start()
    {
        if (startBattleButton != null) startBattleButton.onClick.AddListener(OnStartClicked);
        if (restartButton != null) restartButton.onClick.AddListener(OnRestartClicked);
        TrySubscribe();
        FullRefresh();
    }

    private void OnDisable()
    {
        Unsubscribe();
    }

    private void Update()
    {
        if (!subscribed) TrySubscribe();
        if (LevelManager.Instance == null) return;
        UpdateStartButtonState();
    }

    private void TrySubscribe()
    {
        if (subscribed) return;
        if (LevelManager.Instance == null) return;
        LevelManager.Instance.OnPhaseChanged += OnPhaseChanged;
        LevelManager.Instance.OnBlockProgress += OnBlockProgress;
        subscribed = true;
        FullRefresh();
    }

    private void Unsubscribe()
    {
        if (!subscribed) return;
        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.OnPhaseChanged -= OnPhaseChanged;
            LevelManager.Instance.OnBlockProgress -= OnBlockProgress;
        }
        subscribed = false;
    }

    private void FullRefresh()
    {
        if (LevelManager.Instance == null) return;

        if (levelText != null) levelText.text = "LEVEL " + LevelManager.Instance.CurrentLevelNumber;

        OnPhaseChanged(LevelManager.Instance.Phase);

        int destroyed = LevelManager.Instance.GetBlocksDestroyed();
        int total = LevelManager.Instance.GetBlocksTotal();
        OnBlockProgress(destroyed, total);
    }

    private void OnPhaseChanged(LevelPhase phase)
    {
        if (phaseText != null)
        {
            switch (phase)
            {
                case LevelPhase.Setup: phaseText.text = "BUY & MERGE PICKAXES"; break;
                case LevelPhase.Battle: phaseText.text = "BATTLE!"; break;
                case LevelPhase.Victory: phaseText.text = "VICTORY"; break;
            }
        }

        if (startBattleButton != null)
        {
            bool show = phase == LevelPhase.Setup;
            startBattleButton.gameObject.SetActive(show);
            if (show && startButtonLabel != null) startButtonLabel.text = "START!";
        }
    }

    private void OnBlockProgress(int destroyed, int total)
    {
        if (total <= 0 && LevelManager.Instance != null) total = LevelManager.Instance.GetBlocksTotal();

        if (blocksProgressText != null) blocksProgressText.text = destroyed + "/" + total;
        if (blocksProgressFill != null)
        {
            float ratio = total > 0 ? (float)destroyed / total : 0f;
            blocksProgressFill.DOKill();
            blocksProgressFill.DOFillAmount(ratio, 0.25f).SetEase(Ease.OutQuad);
        }
    }

    private void UpdateStartButtonState()
    {
        if (startBattleButton == null) return;
        if (!startBattleButton.gameObject.activeSelf) return;
        if (LevelManager.Instance.Phase != LevelPhase.Setup) return;
        if (PickaxeGridManager.Instance == null) return;
        bool can = PickaxeGridManager.Instance.CountAlive() > 0;
        if (startBattleButton.interactable != can) startBattleButton.interactable = can;
    }

    private void OnStartClicked()
    {
        if (LevelManager.Instance == null) return;
        if (PickaxeGridManager.Instance == null || PickaxeGridManager.Instance.CountAlive() == 0) return;
        LevelManager.Instance.StartBattle();
    }

    private void OnRestartClicked()
    {
        if (LevelManager.Instance == null) return;
        LevelManager.Instance.RestartLevel();
    }
}
