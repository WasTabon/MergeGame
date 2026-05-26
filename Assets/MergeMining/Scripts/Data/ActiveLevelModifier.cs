using UnityEngine;

public class ActiveLevelModifier
{
    public LevelModifierId Id;

    public float DamageMultiplier = 1f;
    public int SlotPenalty = 0;
    public int FreePickaxes = 0;
    public float BlockHPMultiplier = 1f;
    public float GemsRewardMultiplier = 1f;
    public float TimerOffset = 0f;
    public float DurabilityMultiplier = 1f;
    public float PickaxeSpeedMultiplier = 1f;
    public float DescendSpeedMultiplier = 1f;
    public int StartingCoinsOffset = 0;
    public int MaxLevelOffset = 0;

    public static ActiveLevelModifier FromDefinition(LevelModifierDefinition def)
    {
        ActiveLevelModifier m = new ActiveLevelModifier { Id = def.id };
        switch (def.id)
        {
            case LevelModifierId.DamageBoostSlotPenalty:
                m.DamageMultiplier = 1.5f;
                m.SlotPenalty = 1;
                break;
            case LevelModifierId.FreePickaxeHpPenalty:
                m.FreePickaxes = 1;
                m.BlockHPMultiplier = 1.3f;
                break;
            case LevelModifierId.DoubleGemsTimePenalty:
                m.GemsRewardMultiplier = 2f;
                m.TimerOffset = -10f;
                break;
            case LevelModifierId.DurabilityBoostHpPenalty:
                m.DurabilityMultiplier = 1.3f;
                m.BlockHPMultiplier = 1.2f;
                break;
            case LevelModifierId.SpeedBoostDescendPenalty:
                m.PickaxeSpeedMultiplier = 1.25f;
                m.DescendSpeedMultiplier = 1.5f;
                break;
            case LevelModifierId.CoinsBoostLevelCap:
                m.StartingCoinsOffset = 20;
                m.MaxLevelOffset = -1;
                break;
        }
        return m;
    }
}
