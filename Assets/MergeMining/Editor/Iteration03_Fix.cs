#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public static class Iteration03_Fix
{
    [MenuItem("Tools/Merge Mining/(Iteration 3) FIX - Attach Mining Behaviour")]
    public static void FixMiningBehaviour()
    {
        int templateFixed = 0;
        int instancesFixed = 0;

        GameObject template = GameObject.Find("PickaxePrefabTemplate");
        if (template == null)
        {
            EditorUtility.DisplayDialog("Error", "PickaxePrefabTemplate not found on scene. Run Iteration 2 setup first.", "OK");
            return;
        }

        if (template.GetComponent<PickaxeMiningBehaviour>() == null)
        {
            template.AddComponent<PickaxeMiningBehaviour>();
            templateFixed = 1;
        }

        Pickaxe[] all = Object.FindObjectsOfType<Pickaxe>(true);
        foreach (var p in all)
        {
            if (p.gameObject == template) continue;
            if (p.GetComponent<PickaxeMiningBehaviour>() == null)
            {
                p.gameObject.AddComponent<PickaxeMiningBehaviour>();
                instancesFixed++;
            }
        }

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());

        string msg = "Template fixed: " + (templateFixed == 1 ? "yes" : "no (already had component)") + "\n";
        msg += "Existing pickaxes fixed: " + instancesFixed;
        Debug.Log("[Iteration 3 FIX] " + msg);
        EditorUtility.DisplayDialog("Fix complete", msg, "OK");
    }
}
#endif
