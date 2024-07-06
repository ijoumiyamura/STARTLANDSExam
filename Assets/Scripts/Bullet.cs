using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Bullet : NetworkBehaviour 
{
    [SerializeField] private float bulletDamage = 1f;

    private void OnCollisionEnter(Collision other) {
        Destroy(gameObject);    
    }
}
