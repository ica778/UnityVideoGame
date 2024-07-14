using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobAttackCollisionDetector : MonoBehaviour {
    [SerializeField] private int damage;

    private Collider weaponCollider;

    private void Awake() {
        weaponCollider = GetComponent<Collider>();
        DisableCollider();
    }

    public int Damage => damage;

    public void DisableCollider() {
        weaponCollider.enabled = false;
    }

    public void EnableCollider() {
        weaponCollider.enabled = true;
    }
}
