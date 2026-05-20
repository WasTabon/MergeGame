using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;

public class TransitionManager : MonoBehaviour
{
    public static TransitionManager Instance { get; private set; }

    [SerializeField] private float fadeDuration = 0.35f;

    private Canvas canvas;
    private CanvasGroup canvasGroup;
    private Image fadeImage;
    private bool isTransitioning = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        CreateFadeCanvas();
    }

    private void CreateFadeCanvas()
    {
        GameObject canvasGo = new GameObject("FadeCanvas");
        canvasGo.transform.SetParent(transform);

        canvas = canvasGo.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 9999;

        canvasGo.AddComponent<CanvasScaler>();
        canvasGo.AddComponent<GraphicRaycaster>();

        canvasGroup = canvasGo.AddComponent<CanvasGroup>();

        GameObject imgGo = new GameObject("FadeImage");
        imgGo.transform.SetParent(canvasGo.transform, false);

        fadeImage = imgGo.AddComponent<Image>();
        fadeImage.color = Color.black;
        fadeImage.raycastTarget = true;

        RectTransform rt = fadeImage.rectTransform;
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;

        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;
    }

    public void LoadScene(string sceneName)
    {
        if (isTransitioning) return;
        StartCoroutine(LoadSceneRoutine(sceneName));
    }

    private IEnumerator LoadSceneRoutine(string sceneName)
    {
        isTransitioning = true;
        canvasGroup.blocksRaycasts = true;

        yield return canvasGroup.DOFade(1f, fadeDuration).SetEase(Ease.InQuad).WaitForCompletion();

        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
        while (!op.isDone) yield return null;

        yield return canvasGroup.DOFade(0f, fadeDuration).SetEase(Ease.OutQuad).WaitForCompletion();

        canvasGroup.blocksRaycasts = false;
        isTransitioning = false;
    }
}
