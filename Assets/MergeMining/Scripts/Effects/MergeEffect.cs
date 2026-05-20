using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections.Generic;

public class MergeEffect : MonoBehaviour
{
    [SerializeField] private Image flashImage;
    [SerializeField] private List<RectTransform> particles;
    [SerializeField] private List<Image> particleImages;
    [SerializeField] private float duration = 0.5f;
    [SerializeField] private float particleRadius = 200f;

    public RectTransform RectTransform => transform as RectTransform;

    public void Play(Color tint)
    {
        if (flashImage != null)
        {
            Color flashColor = tint;
            flashColor.a = 0.9f;
            flashImage.color = flashColor;
            flashImage.transform.localScale = Vector3.one * 0.4f;
            flashImage.transform.DOScale(2.2f, duration).SetEase(Ease.OutQuad);
            flashImage.DOFade(0f, duration).SetEase(Ease.OutQuad);
        }

        for (int i = 0; i < particles.Count; i++)
        {
            RectTransform p = particles[i];
            Image img = particleImages[i];
            p.anchoredPosition = Vector2.zero;
            p.localScale = Vector3.one;

            float angle = (360f / particles.Count) * i + Random.Range(-15f, 15f);
            float rad = angle * Mathf.Deg2Rad;
            Vector2 dir = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));
            float distance = particleRadius * Random.Range(0.7f, 1.2f);

            Color c = tint;
            c.a = 1f;
            img.color = c;

            p.DOAnchorPos(dir * distance, duration).SetEase(Ease.OutQuad);
            p.DOScale(0f, duration).SetEase(Ease.InQuad);
            img.DOFade(0f, duration).SetEase(Ease.InQuad);
        }

        Destroy(gameObject, duration + 0.1f);
    }
}
