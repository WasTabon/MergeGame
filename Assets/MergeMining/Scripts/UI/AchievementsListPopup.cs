using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class AchievementsListPopup : BasePopup
{
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI summaryText;
    [SerializeField] private RectTransform contentContainer;
    [SerializeField] private AchievementListItem itemTemplate;
    [SerializeField] private Button closeButton;

    private List<AchievementListItem> spawnedItems = new List<AchievementListItem>();
    private bool builtOnce = false;

    protected override void Awake()
    {
        base.Awake();
        if (closeButton != null) closeButton.onClick.AddListener(Hide);
    }

    protected override void OnShow()
    {
        BuildList();
    }

    private void BuildList()
    {
        AchievementData config = AchievementConfigProvider.Config;
        if (config == null) return;

        foreach (var item in spawnedItems) if (item != null) Destroy(item.gameObject);
        spawnedItems.Clear();

        int claimedCount = 0;
        int totalCount = config.achievements.Count;

        foreach (var def in config.achievements)
        {
            AchievementListItem item = Instantiate(itemTemplate, contentContainer);
            item.gameObject.SetActive(true);

            bool claimed = AchievementManager.Instance != null && AchievementManager.Instance.IsClaimed(def.id);
            int progress = AchievementManager.Instance != null ? AchievementManager.Instance.GetProgress(def.condition) : 0;
            item.Bind(def, progress, claimed);

            if (claimed) claimedCount++;
            spawnedItems.Add(item);
        }

        if (summaryText != null) summaryText.text = claimedCount + "/" + totalCount + " UNLOCKED";

        builtOnce = true;
    }
}
