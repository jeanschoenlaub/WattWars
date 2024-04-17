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
        idleCoinText.text = coins.ToString();
    }

    public void IncreaseCurrency( int amount ){
        coins += amount;
        idleCoinText.text = amount.ToString();
    }

    public bool SpendCurrency( int amount ){
        if (amount <= coins){
            //Buy Item
            coins -= amount;
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
