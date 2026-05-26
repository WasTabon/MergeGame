using System.Collections.Generic;
using UnityEngine;

public enum LevelModifierId
{
    DamageBoostSlotPenalty,
    FreePickaxeHpPenalty,
    DoubleGemsTimePenalty,
    DurabilityBoostHpPenalty,
    SpeedBoostDescendPenalty,
    CoinsBoostLevelCap
}

[System.Serializable]
public class LevelModifierDefinition
{
    public LevelModifierId id;
    public string title;
    [TextArea(2, 4)] public string description;
    public Color iconColor = Color.white;
}

public static class LevelModifierCatalog
{
    public static List<LevelModifierDefinition> All = new List<LevelModifierDefinition>
    {
        new LevelModifierDefinition
        {
            id = LevelModifierId.DamageBoostSlotPenalty,
            title = "BLOODLUST",
            description = "+50% pickaxe damage\nBut: -1 grid slot available",
            iconColor = new Color(0.95f, 0.3f, 0.3f)
        },
        new LevelModifierDefinition
        {
            id = LevelModifierId.FreePickaxeHpPenalty,
            title = "STARTER KIT",
            description = "+1 free Lvl 1 pickaxe\nBut: blocks have +30% HP",
            iconColor = new Color(0.4f, 0.78f, 0.4f)
        },
        new LevelModifierDefinition
        {
            id = LevelModifierId.DoubleGemsTimePenalty,
            title = "GREEDY HASTE",
            description = "Double gem reward\nBut: -10 seconds timer",
            iconColor = new Color(0.4f, 0.85f, 0.9f)
        },
        new LevelModifierDefinition
        {
            id = LevelModifierId.DurabilityBoostHpPenalty,
            title = "FORGED TOUGH",
            description = "+30% pickaxe durability\nBut: blocks have +20% HP",
            iconColor = new Color(0.95f, 0.78f, 0.25f)
        },
        new LevelModifierDefinition
        {
            id = LevelModifierId.SpeedBoostDescendPenalty,
            title = "RUSH HOUR",
            description = "+25% pickaxe attack speed\nBut: blocks descend 50% faster",
            iconColor = new Color(0.55f, 0.565f, 0.886f)
        },
        new LevelModifierDefinition
        {
            id = LevelModifierId.CoinsBoostLevelCap,
            title = "BUDGET PLUS",
            description = "+20 starting coins\nBut: max pickaxe level -1",
            iconColor = new Color(0.85f, 0.55f, 0.95f)
        }
    };
}
