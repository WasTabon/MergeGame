using UnityEngine;

public class GameInitializer : MonoBehaviour
{
    private void Awake()
    {
        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 0;
        Input.multiTouchEnabled = false;
    }
}
