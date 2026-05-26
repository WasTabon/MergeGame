using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class SacrificeButton : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private TextMeshProUGUI label;

    private void Start()
    {
        if (button != null) button.onClick.AddListener(OnClicked);
        Refresh();
    }

    private void Update()
    {
        Refresh();
    }

    private void Refresh()
    {
        if (button == null) return;
        bool available = LevelManager.Instance != null
            && LevelManager.Instance.Phase == LevelPhase.Battle
            && !LevelManager.Instance.SacrificeUsed
            && PickaxeGridManager.Instance != null
            && PickaxeGridManager.Instance.CountAlive() > 0;
        button.interactable = available;
        if (label != null) label.text = (LevelManager.Instance != null && LevelManager.Instance.SacrificeUsed) ? "USED" : "SACRIFICE";
    }

    private void OnClicked()
    {
        if (LevelManager.Instance == null) return;
        if (LevelManager.Instance.SacrificeUsed) return;
        if (PickaxeGridManager.Instance == null) return;
        if (BlocksRowManager.Instance == null) return;

        var pickaxes = UnityEngine.Object.FindObjectsOfType<Pickaxe>();
        if (pickaxes == null || pickaxes.Length == 0) return;

        Pickaxe strongest = null;
        int highest = -1;
        foreach (var p in pickaxes)
        {
            if (p == null) continue;
            if (p.Level > highest)
            {
                highest = p.Level;
                strongest = p;
            }
        }
        if (strongest == null) return;

        Vector3 srcPos = strongest.transform.position;
        if (strongest.CurrentSlot != null) strongest.CurrentSlot.Clear();
        strongest.transform.DOKill();
        strongest.transform.DOScale(0f, 0.3f).SetEase(Ease.InBack)
            .OnComplete(() => Destroy(strongest.gameObject));

        List<Block> snapshot = new List<Block>();
        foreach (var b in BlocksRowManager.Instance.ActiveBlocks)
        {
            if (b == null || !b.IsAlive) continue;
            snapshot.Add(b);
        }
        foreach (var b in snapshot)
        {
            if (b != null && b.IsAlive) b.TakeDamage(999999f, 99);
        }

        Camera cam = Camera.main;
        if (cam != null)
        {
            cam.transform.DOKill();
            cam.transform.DOShakePosition(0.6f, 0.7f, 16, 90f);
        }
        if (HapticManager.Instance != null) HapticManager.Instance.Heavy();
        if (SfxLibrary.Instance != null) SfxLibrary.Instance.Play(SfxLibrary.Instance.blockExplode, 1.5f, 0.5f);

        LevelManager.Instance.NotifySacrificeUsed();
        if (PickaxeGridManager.Instance != null) PickaxeGridManager.Instance.NotifyPickaxeBroken();
        Refresh();
    }
}
