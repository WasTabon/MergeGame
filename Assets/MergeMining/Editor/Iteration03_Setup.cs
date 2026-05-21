#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;

public static class Iteration03_Setup
{
    private const string GAME_SCENE_PATH = "Assets/MergeMining/Scenes/Game.unity";
    private const string CONFIG_DIR = "Assets/MergeMining/Resources";
    private const string BLOCK_CONFIG_PATH = "Assets/MergeMining/Resources/BlockConfig.asset";

    [MenuItem("Tools/Merge Mining/(Iteration 3) Update Game Scene")]
    public static void UpdateGameScene()
    {
        CreateBlockConfigIfMissing();

        if (EditorSceneManager.GetActiveScene().path != GAME_SCENE_PATH)
        {
            EditorSceneManager.OpenScene(GAME_SCENE_PATH);
        }

        GameObject canvasGo = GameObject.Find("Canvas");
        Debug.Assert(canvasGo != null, "Canvas not found!");

        GameObject blocksRow = FindByName(canvasGo.transform, "BlocksRow");
        Debug.Assert(blocksRow != null, "BlocksRow not found!");

        RemoveHintText(blocksRow);
        List<RectTransform> blockSlots = EnsureBlockSlots(blocksRow);

        GameObject blockPrefab = EnsureBlockPrefabRoot();
        GameObject miningAttackPrefab = EnsureMiningAttackPrefabRoot();
        GameObject blockDestroyPrefab = EnsureBlockDestroyEffectPrefabRoot();
        GameObject coinBurstPrefab = EnsureCoinBurstPrefabRoot();

        EnsureMiningBehaviourOnPickaxeTemplate();
        EnsureMiningBehaviourOnExistingPickaxes();

        AttachBlocksRowManager(blocksRow, blockSlots, blockPrefab, blockDestroyPrefab, coinBurstPrefab);
        UpdatePickaxeGridManagerMiningTemplate(miningAttackPrefab);

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());

        Debug.Log("Iteration 3 setup complete.");
    }

    private static GameObject FindByName(Transform parent, string name)
    {
        if (parent.name == name) return parent.gameObject;
        foreach (Transform t in parent)
        {
            GameObject res = FindByName(t, name);
            if (res != null) return res;
        }
        return null;
    }

    private static void RemoveHintText(GameObject blocksRow)
    {
        Transform hint = blocksRow.transform.Find("HintText");
        if (hint != null) Object.DestroyImmediate(hint.gameObject);
    }

    private static List<RectTransform> EnsureBlockSlots(GameObject blocksRow)
    {
        List<RectTransform> result = new List<RectTransform>();

        RectTransform rowRt = blocksRow.transform as RectTransform;
        rowRt.sizeDelta = new Vector2(1000f, 320f);

        int count = 4;
        float slotW = 220f;
        float slotH = 300f;
        float spacing = 12f;
        float totalW = count * slotW + (count - 1) * spacing;
        float startX = -totalW * 0.5f + slotW * 0.5f;

        for (int i = 0; i < count; i++)
        {
            string slotName = "BlockSlot_" + i;
            Transform existing = blocksRow.transform.Find(slotName);
            GameObject slot;
            if (existing != null) slot = existing.gameObject;
            else
            {
                slot = new GameObject(slotName);
                RectTransform srt = slot.AddComponent<RectTransform>();
                srt.SetParent(blocksRow.transform, false);
            }

            RectTransform rt = slot.GetComponent<RectTransform>();
            float x = startX + i * (slotW + spacing);
            rt.anchorMin = new Vector2(0.5f, 0.5f);
            rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = new Vector2(x, 0f);
            rt.sizeDelta = new Vector2(slotW, slotH);

            result.Add(rt);
        }

        return result;
    }

    private static GameObject EnsureBlockPrefabRoot()
    {
        GameObject existing = GameObject.Find("BlockPrefabTemplate");
        if (existing != null) return existing;

        GameObject root = new GameObject("BlockPrefabTemplate");
        RectTransform rt = root.AddComponent<RectTransform>();
        rt.sizeDelta = new Vector2(200f, 260f);

        GameObject body = UIBuildUtils.CreateImage("Body", root.transform, Color.gray, UIBuildUtils.GetUISprite());
        UIBuildUtils.SetRect(body, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
        body.GetComponent<Image>().raycastTarget = true;

        GameObject highlight = UIBuildUtils.CreateImage("Highlight", body.transform, new Color(1f, 1f, 1f, 0.15f), UIBuildUtils.GetUISprite());
        UIBuildUtils.SetRect(highlight, new Vector2(0.1f, 0.6f), new Vector2(0.9f, 0.9f), Vector2.zero, Vector2.zero);
        highlight.GetComponent<Image>().raycastTarget = false;

        GameObject nameGo = UIBuildUtils.CreateText("NameText", body.transform, "STONE", 36, new Color(1f, 1f, 1f, 0.85f));
        UIBuildUtils.SetRect(nameGo, new Vector2(0f, 0f), new Vector2(1f, 0.25f), Vector2.zero, Vector2.zero);
        nameGo.GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Bold;

        GameObject hpBarRoot = new GameObject("HpBar");
        RectTransform hpRt = hpBarRoot.AddComponent<RectTransform>();
        hpRt.SetParent(root.transform, false);
        hpRt.anchorMin = new Vector2(0f, 1f);
        hpRt.anchorMax = new Vector2(1f, 1f);
        hpRt.pivot = new Vector2(0.5f, 1f);
        hpRt.anchoredPosition = new Vector2(0f, 30f);
        hpRt.sizeDelta = new Vector2(-20f, 18f);

        GameObject hpBg = UIBuildUtils.CreateImage("Bg", hpBarRoot.transform, new Color(0f, 0f, 0f, 0.6f), UIBuildUtils.GetUISprite());
        UIBuildUtils.SetRect(hpBg, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);

        GameObject hpFill = UIBuildUtils.CreateImage("Fill", hpBarRoot.transform, new Color(0.4f, 0.85f, 0.4f), UIBuildUtils.GetUISprite());
        UIBuildUtils.SetRect(hpFill, new Vector2(0f, 0f), new Vector2(1f, 1f), new Vector2(3f, 3f), new Vector2(-3f, -3f));
        Image hpFillImg = hpFill.GetComponent<Image>();
        hpFillImg.type = Image.Type.Filled;
        hpFillImg.fillMethod = Image.FillMethod.Horizontal;
        hpFillImg.fillOrigin = (int)Image.OriginHorizontal.Left;
        hpFillImg.fillAmount = 1f;
        hpFillImg.raycastTarget = false;

        BlockHpBar hpBar = hpBarRoot.AddComponent<BlockHpBar>();
        SerializedObject hpSo = new SerializedObject(hpBar);
        hpSo.FindProperty("fillImage").objectReferenceValue = hpFillImg;
        hpSo.FindProperty("bgImage").objectReferenceValue = hpBg.GetComponent<Image>();
        hpSo.ApplyModifiedProperties();

        Block block = root.AddComponent<Block>();
        SerializedObject bso = new SerializedObject(block);
        bso.FindProperty("bodyImage").objectReferenceValue = body.GetComponent<Image>();
        bso.FindProperty("highlightImage").objectReferenceValue = highlight.GetComponent<Image>();
        bso.FindProperty("nameText").objectReferenceValue = nameGo.GetComponent<TextMeshProUGUI>();
        bso.FindProperty("hpBar").objectReferenceValue = hpBar;
        bso.ApplyModifiedProperties();

        root.SetActive(false);
        return root;
    }

    private static GameObject EnsureMiningAttackPrefabRoot()
    {
        GameObject existing = GameObject.Find("MiningAttackTemplate");
        if (existing != null) return existing;

        GameObject root = new GameObject("MiningAttackTemplate");
        RectTransform rt = root.AddComponent<RectTransform>();
        rt.sizeDelta = new Vector2(80f, 80f);

        GameObject trail = UIBuildUtils.CreateImage("Trail", root.transform, new Color(1f, 1f, 1f, 0.5f), UIBuildUtils.GetKnobSprite());
        UIBuildUtils.SetRect(trail, new Vector2(-0.3f, -0.3f), new Vector2(1.3f, 1.3f), Vector2.zero, Vector2.zero);
        trail.GetComponent<Image>().raycastTarget = false;

        GameObject body = UIBuildUtils.CreateImage("Body", root.transform, Color.white, UIBuildUtils.GetUISprite());
        UIBuildUtils.SetRect(body, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
        body.GetComponent<Image>().raycastTarget = false;

        MiningAttack ma = root.AddComponent<MiningAttack>();
        SerializedObject so = new SerializedObject(ma);
        so.FindProperty("bodyImage").objectReferenceValue = body.GetComponent<Image>();
        so.FindProperty("trailImage").objectReferenceValue = trail.GetComponent<Image>();
        so.ApplyModifiedProperties();

        root.SetActive(false);
        return root;
    }

    private static GameObject EnsureBlockDestroyEffectPrefabRoot()
    {
        GameObject existing = GameObject.Find("BlockDestroyEffectTemplate");
        if (existing != null) return existing;

        GameObject root = new GameObject("BlockDestroyEffectTemplate");
        RectTransform rt = root.AddComponent<RectTransform>();
        rt.sizeDelta = new Vector2(280f, 280f);

        GameObject shock = UIBuildUtils.CreateImage("Shockwave", root.transform, Color.white, UIBuildUtils.GetKnobSprite());
        UIBuildUtils.SetRect(shock, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
        shock.GetComponent<Image>().raycastTarget = false;

        List<RectTransform> particles = new List<RectTransform>();
        List<Image> particleImgs = new List<Image>();
        for (int i = 0; i < 12; i++)
        {
            GameObject p = UIBuildUtils.CreateImage("Particle_" + i, root.transform, Color.white, UIBuildUtils.GetKnobSprite());
            UIBuildUtils.SetSize(p, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(34f, 34f));
            p.GetComponent<Image>().raycastTarget = false;
            particles.Add(p.GetComponent<RectTransform>());
            particleImgs.Add(p.GetComponent<Image>());
        }

        List<RectTransform> chunks = new List<RectTransform>();
        List<Image> chunkImgs = new List<Image>();
        for (int i = 0; i < 6; i++)
        {
            GameObject c = UIBuildUtils.CreateImage("Chunk_" + i, root.transform, Color.white, UIBuildUtils.GetUISprite());
            UIBuildUtils.SetSize(c, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(48f, 48f));
            c.GetComponent<Image>().raycastTarget = false;
            chunks.Add(c.GetComponent<RectTransform>());
            chunkImgs.Add(c.GetComponent<Image>());
        }

        BlockDestroyEffect fx = root.AddComponent<BlockDestroyEffect>();
        SerializedObject so = new SerializedObject(fx);
        so.FindProperty("shockwaveImage").objectReferenceValue = shock.GetComponent<Image>();

        SerializedProperty pProp = so.FindProperty("particles");
        pProp.arraySize = particles.Count;
        for (int i = 0; i < particles.Count; i++) pProp.GetArrayElementAtIndex(i).objectReferenceValue = particles[i];

        SerializedProperty piProp = so.FindProperty("particleImages");
        piProp.arraySize = particleImgs.Count;
        for (int i = 0; i < particleImgs.Count; i++) piProp.GetArrayElementAtIndex(i).objectReferenceValue = particleImgs[i];

        SerializedProperty cProp = so.FindProperty("chunks");
        cProp.arraySize = chunks.Count;
        for (int i = 0; i < chunks.Count; i++) cProp.GetArrayElementAtIndex(i).objectReferenceValue = chunks[i];

        SerializedProperty ciProp = so.FindProperty("chunkImages");
        ciProp.arraySize = chunkImgs.Count;
        for (int i = 0; i < chunkImgs.Count; i++) ciProp.GetArrayElementAtIndex(i).objectReferenceValue = chunkImgs[i];

        so.ApplyModifiedProperties();

        root.SetActive(false);
        return root;
    }

    private static GameObject EnsureCoinBurstPrefabRoot()
    {
        GameObject existing = GameObject.Find("CoinBurstParticleTemplate");
        if (existing != null) return existing;

        GameObject root = new GameObject("CoinBurstParticleTemplate");
        RectTransform rt = root.AddComponent<RectTransform>();
        rt.sizeDelta = new Vector2(60f, 60f);

        GameObject coin = UIBuildUtils.CreateImage("Coin", root.transform, new Color(1f, 0.85f, 0.4f), UIBuildUtils.GetKnobSprite());
        UIBuildUtils.SetRect(coin, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
        coin.GetComponent<Image>().raycastTarget = false;

        GameObject inner = UIBuildUtils.CreateImage("Inner", coin.transform, UIBuildUtils.ACCENT, UIBuildUtils.GetKnobSprite());
        UIBuildUtils.SetRect(inner, new Vector2(0.2f, 0.2f), new Vector2(0.8f, 0.8f), Vector2.zero, Vector2.zero);
        inner.GetComponent<Image>().raycastTarget = false;

        root.SetActive(false);
        return root;
    }

    private static void EnsureMiningBehaviourOnPickaxeTemplate()
    {
        GameObject template = GameObject.Find("PickaxePrefabTemplate");
        if (template == null)
        {
            Debug.LogWarning("PickaxePrefabTemplate not found! Run Iteration 2 setup first.");
            return;
        }
        if (template.GetComponent<PickaxeMiningBehaviour>() == null)
        {
            template.AddComponent<PickaxeMiningBehaviour>();
        }
    }

    private static void EnsureMiningBehaviourOnExistingPickaxes()
    {
        Pickaxe[] all = Object.FindObjectsOfType<Pickaxe>(true);
        foreach (var p in all)
        {
            if (p.gameObject.name == "PickaxePrefabTemplate") continue;
            if (p.GetComponent<PickaxeMiningBehaviour>() == null)
            {
                p.gameObject.AddComponent<PickaxeMiningBehaviour>();
            }
        }
    }

    private static void AttachBlocksRowManager(GameObject blocksRow, List<RectTransform> slots, GameObject blockPrefab, GameObject destroyFxPrefab, GameObject coinBurstPrefab)
    {
        BlocksRowManager mgr = blocksRow.GetComponent<BlocksRowManager>();
        if (mgr == null) mgr = blocksRow.AddComponent<BlocksRowManager>();

        GameObject dragLayer = GameObject.Find("DragLayer");
        Debug.Assert(dragLayer != null, "DragLayer not found! Run Iteration 2 setup first.");

        SerializedObject so = new SerializedObject(mgr);
        SerializedProperty slotsProp = so.FindProperty("blockSlots");
        slotsProp.arraySize = slots.Count;
        for (int i = 0; i < slots.Count; i++) slotsProp.GetArrayElementAtIndex(i).objectReferenceValue = slots[i];
        so.FindProperty("blockPrefabRoot").objectReferenceValue = blockPrefab;
        so.FindProperty("blockDestroyEffectPrefab").objectReferenceValue = destroyFxPrefab;
        so.FindProperty("coinBurstParticlePrefab").objectReferenceValue = coinBurstPrefab;
        so.FindProperty("effectsLayer").objectReferenceValue = dragLayer.GetComponent<RectTransform>();
        so.FindProperty("respawnDelay").floatValue = 0.35f;
        so.FindProperty("coinsBurstCount").intValue = 6;
        so.ApplyModifiedProperties();
    }

    private static void UpdatePickaxeGridManagerMiningTemplate(GameObject miningTemplate)
    {
        PickaxeGridManager grid = Object.FindObjectOfType<PickaxeGridManager>();
        if (grid == null)
        {
            Debug.LogWarning("PickaxeGridManager not found! Run Iteration 2 setup first.");
            return;
        }
        SerializedObject so = new SerializedObject(grid);
        so.FindProperty("miningAttackTemplate").objectReferenceValue = miningTemplate;
        so.ApplyModifiedProperties();
    }

    private static void CreateBlockConfigIfMissing()
    {
        if (!Directory.Exists(CONFIG_DIR))
        {
            Directory.CreateDirectory(CONFIG_DIR);
            AssetDatabase.Refresh();
        }

        BlockData existing = AssetDatabase.LoadAssetAtPath<BlockData>(BLOCK_CONFIG_PATH);
        if (existing != null) return;

        BlockData data = ScriptableObject.CreateInstance<BlockData>();

        BlockTypeData stone = new BlockTypeData
        {
            id = "stone", displayName = "Stone",
            color = new Color(0.55f, 0.55f, 0.6f),
            darkColor = new Color(0.35f, 0.35f, 0.4f),
            hpMultiplier = 1f, rewardMultiplier = 1f
        };
        BlockTypeData iron = new BlockTypeData
        {
            id = "iron", displayName = "Iron",
            color = new Color(0.65f, 0.45f, 0.3f),
            darkColor = new Color(0.4f, 0.28f, 0.18f),
            hpMultiplier = 1.4f, rewardMultiplier = 1.6f
        };
        BlockTypeData gold = new BlockTypeData
        {
            id = "gold", displayName = "Gold",
            color = new Color(0.95f, 0.78f, 0.25f),
            darkColor = new Color(0.7f, 0.55f, 0.15f),
            hpMultiplier = 2f, rewardMultiplier = 3f
        };
        BlockTypeData crystal = new BlockTypeData
        {
            id = "crystal", displayName = "Crystal",
            color = new Color(0.35f, 0.78f, 0.92f),
            darkColor = new Color(0.18f, 0.45f, 0.7f),
            hpMultiplier = 3f, rewardMultiplier = 5f
        };

        data.types = new List<BlockTypeData> { stone, iron, gold, crystal };
        data.sequence = new List<int> { 0, 0, 1, 0, 0, 1, 2, 0, 1, 0, 1, 2 };
        data.baseHP = 10f;
        data.hpGrowth = 1.18f;
        data.baseReward = 3;
        data.rewardGrowth = 1.15f;

        AssetDatabase.CreateAsset(data, BLOCK_CONFIG_PATH);
        AssetDatabase.SaveAssets();
        Debug.Log("BlockConfig.asset created");
    }
}
#endif
