using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class LevelVictoryPopup : BasePopup
{
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private Image star1Icon;
    [SerializeField] private Image star2Icon;
    [SerializeField] private Image star3Icon;
    [SerializeField] private Sprite starSpriteOverride;
    [SerializeField] private TextMeshProUGUI gemsRewardText;
    [SerializeField] private RectTransform gemIcon;
    [SerializeField] private Button nextButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button menuButton;

    private int pendingLevel;
    private int pendingStars;
    private int pendingGems;

    protected override void Awake()
    {
        base.Awake();
        if (nextButton != null) nextButton.onClick.AddListener(OnNext);
        if (restartButton != null) restartButton.onClick.AddListener(OnRestart);
        if (menuButton != null) menuButton.onClick.AddListener(OnMenu);

        if (starSpriteOverride != null)
        {
            if (star1Icon != null) star1Icon.sprite = starSpriteOverride;
            if (star2Icon != null) star2Icon.sprite = starSpriteOverride;
            if (star3Icon != null) star3Icon.sprite = starSpriteOverride;
        }
    }

    public void ShowVictory(int level, int stars, int gems)
    {
        pendingLevel = level;
        pendingStars = stars;
        pendingGems = gems;
        if (levelText != null) levelText.text = "LEVEL " + level + " COMPLETE";
        if (gemsRewardText != null) gemsRewardText.text = "+" + gems;

        bool isLast = LevelConfigProvider.Config != null && level >= LevelConfigProvider.Config.Count;
        if (nextButton != null) nextButton.gameObject.SetActive(!isLast);

        Show();
    }

    protected override void OnShow()
    {
        Color lit = new Color(1f, 0.85f, 0.25f);
        Color dim = new Color(0.25f, 0.25f, 0.3f);
        if (star1Icon != null) { star1Icon.color = dim; star1Icon.transform.localScale = Vector3.one; }
        if (star2Icon != null) { star2Icon.color = dim; star2Icon.transform.localScale = Vector3.one; }
        if (star3Icon != null) { star3Icon.color = dim; star3Icon.transform.localScale = Vector3.one; }

        if (SfxLibrary.Instance != null) SfxLibrary.Instance.Play(SfxLibrary.Instance.zoneComplete);

        Sequence s = DOTween.Sequence().SetUpdate(true);
        s.AppendInterval(0.3f);
        if (pendingStars >= 1) s.AppendCallback(() => AnimateStar(star1Icon, lit));
        s.AppendInterval(0.25f);
        if (pendingStars >= 2) s.AppendCallback(() => AnimateStar(star2Icon, lit));
        s.AppendInterval(0.25f);
        if (pendingStars >= 3) s.AppendCallback(() => AnimateStar(star3Icon, lit));

        if (gemIcon != null)
        {
            gemIcon.localScale = Vector3.zero;
            gemIcon.DOScale(1f, 0.4f).SetEase(Ease.OutBack).SetDelay(0.4f).SetUpdate(true);
        }
    }

    private void AnimateStar(Image img, Color lit)
    {
        if (img == null) return;
        img.color = lit;
        img.transform.localScale = Vector3.one * 0.5f;
        img.transform.DOScale(1f, 0.35f).SetEase(Ease.OutBack).SetUpdate(true);
        if (SfxLibrary.Instance != null) SfxLibrary.Instance.Play(SfxLibrary.Instance.coinTick);
    }

    private void OnNext()
    {
        Hide();
        if (LevelManager.Instance != null) LevelManager.Instance.GoToNextLevel();
    }

    private void OnRestart()
    {
        Hide();
        if (LevelManager.Instance != null) LevelManager.Instance.RestartLevel();
    }

    private void OnMenu()
    {
        Hide();
        if (LevelManager.Instance != null) LevelManager.Instance.GoToLevelSelect();
        else if (TransitionManager.Instance != null) TransitionManager.Instance.LoadScene("LevelSelect");
    }
}
