using UnityEngine;

public class HapticManager : MonoBehaviour
{
    public static HapticManager Instance { get; private set; }

    private const string HAPTIC_ENABLED_KEY = "haptic_enabled";

    private bool hapticsEnabled = true;
    public bool HapticsEnabled => hapticsEnabled;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        hapticsEnabled = PlayerPrefs.GetInt(HAPTIC_ENABLED_KEY, 1) == 1;
    }

    public void Light()
    {
        if (!hapticsEnabled) return;
#if UNITY_IOS || UNITY_ANDROID
        Handheld.Vibrate();
#endif
    }

    public void Medium()
    {
        if (!hapticsEnabled) return;
#if UNITY_IOS || UNITY_ANDROID
        Handheld.Vibrate();
#endif
    }

    public void Heavy()
    {
        if (!hapticsEnabled) return;
#if UNITY_IOS || UNITY_ANDROID
        Handheld.Vibrate();
#endif
    }

    public void SetEnabled(bool enabled)
    {
        hapticsEnabled = enabled;
        PlayerPrefs.SetInt(HAPTIC_ENABLED_KEY, enabled ? 1 : 0);
    }
}
