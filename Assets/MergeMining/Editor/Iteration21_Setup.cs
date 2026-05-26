#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

public static class Iteration21_Setup
{
    [MenuItem("Tools/Merge Mining/(Iteration 21) Info")]
    public static void Info()
    {
        Debug.Log("Iteration 21: code-only changes (no scene/asset rebuild needed). " +
                  "Blocks now drop coins on death. Shop is available during Battle phase. " +
                  "Just compile and play.");
    }
}
#endif
