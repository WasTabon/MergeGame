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

    private void OnEnable()
    {
        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.OnPhaseChanged -= OnPhaseChanged;
            LevelManager.Instance.OnPhaseChanged += OnPhaseChanged;
            LevelManager.Instance.OnBlockProgress -= OnBlockProgress;
            LevelManager.Instance.OnBlockProgress += OnBlockProgress;
            Refresh();
        }
    }

    private void OnDisable()
    {
        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.OnPhaseChanged -= OnPhaseChanged;
            LevelManager.Instance.OnBlockProgress -= OnBlockProgress;
        }
    }

    private void Start()
    {
        if (startBattleButton != null) startBattleButton.onClick.AddListener(OnStartClicked);
        if (restartButton != null) restartButton.onClick.AddListener(OnRestartClicked);
        Refresh();
    }

    private void Refresh()
    {
        if (LevelManager.Instance == null) return;
        if (levelText != null) levelText.text = "LEVEL " + LevelManager.Instance.CurrentLevelNumber;
        OnPhaseChanged(LevelManager.Instance.Phase);
        OnBlockProgress(LevelManager.Instance.GetBlocksDestroyed(), LevelManager.Instance.GetBlocksTotal());
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
            if (show && PickaxeGridManager.Instance != null)
            {
                startBattleButton.interactable = PickaxeGridManager.Instance.CountAlive() > 0;
            }
        }
    }

    private void OnBlockProgress(int destroyed, int total)
    {
        if (blocksProgressText != null) blocksProgressText.text = destroyed + "/" + total;
        if (blocksProgressFill != null)
        {
            float ratio = total > 0 ? (float)destroyed / total : 0f;
            blocksProgressFill.DOKill();
            blocksProgressFill.DOFillAmount(ratio, 0.25f).SetEase(Ease.OutQuad);
        }
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

    private void Update()
    {
        if (LevelManager.Instance != null && LevelManager.Instance.Phase == LevelPhase.Setup && startBattleButton != null && PickaxeGridManager.Instance != null)
        {
            bool can = PickaxeGridManager.Instance.CountAlive() > 0;
            if (startBattleButton.interactable != can) startBattleButton.interactable = can;
        }
    }
}
