using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class ChestOpeningPopup : BasePopup
{
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private RectTransform chestVisual;
    [SerializeField] private Image chestBodyImage;
    [SerializeField] private Image chestLidImage;
    [SerializeField] private Image flashImage;
    [SerializeField] private RectTransform rewardsContainer;
    [SerializeField] private GameObject rewardItemTemplate;
    [SerializeField] private Button continueButton;

    private List<GameObject> spawnedItems = new List<GameObject>();

    protected override void Awake()
    {
        base.Awake();
        continueButton.onClick.AddListener(OnContinue);
    }

    public void ShowChest(ChestTypeData chestData, List<GeneratedChestReward> rewards)
    {
        ClearOldItems();
        if (titleText != null) titleText.text = chestData.displayName.ToUpper();
        if (chestBodyImage != null) chestBodyImage.color = chestData.chestColor;
        if (chestLidImage != null) chestLidImage.color = chestData.accentColor;

        Show();
        PlayOpeningSequence(chestData, rewards);
    }

    private void ClearOldItems()
    {
        foreach (var go in spawnedItems) if (go != null) Destroy(go);
        spawnedItems.Clear();
    }

    protected override void OnShow()
    {
        if (continueButton != null) continueButton.gameObject.SetActive(false);
        if (chestVisual != null) chestVisual.localScale = Vector3.zero;
        if (rewardsContainer != null) rewardsContainer.gameObject.SetActive(false);
        if (flashImage != null)
        {
            Color c = flashImage.color; c.a = 0f; flashImage.color = c;
        }
    }

    private void PlayOpeningSequence(ChestTypeData chestData, List<GeneratedChestReward> rewards)
    {
        Sequence s = DOTween.Sequence().SetUpdate(true);

        s.AppendInterval(0.2f);
        s.Append(chestVisual.DOScale(1f, 0.45f).SetEase(Ease.OutBack));

        s.AppendInterval(0.3f);
        s.Append(chestVisual.DOShakePosition(0.4f, new Vector3(15f, 0f, 0f), 18, 90f).SetUpdate(true));
        s.Join(chestVisual.DOPunchScale(Vector3.one * 0.12f, 0.4f, 6, 0.5f).SetUpdate(true));

        s.AppendCallback(() =>
        {
            if (HapticManager.Instance != null) HapticManager.Instance.Medium();
        });

        s.AppendInterval(0.1f);
        s.Append(chestLidImage.rectTransform.DOAnchorPosY(120f, 0.3f).SetEase(Ease.OutBack).SetUpdate(true));
        s.Join(chestLidImage.rectTransform.DOLocalRotate(new Vector3(0f, 0f, 25f), 0.3f).SetUpdate(true));

        s.AppendCallback(() =>
        {
            if (flashImage != null)
            {
                flashImage.color = new Color(1f, 1f, 1f, 0.9f);
                flashImage.transform.localScale = Vector3.one * 0.3f;
                flashImage.transform.DOScale(2.5f, 0.5f).SetEase(Ease.OutQuad).SetUpdate(true);
                flashImage.DOFade(0f, 0.5f).SetEase(Ease.OutQuad).SetUpdate(true);
            }
            if (HapticManager.Instance != null) HapticManager.Instance.Heavy();
        });

        s.AppendInterval(0.3f);
        s.AppendCallback(() => SpawnRewards(rewards));
        s.AppendInterval(0.6f + rewards.Count * 0.12f);
        s.AppendCallback(() =>
        {
            if (continueButton != null)
            {
                continueButton.gameObject.SetActive(true);
                continueButton.transform.localScale = Vector3.zero;
                continueButton.transform.DOScale(1f, 0.3f).SetEase(Ease.OutBack).SetUpdate(true);
            }
        });
    }

    private void SpawnRewards(List<GeneratedChestReward> rewards)
    {
        if (rewardsContainer == null || rewardItemTemplate == null) return;
        rewardsContainer.gameObject.SetActive(true);

        for (int i = 0; i < rewards.Count; i++)
        {
            GameObject item = Instantiate(rewardItemTemplate, rewardsContainer);
            item.SetActive(true);
            spawnedItems.Add(item);

            Transform iconT = item.transform.Find("Icon");
            Transform amountT = item.transform.Find("Amount");
            if (iconT != null)
            {
                Image img = iconT.GetComponent<Image>();
                if (img != null) img.color = rewards[i].color;
            }
            if (amountT != null)
            {
                TextMeshProUGUI tmp = amountT.GetComponent<TextMeshProUGUI>();
                if (tmp != null) tmp.text = FormatRewardLabel(rewards[i]);
            }

            item.transform.localScale = Vector3.zero;
            item.transform.DOScale(1f, 0.35f).SetEase(Ease.OutBack).SetDelay(i * 0.12f).SetUpdate(true);
        }
    }

    private string FormatRewardLabel(GeneratedChestReward r)
    {
        if (r.id == "coins") return "+" + r.amount;
        if (r.id == "gems") return "+" + r.amount;
        if (r.id.StartsWith("pickaxe_lvl"))
        {
            string lvl = r.id.Substring("pickaxe_lvl".Length);
            return "LV" + lvl;
        }
        return r.amount.ToString();
    }

    private void OnContinue() { Hide(); }
}
