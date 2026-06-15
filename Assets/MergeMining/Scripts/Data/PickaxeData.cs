using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PickaxeLevelData
{
    public int level;
    public Color color = Color.white;
    public Sprite spriteOverride;
    public float damage = 1f;
    public float miningSpeed = 1f;
    public string displayName = "Pickaxe";
    public int durability = 10;
}

[CreateAssetMenu(menuName = "MergeMining/PickaxeConfig", fileName = "PickaxeConfig")]
public class PickaxeData : ScriptableObject
{
    public List<PickaxeLevelData> levels = new List<PickaxeLevelData>();

    public PickaxeLevelData GetLevel(int level)
    {
        if (level < 1 || level > levels.Count)
        {
            Debug.LogWarning("PickaxeData: requested level " + level + " is out of range. Max is " + levels.Count);
            return levels.Count > 0 ? levels[0] : null;
        }
        return levels[level - 1];
    }

    public int MaxLevel => levels.Count;
}
