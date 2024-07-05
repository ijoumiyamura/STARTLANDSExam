using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float bulletDamage = 1f;

    private void OnCollisionEnter(Collision other) {
        Destroy(gameObject);    
    }
}
