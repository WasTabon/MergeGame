using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelSelectController : MonoBehaviour
{
    [SerializeField] private RectTransform pathContainer;
    [SerializeField] private LevelSelectNodeView nodeTemplate;
    [SerializeField] private Button backButton;
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private float nodeSpacing = 220f;
    [SerializeField] private float horizontalAmplitude = 200f;

    private const string MAX_PASSED_LEVEL_KEY = "max_passed_level";
    private const string CURRENT_LEVEL_KEY = "current_level";
    private const string PENDING_LEVEL_KEY = "pending_level_to_play";

    private List<LevelSelectNodeView> nodes = new List<LevelSelectNodeView>();

    private void Start()
    {
        if (backButton != null) backButton.onClick.AddListener(OnBack);
        BuildLevels();
        ScrollToCurrent();
    }

    private void BuildLevels()
    {
        if (LevelConfigProvider.Config == null) return;
        int total = LevelConfigProvider.Config.Count;
        int maxPassed = PlayerPrefs.GetInt(MAX_PASSED_LEVEL_KEY, 0);
        int current = Mathf.Max(1, PlayerPrefs.GetInt(CURRENT_LEVEL_KEY, 1));

        if (pathContainer != null)
        {
            pathContainer.sizeDelta = new Vector2(pathContainer.sizeDelta.x, total * nodeSpacing + 200f);
        }

        for (int i = 0; i < total; i++)
        {
            int levelNum = i + 1;
            LevelSelectNodeView node = Instantiate(nodeTemplate, pathContainer);
            node.gameObject.SetActive(true);

            RectTransform rt = node.transform as RectTransform;
            rt.anchorMin = new Vector2(0.5f, 0f);
            rt.anchorMax = new Vector2(0.5f, 0f);
            rt.pivot = new Vector2(0.5f, 0.5f);
            float xWave = Mathf.Sin(i * 0.7f) * horizontalAmplitude;
            rt.anchoredPosition = new Vector2(xWave, 150f + i * nodeSpacing);

            int stars = PlayerPrefs.GetInt("level_stars_" + levelNum, 0);
            bool unlocked = levelNum <= maxPassed + 1;
            bool isCurrent = levelNum == current;

            node.Bind(levelNum, unlocked, stars, isCurrent);
            int captured = levelNum;
            node.button.onClick.AddListener(() => OnLevelTapped(captured));
            nodes.Add(node);
        }
    }

    private void ScrollToCurrent()
    {
        if (scrollRect == null || pathContainer == null) return;
        int current = Mathf.Max(1, PlayerPrefs.GetInt(CURRENT_LEVEL_KEY, 1));
        int idx = Mathf.Clamp(current - 1, 0, nodes.Count - 1);
        float targetY = 150f + idx * nodeSpacing;
        float totalH = pathContainer.sizeDelta.y;
        float viewportH = scrollRect.viewport != null ? scrollRect.viewport.rect.height : Screen.height;
        float scrollable = Mathf.Max(1f, totalH - viewportH);
        float normalizedFromBottom = (targetY - viewportH * 0.5f) / scrollable;
        scrollRect.verticalNormalizedPosition = Mathf.Clamp01(normalizedFromBottom);
    }

    private void OnLevelTapped(int levelNum)
    {
        PlayerPrefs.SetInt(PENDING_LEVEL_KEY, levelNum);
        if (TransitionManager.Instance != null) TransitionManager.Instance.LoadScene("Game");
    }

    private void OnBack()
    {
        if (TransitionManager.Instance != null) TransitionManager.Instance.LoadScene("MainMenu");
    }
}
