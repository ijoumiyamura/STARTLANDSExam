using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    [SerializeField] private Player player;
    private Animator animator;
    private void Awake() {
        animator = GetComponent<Animator>();
        animator.SetBool("IsWalking", player.IsWalking());
    }

    private void Update() {
        animator.SetBool("IsWalking", player.IsWalking());
    }
}
