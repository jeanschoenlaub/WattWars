using UnityEngine;
using TMPro;

public class IdleManager : MonoBehaviour
{
    public static IdleManager main;

    [Header("Attributes")]
    [SerializeField] public TextMeshProUGUI idleCoinText;
     
    public int coins; 
    
    private void Awake()
    {
        if (main != null && main != this)
        {
            Destroy(gameObject); // Ensure singleton integrity
            return;
        }
        main = this;
    }


    private void Start(){
        // Load  coins =  PlayerPrefs
        LoadPlayerCoins();
        idleCoinText.text = FormatNumber(coins);
    }

    public static string FormatNumber(int num)
{
    if (num >= 1000000000)
        return (num / 1000000000D).ToString("0.#") + "B"; // Billion
    if (num >= 1000000)
        return (num / 1000000D).ToString("0.#") + "M"; // Million
    if (num >= 1000)
        return (num / 1000D).ToString("0.#") + "K"; // Thousand

    return num.ToString(); // Less than 1000
}

    public void IncreaseCurrency(int amount ){
        coins += amount;
        idleCoinText.text = FormatNumber(coins);
        SaveCurrentCoins(coins);
    }

    private void SaveCurrentCoins(int coins)
    {
        PlayerPrefs.SetInt("PlayerCoins", coins);
        PlayerPrefs.Save();
    }

    private void LoadPlayerCoins()
    {
        coins = PlayerPrefs.GetInt("PlayerCoins", 0);
    }

    public bool SpendCurrency( int amount ){
        if (amount <= coins){
            //Buy Item
            coins -= amount;
            SaveCurrentCoins(coins);
            return true;
        }
        else{ 
            Debug.Log("Not enough money");
            return false;
        }
        
    }

    public int GetCurrentMoney(){
        return coins;
    }
}
