using UnityEngine;

public class PickaxeSlot : MonoBehaviour
{
    [SerializeField] private int row;
    [SerializeField] private int col;

    public int Row => row;
    public int Col => col;
    public Pickaxe CurrentPickaxe { get; private set; }
    public bool IsEmpty => CurrentPickaxe == null;
    public RectTransform RectTransform => transform as RectTransform;

    public void Init(int r, int c)
    {
        row = r;
        col = c;
    }

    public void SetPickaxe(Pickaxe p)
    {
        CurrentPickaxe = p;
    }

    public void Clear()
    {
        CurrentPickaxe = null;
    }
}
