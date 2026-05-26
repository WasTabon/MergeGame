#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Text;

public static class DebugTutorialState15
{
    [MenuItem("Tools/Merge Mining/DEBUG - Tutorial State 15")]
    public static void DumpState()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("=== TUTORIAL STATE 15 ===");

        sb.AppendLine("PlayerPrefs text_tutorial_done = " + PlayerPrefs.GetInt("text_tutorial_done", -1));
        sb.AppendLine("PlayerPrefs tutorial_done (OLD) = " + PlayerPrefs.GetInt("tutorial_done", -1));
        sb.AppendLine("PlayerPrefs current_level = " + PlayerPrefs.GetInt("current_level", -1));
        sb.AppendLine("PlayerPrefs pending_level_to_play = " + PlayerPrefs.GetInt("pending_level_to_play", -1));
        sb.AppendLine("PlayerPrefs max_passed_level = " + PlayerPrefs.GetInt("max_passed_level", -1));

        TextTutorialPopup popup = Object.FindObjectOfType<TextTutorialPopup>(true);
        if (popup == null) sb.AppendLine("TextTutorialPopup: NOT FOUND on scene!");
        else
        {
            sb.AppendLine("TextTutorialPopup: found on " + popup.gameObject.name);
            sb.AppendLine("  activeInHierarchy: " + popup.gameObject.activeInHierarchy);
            sb.AppendLine("  activeSelf: " + popup.gameObject.activeSelf);
        }

        LevelManager lm = Object.FindObjectOfType<LevelManager>();
        if (lm == null) sb.AppendLine("LevelManager: NOT FOUND!");
        else
        {
            sb.AppendLine("LevelManager.CurrentLevelNumber = " + lm.CurrentLevelNumber);
            sb.AppendLine("LevelManager.CurrentLevel == null: " + (lm.CurrentLevel == null));
            sb.AppendLine("LevelManager.Phase = " + lm.Phase);
        }

        sb.AppendLine("TextTutorialPopup.IsDone() = " + TextTutorialPopup.IsDone());

        Debug.Log(sb.ToString());
    }

    [MenuItem("Tools/Merge Mining/DEBUG - Force Clear ALL PlayerPrefs")]
    public static void ClearAll()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        Debug.Log("PlayerPrefs FULLY cleared via DeleteAll()");
    }
}
#endif
