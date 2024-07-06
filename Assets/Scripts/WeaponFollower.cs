using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponFollower : MonoBehaviour
{
    public Transform target;

    private void Update(){
        if (target != null){
            transform.parent = target;
        }
    }
}
