using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AchievementListItem : MonoBehaviour
{
    public Image iconBg;
    public Image trophyIcon;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI progressText;
    public Image progressFill;
    public Image gemIcon;
    public TextMeshProUGUI rewardText;
    public GameObject checkmark;

    public void Bind(AchievementDefinition def, int progress, bool claimed)
    {
        if (titleText != null) titleText.text = def.title;
        if (descriptionText != null) descriptionText.text = def.description;

        int shown = claimed ? def.target : Mathf.Min(progress, def.target);
        if (progressText != null) progressText.text = shown + "/" + def.target;

        float ratio = def.target > 0 ? Mathf.Clamp01((float)shown / def.target) : 0f;
        if (progressFill != null) progressFill.fillAmount = ratio;

        if (rewardText != null) rewardText.text = "+" + def.gemsReward;

        Color goldUnclaimed = new Color(0.55f, 0.45f, 0.25f);
        Color goldClaimed = new Color(1f, 0.78f, 0.25f);
        Color gray = new Color(0.4f, 0.4f, 0.45f);
        if (iconBg != null) iconBg.color = claimed ? goldClaimed : (ratio >= 1f ? goldUnclaimed : gray);
        if (trophyIcon != null) trophyIcon.color = claimed ? new Color(1f, 0.95f, 0.7f) : new Color(0.7f, 0.7f, 0.75f);
        if (checkmark != null) checkmark.SetActive(claimed);

        if (progressFill != null)
        {
            Color fillCol;
            if (claimed) fillCol = new Color(0.4f, 0.78f, 0.4f);
            else if (ratio >= 1f) fillCol = new Color(0.95f, 0.78f, 0.25f);
            else fillCol = new Color(0.55f, 0.565f, 0.886f);
            progressFill.color = fillCol;
        }
    }
}
