using FishNet.Connection;
using FishNet.Object;
using UnityEngine;

// NOTE: PUT THIS SCRIPT ON THE HITBOXES PARENT. SO THE GAME OBJECT THIS SCRIPT IS ON SHOULD BE PARENT OF COLLIDERS WHICH ARE HITBOXES.
public class DamageReceiver : NetworkBehaviour {
    [SerializeField] protected CharacterHealth characterHealth;

    protected override void OnValidate() {
        if (!characterHealth) {
            Debug.LogError("characterHealth is not assigned!", this);
        }
    }

    public override void OnStartNetwork() {
        if (Owner.IsLocalClient) {
            this.enabled = false;
        }
    }

    public void ReceiveHit(int damage) {
        Debug.Log("TESTING RECEIVING HIT FOR DAMAGE: " + damage);
        characterHealth.ApplyDamage(damage);
        ReceiveHitServerRpc(damage, base.LocalConnection);
    }

    [ServerRpc(RequireOwnership = false)]
    protected void ReceiveHitServerRpc(int damage, NetworkConnection conn) {
        ReceiveHitObserversRpc(damage, conn);
    }

    [ObserversRpc]
    protected void ReceiveHitObserversRpc(int damage, NetworkConnection conn) {
        if (base.LocalConnection == conn) {
            return;
        }
        characterHealth.ApplyDamage(damage);
        Debug.Log("TESTING RECEIVING HIT BY ANOTHER PLAYER FOR DAMAGE: " + damage);
    }
}