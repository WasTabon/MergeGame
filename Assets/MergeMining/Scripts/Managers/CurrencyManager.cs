using System;
using UnityEngine;

public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager Instance { get; private set; }

    private const string COINS_KEY = "coins";
    private const string GEMS_KEY = "gems";

    public event Action<int> OnCoinsChanged;
    public event Action<int> OnGemsChanged;

    private int coins;
    private int gems;

    public int Coins => coins;
    public int Gems => gems;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        coins = PlayerPrefs.GetInt(COINS_KEY, 50);
        gems = PlayerPrefs.GetInt(GEMS_KEY, 5);
    }

    public void AddCoins(int amount)
    {
        if (amount <= 0) return;
        coins += amount;
        PlayerPrefs.SetInt(COINS_KEY, coins);
        OnCoinsChanged?.Invoke(coins);
    }

    public bool SpendCoins(int amount)
    {
        if (amount <= 0) return false;
        if (coins < amount) return false;
        coins -= amount;
        PlayerPrefs.SetInt(COINS_KEY, coins);
        OnCoinsChanged?.Invoke(coins);
        return true;
    }

    public void AddGems(int amount)
    {
        if (amount <= 0) return;
        gems += amount;
        PlayerPrefs.SetInt(GEMS_KEY, gems);
        OnGemsChanged?.Invoke(gems);
    }

    public bool SpendGems(int amount)
    {
        if (amount <= 0) return false;
        if (gems < amount) return false;
        gems -= amount;
        PlayerPrefs.SetInt(GEMS_KEY, gems);
        OnGemsChanged?.Invoke(gems);
        return true;
    }

    public bool CanAfford(int amount) => coins >= amount;
    public bool CanAffordGems(int amount) => gems >= amount;
}
