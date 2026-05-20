#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using UnityEditor;

public static class UIBuildUtils
{
    public static readonly Color BG_DARK = new Color(0.102f, 0.102f, 0.18f, 1f);
    public static readonly Color PRIMARY = new Color(0.29f, 0.565f, 0.886f, 1f);
    public static readonly Color ACCENT = new Color(0.961f, 0.651f, 0.137f, 1f);
    public static readonly Color WHITE = Color.white;
    public static readonly Color GREEN = new Color(0.4f, 0.78f, 0.4f, 1f);
    public static readonly Color RED = new Color(0.86f, 0.34f, 0.34f, 1f);
    public static readonly Color SLOT_BG = new Color(1f, 1f, 1f, 0.08f);
    public static readonly Color SLOT_BORDER = new Color(1f, 1f, 1f, 0.2f);

    public static GameObject CreateCanvas(string name, int sortingOrder = 0)
    {
        GameObject go = new GameObject(name);
        Canvas canvas = go.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = sortingOrder;

        CanvasScaler scaler = go.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1080, 1920);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 0.5f;

        go.AddComponent<GraphicRaycaster>();
        return go;
    }

    public static GameObject CreateEventSystemIfNeeded()
    {
        EventSystem existing = Object.FindObjectOfType<EventSystem>();
        if (existing != null) return existing.gameObject;
        GameObject go = new GameObject("EventSystem");
        go.AddComponent<EventSystem>();
        go.AddComponent<StandaloneInputModule>();
        return go;
    }

    public static GameObject CreatePanel(string name, Transform parent, Color color)
    {
        GameObject go = new GameObject(name);
        RectTransform rt = go.AddComponent<RectTransform>();
        rt.SetParent(parent, false);
        Image img = go.AddComponent<Image>();
        img.color = color;
        return go;
    }

    public static GameObject CreateImage(string name, Transform parent, Color color, Sprite sprite = null)
    {
        GameObject go = new GameObject(name);
        RectTransform rt = go.AddComponent<RectTransform>();
        rt.SetParent(parent, false);
        Image img = go.AddComponent<Image>();
        img.color = color;
        if (sprite != null)
        {
            img.sprite = sprite;
            img.type = Image.Type.Sliced;
        }
        return go;
    }

    public static GameObject CreateText(string name, Transform parent, string text, int fontSize, Color color, TextAlignmentOptions alignment = TextAlignmentOptions.Center)
    {
        GameObject go = new GameObject(name);
        RectTransform rt = go.AddComponent<RectTransform>();
        rt.SetParent(parent, false);
        TextMeshProUGUI tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = fontSize;
        tmp.color = color;
        tmp.alignment = alignment;
        tmp.outlineWidth = 0.2f;
        tmp.outlineColor = new Color(0f, 0f, 0f, 0.8f);
        tmp.raycastTarget = false;
        return go;
    }

    public static GameObject CreateButton(string name, Transform parent, string label, Color bgColor, Color textColor, int fontSize = 60)
    {
        GameObject go = new GameObject(name);
        RectTransform rt = go.AddComponent<RectTransform>();
        rt.SetParent(parent, false);
        Image img = go.AddComponent<Image>();
        img.color = bgColor;
        img.sprite = GetUISprite();
        img.type = Image.Type.Sliced;
        Button btn = go.AddComponent<Button>();
        ColorBlock cb = btn.colors;
        cb.normalColor = Color.white;
        cb.highlightedColor = Color.white;
        cb.pressedColor = new Color(0.9f, 0.9f, 0.9f, 1f);
        cb.selectedColor = Color.white;
        cb.disabledColor = new Color(1f, 1f, 1f, 0.5f);
        btn.colors = cb;
        go.AddComponent<ButtonAnimator>();

        if (!string.IsNullOrEmpty(label))
        {
            GameObject labelGo = CreateText("Label", go.transform, label, fontSize, textColor);
            RectTransform lrt = labelGo.GetComponent<RectTransform>();
            lrt.anchorMin = Vector2.zero;
            lrt.anchorMax = Vector2.one;
            lrt.offsetMin = Vector2.zero;
            lrt.offsetMax = Vector2.zero;
        }
        return go;
    }

    public static GameObject CreateIconButton(string name, Transform parent, Color bgColor, string iconChar, int iconSize = 48)
    {
        GameObject go = CreateButton(name, parent, iconChar, bgColor, Color.white, iconSize);
        return go;
    }

    public static Sprite GetUISprite()
    {
        return AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
    }

    public static Sprite GetKnobSprite()
    {
        return AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Knob.psd");
    }

    public static Sprite GetBackgroundSprite()
    {
        return AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Background.psd");
    }

    public static void SetRect(GameObject go, Vector2 anchorMin, Vector2 anchorMax, Vector2 offsetMin, Vector2 offsetMax)
    {
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchorMin = anchorMin;
        rt.anchorMax = anchorMax;
        rt.offsetMin = offsetMin;
        rt.offsetMax = offsetMax;
    }

    public static void SetSize(GameObject go, Vector2 anchor, Vector2 pivot, Vector2 anchoredPos, Vector2 size)
    {
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchorMin = anchor;
        rt.anchorMax = anchor;
        rt.pivot = pivot;
        rt.anchoredPosition = anchoredPos;
        rt.sizeDelta = size;
    }

    public static GameObject CreateToggle(string name, Transform parent, string label, bool initialValue)
    {
        GameObject go = new GameObject(name);
        RectTransform rt = go.AddComponent<RectTransform>();
        rt.SetParent(parent, false);

        Toggle toggle = go.AddComponent<Toggle>();
        toggle.isOn = initialValue;

        GameObject bg = CreateImage("Background", go.transform, new Color(0.2f, 0.2f, 0.3f, 1f), GetUISprite());
        SetSize(bg, new Vector2(0f, 0.5f), new Vector2(0f, 0.5f), new Vector2(60f, 0f), new Vector2(80f, 80f));

        GameObject check = CreateImage("Checkmark", bg.transform, GREEN, GetUISprite());
        SetRect(check, new Vector2(0.1f, 0.1f), new Vector2(0.9f, 0.9f), Vector2.zero, Vector2.zero);

        toggle.graphic = check.GetComponent<Image>();
        toggle.targetGraphic = bg.GetComponent<Image>();

        GameObject labelGo = CreateText("Label", go.transform, label, 44, WHITE, TextAlignmentOptions.MidlineLeft);
        SetRect(labelGo, new Vector2(0f, 0f), new Vector2(1f, 1f), new Vector2(160f, 0f), new Vector2(0f, 0f));

        return go;
    }

    public static GameObject CreateSlider(string name, Transform parent, float initialValue)
    {
        GameObject go = new GameObject(name);
        RectTransform rt = go.AddComponent<RectTransform>();
        rt.SetParent(parent, false);

        Slider slider = go.AddComponent<Slider>();
        slider.minValue = 0f;
        slider.maxValue = 1f;

        GameObject bg = CreateImage("Background", go.transform, new Color(0.15f, 0.15f, 0.2f, 1f), GetBackgroundSprite());
        SetRect(bg, new Vector2(0f, 0.25f), new Vector2(1f, 0.75f), Vector2.zero, Vector2.zero);

        GameObject fillArea = new GameObject("Fill Area");
        RectTransform farRt = fillArea.AddComponent<RectTransform>();
        farRt.SetParent(go.transform, false);
        farRt.anchorMin = new Vector2(0f, 0.25f);
        farRt.anchorMax = new Vector2(1f, 0.75f);
        farRt.offsetMin = new Vector2(10f, 0f);
        farRt.offsetMax = new Vector2(-10f, 0f);

        GameObject fill = CreateImage("Fill", fillArea.transform, PRIMARY, GetUISprite());
        SetRect(fill, new Vector2(0f, 0f), new Vector2(1f, 1f), Vector2.zero, Vector2.zero);

        GameObject handleArea = new GameObject("Handle Slide Area");
        RectTransform haRt = handleArea.AddComponent<RectTransform>();
        haRt.SetParent(go.transform, false);
        haRt.anchorMin = new Vector2(0f, 0f);
        haRt.anchorMax = new Vector2(1f, 1f);
        haRt.offsetMin = new Vector2(10f, 0f);
        haRt.offsetMax = new Vector2(-10f, 0f);

        GameObject handle = CreateImage("Handle", handleArea.transform, WHITE, GetKnobSprite());
        RectTransform handleRt = handle.GetComponent<RectTransform>();
        handleRt.sizeDelta = new Vector2(50f, 50f);

        slider.fillRect = fill.GetComponent<RectTransform>();
        slider.handleRect = handleRt;
        slider.targetGraphic = handle.GetComponent<Image>();
        slider.direction = Slider.Direction.LeftToRight;
        slider.value = initialValue;

        return go;
    }
}
#endif
