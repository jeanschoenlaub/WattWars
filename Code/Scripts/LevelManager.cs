using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager main;
    
    public Transform startPoint;
    public Transform[] path;

    public int coins;
    private static int gameSpeed = 1; // Default game speed

    // Public method to set game speed
    public static void SetGameSpeed(int speed)
    {
        gameSpeed = speed;
    }

    // Public method to get current game speed
    public static int GetGameSpeed()
    {
        return gameSpeed;
    }

    private void Awake(){
        main = this;
    }

    private void Start(){
        coins = 100;
    }

    public void IncreaseCurrency( int amount ){
        coins += amount;
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
}
