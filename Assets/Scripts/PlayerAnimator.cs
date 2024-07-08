using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerAnimator : NetworkBehaviour
{
    [SerializeField] private Player player;
    private Animator animator;
    private void Awake() {
        animator = GetComponent<Animator>();
        animator.SetBool("IsWalking", player.IsWalking());
    }

    private void Update() {
        if (!IsOwner) return;
        animator.SetBool("IsWalking", player.IsWalking());
        animator.SetBool("IsRunning", player.IsRunning());
    }
}
