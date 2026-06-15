#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

public static class Iteration23_Setup
{
    [MenuItem("Tools/Merge Mining/(Iteration 23) Re-Bind Sprites + Armored=Shell")]
    public static void Rebind()
    {
        Iteration22_Setup.AutoBind();
        Debug.Log("Iteration 23: armored block now uses shell sprite. All blocks same size.");
    }
}
#endif
