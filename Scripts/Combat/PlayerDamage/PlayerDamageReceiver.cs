using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDamageReceiver : DamageReceiver {
    private void OnTriggerEnter(Collider other) {
        MobAttackCollisionDetector macd = other.GetComponent<MobAttackCollisionDetector>();

        if (macd) {
            Debug.Log("TESTING PLAYER GETTING HIT FOR DAMAGE: " + macd.Damage);
            characterHealth.ApplyDamage(macd.Damage);
        }
    }
}