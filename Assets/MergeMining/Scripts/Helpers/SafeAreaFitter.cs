using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class SafeAreaFitter : MonoBehaviour
{
    [SerializeField] private bool applyTop = true;
    [SerializeField] private bool applyBottom = true;
    [SerializeField] private bool applyLeft = true;
    [SerializeField] private bool applyRight = true;

    private RectTransform rectTransform;
    private Rect lastSafeArea;
    private Vector2Int lastScreenSize;
    private ScreenOrientation lastOrientation;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        Apply();
    }

    private void OnEnable()
    {
        Apply();
    }

    private void Update()
    {
        if (Screen.safeArea != lastSafeArea ||
            Screen.width != lastScreenSize.x ||
            Screen.height != lastScreenSize.y ||
            Screen.orientation != lastOrientation)
        {
            Apply();
        }
    }

    private void Apply()
    {
        if (rectTransform == null) return;

        Rect safe = Screen.safeArea;
        lastSafeArea = safe;
        lastScreenSize = new Vector2Int(Screen.width, Screen.height);
        lastOrientation = Screen.orientation;

        Vector2 anchorMin = safe.position;
        Vector2 anchorMax = safe.position + safe.size;

        anchorMin.x /= Screen.width;
        anchorMin.y /= Screen.height;
        anchorMax.x /= Screen.width;
        anchorMax.y /= Screen.height;

        if (!applyLeft) anchorMin.x = 0f;
        if (!applyRight) anchorMax.x = 1f;
        if (!applyBottom) anchorMin.y = 0f;
        if (!applyTop) anchorMax.y = 1f;

        rectTransform.anchorMin = anchorMin;
        rectTransform.anchorMax = anchorMax;
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;
    }
}
