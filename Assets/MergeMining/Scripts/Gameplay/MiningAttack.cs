using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class MiningAttack : MonoBehaviour
{
    [SerializeField] private Image bodyImage;
    [SerializeField] private Image trailImage;

    private RectTransform rt;
    private Block targetBlock;
    private float damage;

    private void Awake()
    {
        rt = transform as RectTransform;
    }

    public void Launch(Vector3 startWorldPos, Block target, float dmg, Color tint)
    {
        targetBlock = target;
        damage = dmg;

        rt.position = startWorldPos;
        rt.localScale = Vector3.one * 0.6f;

        if (bodyImage != null) bodyImage.color = tint;
        if (trailImage != null)
        {
            Color t = tint;
            t.a = 0.4f;
            trailImage.color = t;
        }

        Vector3 endPos = target.RectTransform.position;
        Vector3 mid = (startWorldPos + endPos) * 0.5f;
        mid.y += 180f;

        Vector3[] path = new Vector3[] { mid, endPos };

        Sequence seq = DOTween.Sequence();
        seq.Append(rt.DOPath(path, 0.35f, PathType.CatmullRom).SetEase(Ease.InQuad));
        seq.Join(rt.DOScale(1.1f, 0.35f).SetEase(Ease.OutQuad));
        seq.Join(rt.DORotate(new Vector3(0f, 0f, -270f), 0.35f, RotateMode.FastBeyond360));
        seq.OnComplete(OnHit);
    }

    private void OnHit()
    {
        if (targetBlock != null && targetBlock.IsAlive)
        {
            targetBlock.TakeDamage(damage);
        }

        rt.DOScale(0f, 0.1f).SetEase(Ease.InQuad).OnComplete(() => Destroy(gameObject));
    }
}
