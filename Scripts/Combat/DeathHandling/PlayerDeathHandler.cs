using ECM2;
using FishNet.Demo.AdditiveScenes;
using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeathHandler : BaseDeathHandler {
    [SerializeField] private InventorySyncing inventorySyncing;
    [SerializeField] private Player player;
    [SerializeField] private NetworkObject playerCharacter;

    public bool isDead = false;

    public override void OnStartClient() {
        if (!(Owner.IsLocalClient || base.IsServerInitialized)) {
            this.gameObject.SetActive(false);
            return;
        }

        base.ServerManager.Objects.OnPreDestroyClientObjects += ServerManager_OnPreDestroyClientObjects;
    }

    public override void OnStopClient() {
        base.ServerManager.Objects.OnPreDestroyClientObjects -= ServerManager_OnPreDestroyClientObjects;
    }

    private void ServerManager_OnPreDestroyClientObjects(FishNet.Connection.NetworkConnection conn) {
        if (base.Owner == conn) {
            base.RemoveOwnership();
            inventorySyncing.RemoveOwnership();
            playerCharacter.RemoveOwnership();
           
            if (!isDead) {
                isDead = true;
                DropInventoryServerRpc();
                SpawnCorpseServerRpc(conn);
            }
        }
    }

    public override void OnCharacterDeath() {
        isDead = true;

        if (Owner.IsLocalClient) {
            EnableCorpse();

            SpawnCorpseServerRpc(base.LocalConnection);
        }


        if (base.IsServerInitialized) {
            DropInventoryServerRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawnCorpseServerRpc(FishNet.Connection.NetworkConnection conn) {
        SpawnCorpseObserversRpc(conn);
    }

    [ObserversRpc]
    private void SpawnCorpseObserversRpc(FishNet.Connection.NetworkConnection conn) {
        if (base.LocalConnection == conn) {
            return;
        }

        EnableCorpse();
    }

    private void EnableCorpse() {
        Rigidbody rb = playerCharacter.GetComponent<Rigidbody>();
        rb.isKinematic = false;
        rb.useGravity = true;
        rb.drag = 0f;
        rb.angularDrag = 0f;

        CapsuleCollider cc = playerCharacter.GetComponent<CapsuleCollider>();
        cc.material = null;

        playerCharacter.GetComponent<PlayerController>().enabled = false;
        playerCharacter.GetComponent<CharacterMovement>().enabled = false;
        playerCharacter.GetComponent<PlayerCharacter>().enabled = false;
    }

    [ServerRpc(RequireOwnership = false)]
    public void DropInventoryServerRpc() {
        StartCoroutine(DropInventoryAsync());
    }

    private IEnumerator DropInventoryAsync() {
        for (int i = 0; i < inventorySyncing.InventoryItemIds.Length; i++) {
            if (inventorySyncing.InventoryItemCounts[i] > 0) {
                for (int j = 0; j < inventorySyncing.InventoryItemCounts[i]; j++) {
                    ItemSO item = ItemDatabase.Instance.GetItem(inventorySyncing.InventoryItemIds[i]);
                    AsyncInstantiateOperation<GameObject> asyncInstantiateOperation = InstantiateAsync(item.GroundLootObject, player.PlayerCharacter.transform.position, Quaternion.identity);

                    while (!asyncInstantiateOperation.isDone) {
                        yield return null;
                    }

                    GameObject itemObject = asyncInstantiateOperation.Result[0];
                    base.ServerManager.Spawn(itemObject);
                }

                inventorySyncing.InventoryItemCounts[i] = 0;
            }
        }
    }
}
