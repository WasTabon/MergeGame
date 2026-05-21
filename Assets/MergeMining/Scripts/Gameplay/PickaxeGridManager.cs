using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class PickaxeGridManager : MonoBehaviour
{
    public static PickaxeGridManager Instance { get; private set; }

    [SerializeField] private List<PickaxeSlot> slots = new List<PickaxeSlot>();
    [SerializeField] private RectTransform dragLayer;
    [SerializeField] private RectTransform pickaxeContainer;
    [SerializeField] private GameObject pickaxePrefabRoot;
    [SerializeField] private MergeEffect mergeEffectPrefab;
    [SerializeField] private GameObject miningAttackTemplate;
    [SerializeField] private float slotSize = 170f;

    public Transform DragLayer => dragLayer;
    public GameObject MiningAttackTemplate => miningAttackTemplate;
    public int MaxLevelOnGrid { get; private set; } = 0;
    public int HighestEverReached { get; private set; } = 1;

    private const string HIGHEST_REACHED_KEY = "highest_pickaxe_level";

    public event Action<Pickaxe> OnPickaxeAdded;
    public event Action<int> OnMerged;
    public event Action OnGridChanged;

    private void Awake()
    {
        Instance = this;
        HighestEverReached = PlayerPrefs.GetInt(HIGHEST_REACHED_KEY, 1);
    }

    private void Start()
    {
        SpawnInitialIfEmpty();
    }

    private void SpawnInitialIfEmpty()
    {
        bool hasAny = false;
        foreach (var s in slots) if (!s.IsEmpty) { hasAny = true; break; }
        if (hasAny) return;
        if (PlayerPrefs.GetInt("starter_pickaxes_given", 0) == 1) return;

        AddPickaxe(1);
        AddPickaxe(1);
        PlayerPrefs.SetInt("starter_pickaxes_given", 1);
    }

    public PickaxeSlot GetFirstFreeSlot()
    {
        foreach (var s in slots) if (s.IsEmpty) return s;
        return null;
    }

    public bool HasFreeSlot()
    {
        return GetFirstFreeSlot() != null;
    }

    public Pickaxe AddPickaxe(int level)
    {
        PickaxeSlot slot = GetFirstFreeSlot();
        if (slot == null)
        {
            Debug.LogWarning("PickaxeGridManager: no free slots for new pickaxe");
            return null;
        }

        Pickaxe p = CreatePickaxe(level, slot);
        slot.SetPickaxe(p);
        p.CurrentSlot = slot;
        p.PlaySpawnAnimation();
        UpdateMaxLevel();
        OnPickaxeAdded?.Invoke(p);
        OnGridChanged?.Invoke();
        return p;
    }

    private Pickaxe CreatePickaxe(int level, PickaxeSlot slot)
    {
        GameObject go = Instantiate(pickaxePrefabRoot, slot.transform);
        go.name = "Pickaxe_lvl" + level;
        RectTransform rt = go.transform as RectTransform;
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = Vector2.zero;
        rt.sizeDelta = new Vector2(slotSize - 20f, slotSize - 20f);
        go.SetActive(true);

        Pickaxe p = go.GetComponent<Pickaxe>();
        p.SetLevel(level);
        return p;
    }

    public void MovePickaxeToSlot(Pickaxe p, PickaxeSlot targetSlot, float duration)
    {
        if (p.CurrentSlot != null) p.CurrentSlot.Clear();
        targetSlot.SetPickaxe(p);
        p.CurrentSlot = targetSlot;

        p.transform.SetParent(targetSlot.transform, true);
        RectTransform rt = p.RectTransform;
        rt.DOAnchorPos(Vector2.zero, duration).SetEase(Ease.OutQuad);
        p.transform.DOScale(Vector3.one, duration).SetEase(Ease.OutBack);

        OnGridChanged?.Invoke();
    }

    public void DoMerge(Pickaxe source, Pickaxe target)
    {
        int newLevel = target.Level + 1;
        PickaxeSlot sourceSlot = source.CurrentSlot;
        PickaxeSlot targetSlot = target.CurrentSlot;

        sourceSlot.Clear();

        source.transform.SetParent(dragLayer, true);
        source.RectTransform.DOMove(target.RectTransform.position, 0.2f).SetEase(Ease.InQuad)
            .OnComplete(() =>
            {
                Destroy(source.gameObject);
                target.SetLevel(newLevel);
                target.PlayLevelUpAnimation();

                if (mergeEffectPrefab != null)
                {
                    MergeEffect fx = Instantiate(mergeEffectPrefab, dragLayer);
                    fx.RectTransform.position = target.RectTransform.position;
                    fx.Play(PickaxeConfigProvider.Config.GetLevel(newLevel).color);
                }

                Camera cam = Camera.main;
                if (cam != null)
                {
                    cam.transform.DOKill();
                    cam.transform.DOShakePosition(0.25f, 0.15f, 10, 90f);
                }

                if (HapticManager.Instance != null) HapticManager.Instance.Medium();

                UpdateMaxLevel();
                if (newLevel > HighestEverReached)
                {
                    HighestEverReached = newLevel;
                    PlayerPrefs.SetInt(HIGHEST_REACHED_KEY, HighestEverReached);
                }

                if (ZoneManager.Instance != null)
                {
                    ZoneManager.Instance.CheckUnlock(HighestEverReached);
                }

                OnMerged?.Invoke(newLevel);
                OnGridChanged?.Invoke();
            });
    }

    private void UpdateMaxLevel()
    {
        int max = 0;
        foreach (var s in slots)
        {
            if (!s.IsEmpty && s.CurrentPickaxe.Level > max) max = s.CurrentPickaxe.Level;
        }
        MaxLevelOnGrid = max;
    }

    public int GetMaxLevelOnGrid() => MaxLevelOnGrid;
}
