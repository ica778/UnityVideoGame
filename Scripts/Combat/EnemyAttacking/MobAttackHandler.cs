using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobAttackHandler : MonoBehaviour {
    [SerializeField] private MobAttackCollisionDetector macd;
    [SerializeField] private Animator animator;

    private const string TRIGGER_ATTACK = "Attack";

    private void OnTriggerEnter(Collider other) {
        // TODO: MAKE THIS MORE PERMANENT. RIGHT NOW IT CHECKS IF OTHER LAYER IS 8 WHICH IS THE PLAYER LAYER
        if (other.gameObject.layer == 8) {
            animator.SetTrigger(TRIGGER_ATTACK);
        }
    }

}