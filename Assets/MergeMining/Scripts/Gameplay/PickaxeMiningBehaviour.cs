using UnityEngine;

[RequireComponent(typeof(Pickaxe))]
public class PickaxeMiningBehaviour : MonoBehaviour
{
    private Pickaxe pickaxe;
    private PickaxeDragHandler dragHandler;
    private float nextAttackTime = 0f;
    private float startupDelay;

    private void Awake()
    {
        pickaxe = GetComponent<Pickaxe>();
        dragHandler = GetComponent<PickaxeDragHandler>();
        startupDelay = Random.Range(0f, 0.6f);
    }

    private void OnEnable()
    {
        nextAttackTime = Time.time + startupDelay;
    }

    private void Update()
    {
        if (LevelManager.Instance != null && LevelManager.Instance.Phase != LevelPhase.Battle) return;
        if (Time.time < nextAttackTime) return;
        if (BlocksRowManager.Instance == null) return;
        if (PickaxeGridManager.Instance == null) return;
        if (pickaxe.CurrentSlot == null) return;
        if (transform.parent != pickaxe.CurrentSlot.transform) return;

        PickaxeLevelData data = PickaxeConfigProvider.Config.GetLevel(pickaxe.Level);
        if (data == null) return;

        Block target = BlocksRowManager.Instance.GetRandomAliveBlock();
        if (target == null)
        {
            nextAttackTime = Time.time + 0.2f;
            return;
        }

        SpawnAttack(target, data);

        float speedMult = BoosterManager.Instance != null ? BoosterManager.Instance.SpeedMultiplier : 1f;
        float interval = 1f / (Mathf.Max(0.1f, data.miningSpeed) * speedMult);
        nextAttackTime = Time.time + interval;
    }

    private void SpawnAttack(Block target, PickaxeLevelData data)
    {
        Transform layer = PickaxeGridManager.Instance.DragLayer;
        if (layer == null) return;

        GameObject template = PickaxeGridManager.Instance.MiningAttackTemplate;
        if (template == null) return;

        GameObject go = Instantiate(template, layer);
        go.SetActive(true);
        MiningAttack attack = go.GetComponent<MiningAttack>();
        if (attack == null) { Destroy(go); return; }

        attack.Launch(pickaxe.RectTransform.position, target, data.damage, data.color);
    }
}
