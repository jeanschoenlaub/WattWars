using UnityEngine;

public class Bullet : MonoBehaviour
{

    [Header("Attributes")]
    [SerializeField] private float bulletSpeed = 5f;

    //Variables 
    private float elecDamage = 0; //Set Using method by tower script
    private float fuelDamage = 0; //Set Using method by tower script
    private Transform target;
    private float proximityDetectionRange = 0.1f; // If a bullet is closer than this to ennemy interacts

    public void SetTarget(Transform _target){
        target = _target; 
    }

    public void SetDamage(float _elecDamag, float _fuelDamage){
        elecDamage = _elecDamag; 
        fuelDamage = _fuelDamage;
    }

    private void Update() {
        if (!target) {
            Destroy(gameObject); // Destroy the projectile if the target is null
            return;
        }

        MoveTowardsTarget();
        CheckProximityAndInteract();
    }

    private void CheckProximityAndInteract() {
        if (Vector2.Distance(transform.position, target.position) < proximityDetectionRange) {
            HandleInteractionBasedOnTag();
            Destroy(gameObject); // Destroy the projectile after interacting
        }
    }

    private void MoveTowardsTarget() {
        int currentGameSpeed = LevelManager.GetGameSpeed(); // Assuming you have a method to adjust game speed dynamically
        float step = bulletSpeed * currentGameSpeed * Time.deltaTime; // Calculate the step size
        transform.position = Vector2.MoveTowards(transform.position, target.position, step);
    }

    private void HandleInteractionBasedOnTag() {
        if (target.CompareTag("Enemy")) {
            DealDamageToEnemy();
        }
        else if (target.CompareTag("Building")) {
            TransferEnergyToBuilding();
        }
        else if (target.CompareTag("Tower")) {
            TransferEnergyToConvertingTower();
        }
    }


    private void DealDamageToEnemy() {
        Health health = target.GetComponent<Health>();
        if (health != null) {
            health.TakeElecDamage(elecDamage);
            health.TakeFuelDamage(fuelDamage);
        }
    }


    private void TransferEnergyToBuilding() {
        GenerateMoney generateMoney = target.GetComponent<GenerateMoney>();
        if (generateMoney != null) {
            if (elecDamage != 0){
                generateMoney.ReceiveEnergy(elecDamage,0); 
            }
            if (fuelDamage != 0){
                generateMoney.ReceiveEnergy(0,fuelDamage); 
            }
        }
    }

    private void TransferEnergyToConvertingTower() {
        Turret tower = target.GetComponent<Turret>();
        tower.Convert();
    }

}

