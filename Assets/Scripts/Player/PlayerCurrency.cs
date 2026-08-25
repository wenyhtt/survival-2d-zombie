using UnityEngine;
using System;

public class PlayerCurrency : MonoBehaviour
{
    [SerializeField] private int currentCoins = 100; // Mock starting money
    
    public event Action<int> OnCurrencyChanged;
    
    public int CurrentCoins => currentCoins;
    
    public void AddCoins(int amount)
    {
        currentCoins += amount;
        OnCurrencyChanged?.Invoke(currentCoins);
    }
    
    public bool SpendCoins(int amount)
    {
        if (currentCoins >= amount)
        {
            currentCoins -= amount;
            OnCurrencyChanged?.Invoke(currentCoins);
            return true;
        }
        return false;
    }
}
