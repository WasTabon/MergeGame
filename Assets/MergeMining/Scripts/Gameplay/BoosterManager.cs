using System;
using System.Collections.Generic;
using UnityEngine;

public class BoosterManager : MonoBehaviour
{
    public static BoosterManager Instance { get; private set; }

    public class ActiveBooster
    {
        public BoosterInfo info;
        public float remaining;
    }

    private Dictionary<BoosterType, ActiveBooster> active = new Dictionary<BoosterType, ActiveBooster>();
    private float autoMergeTimer = 0f;

    public event Action<BoosterType, float, float> OnBoosterStarted;
    public event Action<BoosterType> OnBoosterEnded;
    public event Action<BoosterType, float> OnBoosterTick;

    public float SpeedMultiplier => IsActive(BoosterType.SpeedX2) ? 2f : 1f;
    public float RewardsMultiplier => IsActive(BoosterType.RewardsX2) ? 2f : 1f;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (active.Count == 0) return;

        List<BoosterType> expired = null;
        foreach (var kvp in active)
        {
            kvp.Value.remaining -= Time.deltaTime;
            OnBoosterTick?.Invoke(kvp.Key, kvp.Value.remaining);
            if (kvp.Value.remaining <= 0f)
            {
                if (expired == null) expired = new List<BoosterType>();
                expired.Add(kvp.Key);
            }
        }

        if (IsActive(BoosterType.AutoMerge))
        {
            autoMergeTimer -= Time.deltaTime;
            if (autoMergeTimer <= 0f)
            {
                TryAutoMergeOnce();
                autoMergeTimer = 2f;
            }
        }

        if (expired != null)
        {
            foreach (var t in expired)
            {
                active.Remove(t);
                OnBoosterEnded?.Invoke(t);
            }
        }
    }

    public bool IsActive(BoosterType type)
    {
        return active.ContainsKey(type);
    }

    public float GetRemaining(BoosterType type)
    {
        return active.TryGetValue(type, out ActiveBooster b) ? b.remaining : 0f;
    }

    public bool TryActivate(BoosterType type)
    {
        BoosterInfo info = BoosterConfigProvider.Config.Get(type);
        if (info == null) return false;

        if (CurrencyManager.Instance == null) return false;
        if (!CurrencyManager.Instance.SpendGems(info.gemsCost)) return false;

        if (type == BoosterType.InstantDestroy)
        {
            ActivateInstantDestroy();
            return true;
        }

        if (active.ContainsKey(type))
        {
            active[type].remaining += info.duration;
        }
        else
        {
            active[type] = new ActiveBooster { info = info, remaining = info.duration };
            OnBoosterStarted?.Invoke(type, info.duration, info.duration);
        }

        if (type == BoosterType.AutoMerge) autoMergeTimer = 0.5f;

        return true;
    }

    private void ActivateInstantDestroy()
    {
        if (BlocksRowManager.Instance == null) return;
        foreach (var b in BlocksRowManager.Instance.ActiveBlocks)
        {
            if (b != null && b.IsAlive)
            {
                b.TakeDamage(999999f);
            }
        }
    }

    private void TryAutoMergeOnce()
    {
        if (PickaxeGridManager.Instance == null) return;
        PickaxeGridManager.Instance.TryAutoMergeOneRandom();
    }
}
