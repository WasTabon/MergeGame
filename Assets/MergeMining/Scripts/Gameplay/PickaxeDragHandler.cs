using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;
using DG.Tweening;

[RequireComponent(typeof(Pickaxe))]
public class PickaxeDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private float pickupScale = 1.15f;
    [SerializeField] private float returnDuration = 0.25f;
    [SerializeField] private float moveDuration = 0.2f;

    private Pickaxe pickaxe;
    private RectTransform rt;
    private Canvas rootCanvas;
    private CanvasGroup canvasGroup;
    private Transform originalParent;
    private Vector3 originalScale;
    private bool isDragging = false;

    private void Awake()
    {
        pickaxe = GetComponent<Pickaxe>();
        rt = transform as RectTransform;
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = gameObject.AddComponent<CanvasGroup>();
        rootCanvas = GetComponentInParent<Canvas>().rootCanvas;
        originalScale = transform.localScale;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (PickaxeGridManager.Instance == null) return;

        isDragging = true;
        originalParent = transform.parent;

        Transform dragLayer = PickaxeGridManager.Instance.DragLayer;
        if (dragLayer != null) transform.SetParent(dragLayer, true);

        canvasGroup.blocksRaycasts = false;
        transform.DOKill();
        transform.DOScale(originalScale * pickupScale, 0.1f).SetEase(Ease.OutQuad);

        if (HapticManager.Instance != null) HapticManager.Instance.Light();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging) return;
        rt.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!isDragging) return;
        isDragging = false;
        canvasGroup.blocksRaycasts = true;

        PickaxeSlot targetSlot = FindSlotUnderPointer(eventData);

        if (targetSlot == null)
        {
            ReturnToOriginalSlot();
            return;
        }

        if (targetSlot == pickaxe.CurrentSlot)
        {
            ReturnToOriginalSlot();
            return;
        }

        if (!targetSlot.IsEmpty)
        {
            Pickaxe targetPickaxe = targetSlot.CurrentPickaxe;
            if (targetPickaxe.Level == pickaxe.Level && pickaxe.Level < PickaxeConfigProvider.Config.MaxLevel)
            {
                PickaxeGridManager.Instance.DoMerge(pickaxe, targetPickaxe);
                return;
            }
            ReturnToOriginalSlot();
            return;
        }

        PickaxeGridManager.Instance.MovePickaxeToSlot(pickaxe, targetSlot, moveDuration);
    }

    private PickaxeSlot FindSlotUnderPointer(PointerEventData eventData)
    {
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);
        foreach (var r in results)
        {
            PickaxeSlot slot = r.gameObject.GetComponentInParent<PickaxeSlot>();
            if (slot != null) return slot;
        }
        return null;
    }

    private void ReturnToOriginalSlot()
    {
        PickaxeSlot slot = pickaxe.CurrentSlot;
        transform.SetParent(slot.transform, true);
        transform.DOKill();
        rt.DOAnchorPos(Vector2.zero, returnDuration).SetEase(Ease.OutBack);
        transform.DOScale(originalScale, returnDuration).SetEase(Ease.OutBack);
    }
}
