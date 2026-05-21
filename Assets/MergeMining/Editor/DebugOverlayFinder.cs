#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.Text;

public static class DebugOverlayFinder
{
    [MenuItem("Tools/Merge Mining/DEBUG - Find Blocking Overlays")]
    public static void FindBlockingOverlays()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("=== BLOCKING OVERLAY SCAN ===");

        Canvas[] canvases = Object.FindObjectsOfType<Canvas>(true);
        System.Array.Sort(canvases, (a, b) => b.sortingOrder.CompareTo(a.sortingOrder));

        foreach (Canvas c in canvases)
        {
            if (!c.gameObject.activeInHierarchy) continue;
            sb.AppendLine("\n--- Canvas: " + c.name + " (sortingOrder=" + c.sortingOrder + ") ---");

            CanvasGroup cg = c.GetComponentInChildren<CanvasGroup>(true);
            if (cg != null)
            {
                sb.AppendLine("  CanvasGroup on " + cg.gameObject.name + ": alpha=" + cg.alpha + ", blocksRaycasts=" + cg.blocksRaycasts + ", interactable=" + cg.interactable + ", active=" + cg.gameObject.activeInHierarchy);
            }

            Image[] imgs = c.GetComponentsInChildren<Image>(true);
            foreach (Image img in imgs)
            {
                if (!img.gameObject.activeInHierarchy) continue;
                if (!img.raycastTarget) continue;
                if (img.color.a < 0.1f) continue;

                RectTransform rt = img.rectTransform;
                Vector3[] corners = new Vector3[4];
                rt.GetWorldCorners(corners);
                float w = Vector3.Distance(corners[0], corners[3]);
                float h = Vector3.Distance(corners[0], corners[1]);

                if (w > Screen.width * 0.7f && h > Screen.height * 0.7f)
                {
                    sb.AppendLine("  >> FULL-SCREEN IMAGE: " + GetPath(img.gameObject) + " | alpha=" + img.color.a.ToString("F2") + " | raycastTarget=true | enabled=" + img.enabled);
                }
            }
        }

        Debug.Log(sb.ToString());
    }

    private static string GetPath(GameObject go)
    {
        if (go.transform.parent == null) return go.name;
        return GetPath(go.transform.parent.gameObject) + "/" + go.name;
    }
}
#endif
