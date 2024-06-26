using UnityEngine;

public class Health : MonoBehaviour
{
    [Header("Attributes")]
    [SerializeField] public float elecLives = 2;
    [SerializeField] public float fuelLives = 2;
    [SerializeField] private float maxElecLives = 2;
    [SerializeField] private float maxFuelLives = 2;
    [SerializeField] private int killCoins = 0;

    [Header("UI References")]
    [SerializeField] private RectTransform elecLivesBar; 
    [SerializeField] private RectTransform fuelLivesBar;

    [Header("Death Effect")]
    [SerializeField] private GameObject deathEffectPrefab; // A prefab with a automatic death animation
    [SerializeField] private float deathEffectDuration = 1f;


    private bool isDestroyed = false;

    private AudioManager audioManager;

    private void Awake()
    {
        audioManager = GameObject.FindWithTag("Audio").GetComponent<AudioManager>();
    }   

    void Start() {
        UpdateHUD(); // Initial HUD update
    }

    public void TakeDamage(float dmg){

        if (elecLives >0){
            elecLives = Mathf.Max(elecLives - dmg, 0);
        }
        else if (fuelLives >0){
            fuelLives = Mathf.Max(fuelLives - dmg, 0);
        }

        //Update size of health bar
        UpdateHUD();

        // Display damages
        string textDamage = "-"+(dmg*10).ToString();
        Vector3 textDamagePosition = new Vector2(transform.position.x, transform.position.y+1); // Using parent's (enemy) position
        DynamicTextManager.CreateText2D(textDamagePosition, textDamage, DynamicTextManager.defaultData);
        
        CheckIfDestroyed();
    }

    void UpdateHUD()
    {
        // Update electric lives bar scale
        if (elecLivesBar != null) {
            Vector3 scale = elecLivesBar.localScale;
            if (elecLives == 0){scale.x = 0;}
            else { scale.x = elecLives / maxElecLives;}
            elecLivesBar.localScale = scale;
        }
        
        // Update fuel lives bar scale
        if (fuelLivesBar != null) {
            Vector3 scale = fuelLivesBar.localScale;
            if (fuelLives == 0){scale.x = 0;}
            else { scale.x = fuelLives / maxFuelLives;}
            fuelLivesBar.localScale = scale;
        }
    }
    // Checks if the enemy is destroyed (i.e., if either type of lives reaches 0)
    void CheckIfDestroyed()
    {
        if ( elecLives <= 0 && fuelLives <= 0 && !isDestroyed){
            WaveManager.onEnemyDestroy.Invoke();

            // Play a random electric enemy death SFX
            if (audioManager != null)
            {
                audioManager.PlayRandomElectricEnemyDeathSFX(volume: 1.5f); // Adjusted volume
            }

            // Instantiate a temporary death effect prefab
            if (deathEffectPrefab != null){
                GameObject deathEffect = Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);
                Destroy(deathEffect, deathEffectDuration);
            }

            Destroy(gameObject);
            isDestroyed = true;
        }
    }
}
