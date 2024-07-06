using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class Weapon : NetworkBehaviour
{
    [SerializeField] private GameObject bullet;
    [SerializeField] private GameObject weaponDock;
    [SerializeField] private float bulletSpeed;
    [SerializeField] private float shootInterval;

    // public static Weapon Instance { get; private set; }

    public bool ready;
    private bool allowInvoke = true;

    private void Awake() {
        ready = true;

        // Instance = this;
    }
    private void Update()
    {
        
        // shooting = GameInput.Instance.Shoot();

        // if (IsOwner && ready && shooting){
        //     Shoot();
        // }
    }

    public void SetWeaponParent(Transform player){
        transform.parent = player;
    }

    [ServerRpc]
    private void ShootServerRpc(){
        ready = false;

        Vector3 shootDirection = transform.forward;
        GameObject currentBullet = Instantiate(bullet, weaponDock.transform.position, weaponDock.transform.rotation);
        NetworkObject networkObject = currentBullet.GetComponent<NetworkObject>();
        if (networkObject != null){
            networkObject.Spawn(true);
        }

        Rigidbody rb = currentBullet.GetComponent<Rigidbody>();
        if (rb != null){
            rb.AddForce(shootDirection.normalized * bulletSpeed);
        }
        if (allowInvoke){
            Invoke("ResetShot", shootInterval);
            allowInvoke = false;
        }
    }

    public void Shoot(){
        ShootServerRpc();
    }

    private void ResetShot(){
        ready = true;
        allowInvoke = true;
    }
}
