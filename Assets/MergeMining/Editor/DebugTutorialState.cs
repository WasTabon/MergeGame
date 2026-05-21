#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Text;

public static class DebugTutorialState
{
    [MenuItem("Tools/Merge Mining/DEBUG - Tutorial State")]
    public static void DumpState()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("=== TUTORIAL STATE ===");

        sb.AppendLine("PlayerPrefs tutorial_done = " + PlayerPrefs.GetInt("tutorial_done", 0));
        sb.AppendLine("PlayerPrefs daily_last_claim_date = '" + PlayerPrefs.GetString("daily_last_claim_date", "") + "'");
        sb.AppendLine("PlayerPrefs starter_pickaxes_given = " + PlayerPrefs.GetInt("starter_pickaxes_given", 0));
        sb.AppendLine("PlayerPrefs pickaxes_bought = " + PlayerPrefs.GetInt("pickaxes_bought", 0));

        TutorialManager tm = Object.FindObjectOfType<TutorialManager>();
        if (tm == null)
        {
            sb.AppendLine("TutorialManager: NOT FOUND on scene!");
        }
        else
        {
            sb.AppendLine("TutorialManager found on: " + tm.gameObject.name);
            sb.AppendLine("  IsActive: " + tm.IsActive);
            sb.AppendLine("  CurrentStep: " + tm.CurrentStep);
        }

        TutorialOverlay overlay = Object.FindObjectOfType<TutorialOverlay>(true);
        if (overlay == null)
        {
            sb.AppendLine("TutorialOverlay: NOT FOUND on scene!");
        }
        else
        {
            sb.AppendLine("TutorialOverlay found on: " + overlay.gameObject.name);
            sb.AppendLine("  activeInHierarchy: " + overlay.gameObject.activeInHierarchy);
            sb.AppendLine("  activeSelf: " + overlay.gameObject.activeSelf);
            CanvasGroup cg = overlay.GetComponent<CanvasGroup>();
            if (cg != null) sb.AppendLine("  CanvasGroup alpha=" + cg.alpha + ", blocksRaycasts=" + cg.blocksRaycasts);
        }

        DailyRewardManager dm = Object.FindObjectOfType<DailyRewardManager>();
        if (dm != null)
        {
            sb.AppendLine("DailyRewardManager.CanClaimToday: " + dm.CanClaimToday());
        }

        Debug.Log(sb.ToString());
    }

    [MenuItem("Tools/Merge Mining/DEBUG - Force Reset Tutorial")]
    public static void ForceResetTutorial()
    {
        PlayerPrefs.SetInt("tutorial_done", 0);
        PlayerPrefs.Save();
        Debug.Log("[DEBUG] tutorial_done reset to 0. Restart scene to see tutorial.");
    }
}
#endif
