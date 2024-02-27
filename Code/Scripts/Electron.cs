using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Electron : MonoBehaviour
{
    
    [Header("References")]
    [SerializeField] private Rigidbody2D electron;

    [Header("Attributes")]
    [SerializeField] private float electronSpeed = 5f;
    [SerializeField] private int damagePoint = 1;

    
    private Transform target;

    public void SetTarget(Transform _target){
        target = _target; 
    }

    private void FixedUpdate() {
        if (!target) {
            Destroy(gameObject); // Destroy the electron if the target is null
            return;
        }

        Vector2 direction = (target.position - transform.position).normalized;

        int currentGameSpeed = LevelManager.GetGameSpeed();
        electron.velocity = direction * electronSpeed * currentGameSpeed;
    }

    private void OnCollisionEnter2D(Collision2D other){
        Destroy(gameObject); //Destroys the electron
        other.gameObject.GetComponent<Health>().TakeDamage(damagePoint);
    }
}

