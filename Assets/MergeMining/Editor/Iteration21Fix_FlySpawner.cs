#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.SceneManagement;

public static class Iteration21Fix_FlySpawner
{
    private const string GAME_SCENE_PATH = "Assets/MergeMining/Scenes/Game.unity";

    [MenuItem("Tools/Merge Mining/(Iteration 21 FIX) Rebuild FlyEffectSpawner")]
    public static void Rebuild()
    {
        EditorSceneManager.OpenScene(GAME_SCENE_PATH);

        GameObject coinFly = BuildCoinFlyTemplate();
        GameObject gemFly = BuildGemFlyTemplate();
        AttachSpawner(coinFly, gemFly);

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
        Debug.Log("FlyEffectSpawner rebuilt and attached to Canvas.");
    }

    private static GameObject BuildCoinFlyTemplate()
    {
        GameObject existing = GameObject.Find("CoinFlyTemplate");
        if (existing != null) Object.DestroyImmediate(existing);

        GameObject root = new GameObject("CoinFlyTemplate");
        RectTransform rt = root.AddComponent<RectTransform>();
        rt.sizeDelta = new Vector2(60f, 60f);

        GameObject icon = UIBuildUtils.CreateImage("Icon", root.transform, new Color(1f, 0.85f, 0.4f), UIBuildUtils.GetKnobSprite());
        UIBuildUtils.SetRect(icon, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
        icon.GetComponent<Image>().raycastTarget = false;

        GameObject inner = UIBuildUtils.CreateImage("Inner", icon.transform, UIColors.ACCENT, UIBuildUtils.GetKnobSprite());
        UIBuildUtils.SetRect(inner, new Vector2(0.25f, 0.25f), new Vector2(0.75f, 0.75f), Vector2.zero, Vector2.zero);
        inner.GetComponent<Image>().raycastTarget = false;

        CurrencyFlyEffect fx = root.AddComponent<CurrencyFlyEffect>();
        SerializedObject so = new SerializedObject(fx);
        so.FindProperty("iconImage").objectReferenceValue = icon.GetComponent<Image>();
        so.FindProperty("duration").floatValue = 0.6f;
        so.ApplyModifiedProperties();

        root.SetActive(false);
        return root;
    }

    private static GameObject BuildGemFlyTemplate()
    {
        GameObject existing = GameObject.Find("GemFlyTemplate");
        if (existing != null) Object.DestroyImmediate(existing);

        GameObject root = new GameObject("GemFlyTemplate");
        RectTransform rt = root.AddComponent<RectTransform>();
        rt.sizeDelta = new Vector2(70f, 70f);

        GameObject icon = UIBuildUtils.CreateImage("Icon", root.transform, UIColors.GEM, UIBuildUtils.GetKnobSprite());
        UIBuildUtils.SetRect(icon, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
        icon.GetComponent<Image>().raycastTarget = false;

        CurrencyFlyEffect fx = root.AddComponent<CurrencyFlyEffect>();
        SerializedObject so = new SerializedObject(fx);
        so.FindProperty("iconImage").objectReferenceValue = icon.GetComponent<Image>();
        so.FindProperty("duration").floatValue = 0.6f;
        so.ApplyModifiedProperties();

        root.SetActive(false);
        return root;
    }

    private static void AttachSpawner(GameObject coinFly, GameObject gemFly)
    {
        GameObject canvasGo = GameObject.Find("Canvas");
        if (canvasGo == null) { Debug.LogError("Canvas not found!"); return; }

        FlyEffectSpawner spawner = canvasGo.GetComponent<FlyEffectSpawner>();
        if (spawner == null) spawner = canvasGo.AddComponent<FlyEffectSpawner>();

        GameObject dragLayer = GameObject.Find("DragLayer");
        RectTransform layerRt = dragLayer != null ? dragLayer.GetComponent<RectTransform>() : null;

        GameObject coinsIcon = GameObject.Find("CoinsIcon");
        GameObject gemsIcon = GameObject.Find("GemsIcon");

        SerializedObject so = new SerializedObject(spawner);
        so.FindProperty("coinFlyTemplate").objectReferenceValue = coinFly.GetComponent<CurrencyFlyEffect>();
        so.FindProperty("gemFlyTemplate").objectReferenceValue = gemFly.GetComponent<CurrencyFlyEffect>();
        so.FindProperty("layer").objectReferenceValue = layerRt;
        so.FindProperty("coinsTarget").objectReferenceValue = coinsIcon != null ? coinsIcon.GetComponent<RectTransform>() : null;
        so.FindProperty("gemsTarget").objectReferenceValue = gemsIcon != null ? gemsIcon.GetComponent<RectTransform>() : null;
        so.ApplyModifiedProperties();

        Debug.Log("FlyEffectSpawner attached. Templates: coin=" + (coinFly != null) + " gem=" + (gemFly != null) + 
            ", DragLayer=" + (layerRt != null) + ", CoinsIcon=" + (coinsIcon != null) + ", GemsIcon=" + (gemsIcon != null));
    }
}
#endif
