using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Weapon : MonoBehaviour
{
    [SerializeField] private GameObject bullet;
    [SerializeField] private GameObject weaponDock;
    [SerializeField] private float bulletSpeed;
    [SerializeField] private float shootInterval;
    [SerializeField] private float reloadTime;
    [SerializeField] private int magSize;

    public static Weapon Instance { get; private set; }

    private int bulletsLeft, bulletsShot;
    private bool shooting, ready, reloading;
    private bool allowInvoke = true;
    private void Awake() {
        bulletsLeft = magSize;
        ready = true;

        Instance = this;
    }
    private void Update()
    {
        Vector2 inputVector = GameInput.Instance.GetAimVectorNormalized();
        Vector3 aimDirection = new Vector3(inputVector.x, 0f, inputVector.y);

        float rotateSpeed = 13f;
        transform.position = Player.Instance.transform.position;
        transform.forward = Vector3.Slerp(transform.forward, aimDirection, Time.deltaTime * rotateSpeed);

        
        shooting = GameInput.Instance.Shoot();
        
        
        if (Input.GetKeyDown(KeyCode.R) && bulletsLeft < magSize && !reloading){
            Reload();
        }

        if (ready && shooting && !reloading && bulletsLeft <= 0){
            Reload();
        }

        if (ready && shooting && !reloading && bulletsLeft > 0){
            bulletsShot = 0;
            Shoot();
        }
    }

    private void Shoot(){
        Debug.Log("Shooting!");
        ready = false;

        Vector3 shootDirection = transform.forward;
        GameObject currentBullet = Instantiate(bullet, weaponDock.transform.position, Quaternion.identity);
        currentBullet.transform.forward = shootDirection.normalized;
        currentBullet.GetComponent<Rigidbody>().velocity = shootDirection.normalized * bulletSpeed;
        if (allowInvoke){
            Invoke("ResetShot", shootInterval);
            allowInvoke = false;
        }

        bulletsLeft--;
        bulletsShot++;
    }

    private void ResetShot(){
        ready = true;
        allowInvoke = true;
    }

    private void Reload(){
        reloading = true;
        Invoke("ReloadFinished", reloadTime);
    }

    private void ReloadFinished(){
        bulletsLeft = magSize;
        reloading = false;
    }

}
