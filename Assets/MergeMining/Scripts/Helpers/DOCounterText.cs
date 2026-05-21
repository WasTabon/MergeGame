using UnityEngine;
using TMPro;
using DG.Tweening;

[RequireComponent(typeof(TextMeshProUGUI))]
public class DOCounterText : MonoBehaviour
{
    [SerializeField] private string prefix = "";
    [SerializeField] private string suffix = "";
    [SerializeField] private float duration = 0.5f;

    private TextMeshProUGUI tmp;
    private int displayedValue = 0;
    private Tweener tween;

    private void Awake()
    {
        tmp = GetComponent<TextMeshProUGUI>();
    }

    public void SetImmediate(int value)
    {
        tween?.Kill();
        displayedValue = value;
        tmp.text = prefix + value + suffix;
    }

    public void AnimateTo(int newValue)
    {
        tween?.Kill();
        int from = displayedValue;
        tween = DOTween.To(() => from, x =>
        {
            displayedValue = x;
            tmp.text = prefix + x + suffix;
        }, newValue, duration).SetEase(Ease.OutQuad);
    }
}
