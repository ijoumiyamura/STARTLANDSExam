using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class Damagable : NetworkBehaviour
{
    public float health;
    [SerializeField] private GameObject damagedVersion;

    public void TakeDamage(){
        health --;
        Debug.Log(health);
        if (health <= 0){     
            RequestDamageServerRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestDamageServerRpc()
    {
        RequestSpawnDamagedClientRpc();
        DestroyServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void DestroyServerRpc(){
        NetworkObject networkObject = GetComponent<NetworkObject>();
        
        if (networkObject != null)
        {
            networkObject.Despawn(true);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawnDamagedServerRpc(){
        GameObject newVersion = Instantiate(damagedVersion, transform.position, transform.rotation);
        NetworkObject newNetworkObject = newVersion.GetComponent<NetworkObject>();

        if (newNetworkObject != null){
            newNetworkObject.Spawn(true);
        }
    }

    [ClientRpc]
    private void RequestSpawnDamagedClientRpc(){
        if (IsClient){
            SpawnDamagedServerRpc();
        }
    }
}
