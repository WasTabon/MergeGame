using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class ZoneManager : MonoBehaviour
{
    public static ZoneManager Instance { get; private set; }

    [SerializeField] private Image bgTopImage;
    [SerializeField] private Image bgBottomImage;
    [SerializeField] private TextMeshProUGUI zoneNameText;
    [SerializeField] private TextMeshProUGUI zoneIndexText;
    [SerializeField] private ZoneCompletePopup zoneCompletePopup;

    private const string CURRENT_ZONE_KEY = "current_zone_index";
    private const string UNLOCKED_ZONES_KEY = "unlocked_zones_count";

    private int currentZoneIndex = 0;
    private int unlockedZonesCount = 1;

    public ZoneInfo CurrentZone => ZoneConfigProvider.Config.GetZone(currentZoneIndex);
    public int CurrentZoneIndex => currentZoneIndex;

    public event Action<ZoneInfo> OnZoneChanged;
    public event Action<ZoneInfo, int> OnZoneCompleted;

    private void Awake()
    {
        Instance = this;
        currentZoneIndex = PlayerPrefs.GetInt(CURRENT_ZONE_KEY, 0);
        unlockedZonesCount = PlayerPrefs.GetInt(UNLOCKED_ZONES_KEY, 1);
    }

    private void Start()
    {
        ApplyZoneVisuals(false);
    }

    public void CheckUnlock(int highestPickaxeLevel)
    {
        ZoneData config = ZoneConfigProvider.Config;
        for (int i = unlockedZonesCount; i < config.Count; i++)
        {
            ZoneInfo zone = config.GetZone(i);
            if (highestPickaxeLevel >= zone.requiredPickaxeLevel)
            {
                CompleteCurrentZoneAndAdvanceTo(i);
                return;
            }
            else break;
        }
    }

    private void CompleteCurrentZoneAndAdvanceTo(int newZoneIndex)
    {
        ZoneInfo completed = CurrentZone;
        int gemsReward = completed.gemsReward;

        unlockedZonesCount = newZoneIndex + 1;
        currentZoneIndex = newZoneIndex;
        PlayerPrefs.SetInt(CURRENT_ZONE_KEY, currentZoneIndex);
        PlayerPrefs.SetInt(UNLOCKED_ZONES_KEY, unlockedZonesCount);

        OnZoneCompleted?.Invoke(completed, gemsReward);

        if (zoneCompletePopup != null)
        {
            zoneCompletePopup.ShowZoneCompleted(completed.displayName, gemsReward, CurrentZone.displayName);
        }

        ApplyZoneVisuals(true);
        OnZoneChanged?.Invoke(CurrentZone);
    }

    private void ApplyZoneVisuals(bool animate)
    {
        ZoneInfo zone = CurrentZone;
        if (zone == null) return;

        if (animate)
        {
            if (bgTopImage != null) bgTopImage.DOColor(zone.bgTopColor, 0.8f).SetUpdate(true);
            if (bgBottomImage != null) bgBottomImage.DOColor(zone.bgBottomColor, 0.8f).SetUpdate(true);
        }
        else
        {
            if (bgTopImage != null) bgTopImage.color = zone.bgTopColor;
            if (bgBottomImage != null) bgBottomImage.color = zone.bgBottomColor;
        }

        if (zoneNameText != null) zoneNameText.text = zone.displayName.ToUpper();
        if (zoneIndexText != null) zoneIndexText.text = "ZONE " + (currentZoneIndex + 1) + "/" + ZoneConfigProvider.Config.Count;
    }
}
