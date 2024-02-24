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
        if (!target) return;

        Vector2 direction = (target.position - transform.position).normalized;

        electron.velocity = direction * electronSpeed;
    }

    private void OnCollisionEnter2D(Collision2D other){
        other.gameObject.GetComponent<Health>().TakeDamage(damagePoint);
        Destroy(gameObject); //Destroys the electron
    }
}

