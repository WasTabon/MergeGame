using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class ModifierChoicePopup : BasePopup
{
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private List<ModifierChoiceCard> cards;
    [SerializeField] private Button skipButton;

    private List<LevelModifierDefinition> currentOptions = new List<LevelModifierDefinition>();

    protected override void Awake()
    {
        base.Awake();
        for (int i = 0; i < cards.Count; i++)
        {
            int idx = i;
            cards[i].button.onClick.AddListener(() => OnPick(idx));
        }
        if (skipButton != null) skipButton.onClick.AddListener(OnSkip);
    }

    public void ShowChoice()
    {
        currentOptions = PickThree();
        for (int i = 0; i < cards.Count; i++)
        {
            if (i < currentOptions.Count)
            {
                var def = currentOptions[i];
                if (cards[i].titleText != null) cards[i].titleText.text = def.title;
                if (cards[i].descriptionText != null) cards[i].descriptionText.text = def.description;
                if (cards[i].colorBackground != null) cards[i].colorBackground.color = def.iconColor;
                cards[i].gameObject.SetActive(true);
            }
            else cards[i].gameObject.SetActive(false);
        }
        Show();
    }

    private List<LevelModifierDefinition> PickThree()
    {
        List<LevelModifierDefinition> pool = new List<LevelModifierDefinition>(LevelModifierCatalog.All);
        List<LevelModifierDefinition> result = new List<LevelModifierDefinition>();
        for (int i = 0; i < 3 && pool.Count > 0; i++)
        {
            int idx = Random.Range(0, pool.Count);
            result.Add(pool[idx]);
            pool.RemoveAt(idx);
        }
        return result;
    }

    private void OnPick(int cardIdx)
    {
        if (cardIdx >= currentOptions.Count) return;
        LevelModifierDefinition picked = currentOptions[cardIdx];
        if (LevelManager.Instance != null) LevelManager.Instance.ApplyModifier(picked);
        Hide();
    }

    private void OnSkip()
    {
        Hide();
    }
}
