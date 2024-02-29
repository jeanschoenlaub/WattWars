using UnityEngine;

public class Health : MonoBehaviour
{
    [Header("Attributes")]
    [SerializeField] public int elecLives = 2;
    [SerializeField] public int fuelLives = 2;
    [SerializeField] private int maxElecLives = 2;
    [SerializeField] private int maxFuelLives = 2;
    [SerializeField] private int killCoins = 5;

    [Header("UI References")]
    [SerializeField] private RectTransform elecLivesBar; // Assign in the Inspector
    [SerializeField] private RectTransform fuelLivesBar; // Assign in the Inspector

    private bool isDestroyed = false;

    void Start() {
        UpdateHUD(); // Initial HUD update
    }

    public void TakeElecDamage(int dmg){
        elecLives = Mathf.Max(elecLives - dmg, 0);
        UpdateHUD();

        CheckIfDestroyed();
    }

    public void TakeFuelDamage(int dmg){
        fuelLives = Mathf.Max(fuelLives - dmg, 0);
        UpdateHUD();

        CheckIfDestroyed();
    }

    void UpdateHUD()
    {
        // Update electric lives bar scale
        if (elecLivesBar != null) {
            Vector3 scale = elecLivesBar.localScale;
            scale.x = (float)elecLives / maxElecLives;
            elecLivesBar.localScale = scale;
        }
        
        // Update fuel lives bar scale
        if (fuelLivesBar != null) {
            Vector3 scale = fuelLivesBar.localScale;
            if (fuelLives == 0){scale.x = 0;}
            else { scale.x = (float)fuelLives / maxFuelLives;}
            fuelLivesBar.localScale = scale;
        }
    }
    // Checks if the enemy is destroyed (i.e., if either type of lives reaches 0)
    void CheckIfDestroyed()
    {
        if ( elecLives <= 0 && fuelLives <= 0 && !isDestroyed){
            EnemySpawner.onEnemyDestroy.Invoke();
            LevelManager.main.IncreaseCurrency(killCoins);
            Destroy(gameObject);
            isDestroyed = true;
        }
    }
}
