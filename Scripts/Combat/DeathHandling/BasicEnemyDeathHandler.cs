using FishNet.Connection;
using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BasicEnemyDeathHandler : BaseDeathHandler {
    [SerializeField] private EnemyDropsHandler dropHandler;
    [SerializeField] private BasicEnemyAI basicEnemyAI;
    [SerializeField] private Rigidbody rb;

    private void Start() {
        rb.isKinematic = true;
    }

    public override void OnCharacterDeath() {
        basicEnemyAI.Disable();
        rb.isKinematic = false;

        if (base.IsServerInitialized) {
            dropHandler.SpawnDropsServerRpc(rb.transform.position);
        }
    }
}