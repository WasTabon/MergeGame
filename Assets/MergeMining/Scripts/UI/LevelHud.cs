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
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private Image timerFill;

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
        LevelManager.Instance.OnTimerTick += OnTimerTick;
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
            LevelManager.Instance.OnTimerTick -= OnTimerTick;
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
        OnTimerTick(LevelManager.Instance.TimeRemaining, LevelManager.Instance.GetTimeLimit());
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
                case LevelPhase.Defeat: phaseText.text = "DEFEAT"; break;
            }
        }

        if (startBattleButton != null)
        {
            bool show = phase == LevelPhase.Setup;
            startBattleButton.gameObject.SetActive(show);
            if (show && startButtonLabel != null) startButtonLabel.text = "START!";
        }

        if (timerText != null) timerText.gameObject.SetActive(phase == LevelPhase.Battle);
        if (timerFill != null) timerFill.transform.parent.gameObject.SetActive(phase == LevelPhase.Battle);
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

    private void OnTimerTick(float remaining, float total)
    {
        if (timerText != null)
        {
            int seconds = Mathf.CeilToInt(remaining);
            timerText.text = string.Format("{0:00}:{1:00}", seconds / 60, seconds % 60);
            timerText.color = remaining < 10f ? new Color(1f, 0.4f, 0.4f) : Color.white;
        }
        if (timerFill != null)
        {
            float ratio = total > 0f ? remaining / total : 0f;
            timerFill.fillAmount = ratio;
            timerFill.color = remaining < 10f ? new Color(0.95f, 0.3f, 0.3f) : new Color(0.4f, 0.78f, 0.4f);
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
