#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Text;
using System.Reflection;

public static class DebugFlySpawner
{
    [MenuItem("Tools/Merge Mining/DEBUG - FlyEffectSpawner State")]
    public static void Dump()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("=== FLY EFFECT SPAWNER DEBUG ===");

        FlyEffectSpawner spawner = Object.FindObjectOfType<FlyEffectSpawner>(true);
        if (spawner == null) { Debug.Log("FlyEffectSpawner: NOT FOUND"); return; }

        sb.AppendLine("Found on: " + spawner.gameObject.name + ", activeInHierarchy=" + spawner.gameObject.activeInHierarchy);

        SerializedObject so = new SerializedObject(spawner);
        var coinT = so.FindProperty("coinFlyTemplate").objectReferenceValue;
        var gemT = so.FindProperty("gemFlyTemplate").objectReferenceValue;
        var layer = so.FindProperty("layer").objectReferenceValue;
        var coinsTarget = so.FindProperty("coinsTarget").objectReferenceValue;
        var gemsTarget = so.FindProperty("gemsTarget").objectReferenceValue;

        sb.AppendLine("coinFlyTemplate: " + (coinT == null ? "NULL" : coinT.name));
        sb.AppendLine("gemFlyTemplate: " + (gemT == null ? "NULL" : gemT.name));
        sb.AppendLine("layer (DragLayer): " + (layer == null ? "NULL" : layer.name));
        sb.AppendLine("coinsTarget: " + (coinsTarget == null ? "NULL" : coinsTarget.name));
        sb.AppendLine("gemsTarget: " + (gemsTarget == null ? "NULL" : gemsTarget.name));

        Debug.Log(sb.ToString());
    }
}
#endif
