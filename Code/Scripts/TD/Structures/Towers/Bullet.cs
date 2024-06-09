using UnityEngine;

public enum BulletType
{
    Elec,
    Fuel,
}

public class Bullet : MonoBehaviour
{
    [Header("Attributes")]
    [SerializeField] private float bulletSpeed = 5f;
    [SerializeField] public BulletType bulletType;

    //Variables 
    private float bulletDamage = 0; //Set Using method by tower script
    
    private Transform target;
    private float proximityDetectionRange = 0.1f; // If a bullet is closer than this to ennemy interacts

    private AudioManager audioManager;
    private void Awake()
    {
        audioManager = GameObject.FindWithTag("Audio").GetComponent<AudioManager>();
    }   

    public void SetTarget(Transform _target){
        target = _target; 
    }

    public void SetDamage(float _bulletDamage){
        bulletDamage = _bulletDamage; 
    }

    public void SetBulletType(BulletType _bulletType){
        bulletType = _bulletType; 
    }

    private void Update() {

        // Currently we can target an enemy that will get destroyed so if no target destroy 
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
            PlayEnemyHitSFX();
        }
        else if (target.CompareTag("Building")) {
            TransferEnergyToBuilding();
        }
        else if (target.CompareTag("Tower")) {
            TransferEnergyToBatteryTower();
        }
    }

    private void DealDamageToEnemy() {
        Health health = target.GetComponent<Health>();
        if (health != null) {
            health.TakeDamage(bulletDamage);
        }else {
            Debug.Log("Trying to deal health damage, but couldn't find script");
        }
    }

    private void PlayEnemyHitSFX(){
        if (bulletType == BulletType.Elec){
            audioManager.PlaySFX(audioManager.ElectricEnemyHit);
        }else{
            audioManager.PlaySFX(audioManager.FuelEnemyHit);
        }
    }

    // To-Do change health to take damage regardless of type
    private void TransferEnergyToBuilding() {
        BuildingTower building = target.GetComponent<BuildingTower>();
        if (building != null) {
            building.ReceiveEnergy(bulletDamage); 
        }
    }

    private void TransferEnergyToBatteryTower() {
        Turret tower = target.GetComponent<Turret>();
        if (tower != null) {
            tower.Charge(bulletDamage);
        } else {
            Debug.Log("Trying to charge tower, but couldn't find scritp");
        }
    }

}
