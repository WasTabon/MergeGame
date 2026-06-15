using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelSelectNodeView : MonoBehaviour
{
    public int levelNumber;
    public Button button;
    public Image circleBg;
    public Image lockIcon;
    public TextMeshProUGUI numberText;
    public Image star1Icon;
    public Image star2Icon;
    public Image star3Icon;
    public Image currentMarker;

    public void Bind(int level, bool unlocked, int stars, bool isCurrent)
    {
        levelNumber = level;
        if (numberText != null)
        {
            numberText.gameObject.SetActive(unlocked);
            numberText.text = level.ToString();
        }
        if (lockIcon != null) lockIcon.gameObject.SetActive(!unlocked);

        if (button != null) button.interactable = unlocked;

        Color completed = new Color(1f,1f,1f);
        Color current = new Color(0.95f, 0.78f, 0.25f);
        Color unlockedColor = new Color(1f,1f,1f);
        Color locked = new Color(0.3f, 0.3f, 0.35f);

        if (circleBg != null)
        {
            if (!unlocked) circleBg.color = locked;
            else if (stars > 0) circleBg.color = completed;
            else if (isCurrent) circleBg.color = current;
            else circleBg.color = unlockedColor;
        }

        if (star1Icon != null) star1Icon.gameObject.SetActive(unlocked);
        if (star2Icon != null) star2Icon.gameObject.SetActive(unlocked);
        if (star3Icon != null) star3Icon.gameObject.SetActive(unlocked);

        Color starOn = new Color(1f, 0.85f, 0.25f);
        Color starOff = new Color(0f, 0f, 0f, 0.5f);
        if (star1Icon != null) star1Icon.color = stars >= 1 ? starOn : starOff;
        if (star2Icon != null) star2Icon.color = stars >= 2 ? starOn : starOff;
        if (star3Icon != null) star3Icon.color = stars >= 3 ? starOn : starOff;

        if (currentMarker != null) currentMarker.gameObject.SetActive(isCurrent && stars == 0);
    }
}
