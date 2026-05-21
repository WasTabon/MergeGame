using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BoosterButtonView : MonoBehaviour
{
    public BoosterType type;
    public Button button;
    public Image iconImage;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI costText;
    public RectTransform costRow;
    public Image cooldownOverlay;
    public TextMeshProUGUI remainingText;
}
