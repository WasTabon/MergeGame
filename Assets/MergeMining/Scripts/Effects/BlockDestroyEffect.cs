using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BlockDestroyEffect : MonoBehaviour
{
    [SerializeField] private Image shockwaveImage;
    [SerializeField] private List<RectTransform> particles;
    [SerializeField] private List<Image> particleImages;
    [SerializeField] private List<RectTransform> chunks;
    [SerializeField] private List<Image> chunkImages;
    [SerializeField] private float duration = 0.7f;
    [SerializeField] private float particleRadius = 280f;
    [SerializeField] private float chunkRadius = 200f;

    public RectTransform RectTransform => transform as RectTransform;

    public void Play(Color tint)
    {
        if (shockwaveImage != null)
        {
            Color c = Color.white;
            c.a = 0.85f;
            shockwaveImage.color = c;
            shockwaveImage.transform.localScale = Vector3.one * 0.3f;
            shockwaveImage.transform.DOScale(3.2f, duration).SetEase(Ease.OutQuad);
            shockwaveImage.DOFade(0f, duration).SetEase(Ease.OutQuad);
        }

        for (int i = 0; i < particles.Count; i++)
        {
            RectTransform p = particles[i];
            Image img = particleImages[i];
            p.anchoredPosition = Vector2.zero;
            p.localScale = Vector3.one * Random.Range(0.6f, 1.2f);

            float angle = (360f / particles.Count) * i + Random.Range(-12f, 12f);
            float rad = angle * Mathf.Deg2Rad;
            Vector2 dir = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));
            float distance = particleRadius * Random.Range(0.7f, 1.3f);

            Color col = Color.Lerp(tint, Color.white, 0.3f);
            col.a = 1f;
            img.color = col;

            p.DOAnchorPos(dir * distance, duration).SetEase(Ease.OutQuad);
            p.DOScale(0f, duration).SetEase(Ease.InQuad);
            img.DOFade(0f, duration).SetEase(Ease.InQuad);
        }

        for (int i = 0; i < chunks.Count; i++)
        {
            RectTransform p = chunks[i];
            Image img = chunkImages[i];
            p.anchoredPosition = Vector2.zero;
            p.localScale = Vector3.one;
            p.localRotation = Quaternion.identity;

            float angle = (360f / chunks.Count) * i + Random.Range(-30f, 30f);
            float rad = angle * Mathf.Deg2Rad;
            Vector2 dir = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));
            float distance = chunkRadius * Random.Range(0.8f, 1.2f);
            Vector2 endPos = dir * distance;
            endPos.y -= 200f;

            Color col = tint;
            col.a = 1f;
            img.color = col;

            float rotate = Random.Range(-540f, 540f);

            p.DOAnchorPos(endPos, duration).SetEase(Ease.OutQuad);
            p.DOLocalRotate(new Vector3(0f, 0f, rotate), duration, RotateMode.FastBeyond360);
            img.DOFade(0f, duration).SetEase(Ease.InQuad);
        }

        Destroy(gameObject, duration + 0.1f);
    }
}
