using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class PopupManager : MonoBehaviour
{
    public static PopupManager Instance { get; private set; }

    private Stack<BasePopup> openPopups = new Stack<BasePopup>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void Register(BasePopup popup)
    {
        openPopups.Push(popup);
    }

    public void Unregister(BasePopup popup)
    {
        if (openPopups.Count > 0 && openPopups.Peek() == popup)
        {
            openPopups.Pop();
        }
    }

    public bool IsAnyPopupOpen => openPopups.Count > 0;
}
