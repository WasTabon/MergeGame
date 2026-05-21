#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public static class Iteration11_Setup
{
    private const string MENU_SCENE_PATH = "Assets/MergeMining/Scenes/MainMenu.unity";
    private const string GAME_SCENE_PATH = "Assets/MergeMining/Scenes/Game.unity";

    [MenuItem("Tools/Merge Mining/(Iteration 11) Update ALL Scenes")]
    public static void UpdateAll()
    {
        UpdateMenu();
        UpdateGame();
        EditorSceneManager.OpenScene(MENU_SCENE_PATH);
        Debug.Log("Iteration 11: ALL scenes updated.");
    }

    [MenuItem("Tools/Merge Mining/(Iteration 11) Update MainMenu Scene")]
    public static void UpdateMenu()
    {
        EditorSceneManager.OpenScene(MENU_SCENE_PATH);
        EnsureSfxLibrary();
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
    }

    [MenuItem("Tools/Merge Mining/(Iteration 11) Update Game Scene")]
    public static void UpdateGame()
    {
        EditorSceneManager.OpenScene(GAME_SCENE_PATH);
        EnsureSfxLibrary();
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
    }

    private static void EnsureSfxLibrary()
    {
        SfxLibrary existing = Object.FindObjectOfType<SfxLibrary>();
        if (existing != null) return;

        GameObject managers = GameObject.Find("Managers");
        if (managers == null)
        {
            managers = new GameObject("Managers");
        }
        managers.AddComponent<SfxLibrary>();
    }
}
#endif
