#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public static class Iteration15Fix_Setup
{
    private const string GAME_SCENE_PATH = "Assets/MergeMining/Scenes/Game.unity";

    [MenuItem("Tools/Merge Mining/(Iteration 15 FIX) Attach Tutorial Starter")]
    public static void AttachStarter()
    {
        EditorSceneManager.OpenScene(GAME_SCENE_PATH);

        TextTutorialPopup popup = Object.FindObjectOfType<TextTutorialPopup>(true);
        if (popup == null)
        {
            Debug.LogError("TextTutorialPopup not found! Run Iter 15 first.");
            return;
        }

        GameObject canvasGo = GameObject.Find("Canvas");
        if (canvasGo == null)
        {
            Debug.LogError("Canvas not found!");
            return;
        }

        TextTutorialStarter starter = canvasGo.GetComponent<TextTutorialStarter>();
        if (starter == null) starter = canvasGo.AddComponent<TextTutorialStarter>();

        SerializedObject so = new SerializedObject(starter);
        so.FindProperty("popup").objectReferenceValue = popup;
        so.ApplyModifiedProperties();

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
        Debug.Log("TextTutorialStarter attached to Canvas.");
    }
}
#endif
