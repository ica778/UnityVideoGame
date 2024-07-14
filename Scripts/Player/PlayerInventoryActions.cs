using FishNet.Connection;
using FishNet.Managing.Server;
using FishNet.Object;
using FishNet.Observing;
using HeathenEngineering.SteamworksIntegration;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.UI.GridLayoutGroup;

public class PlayerInventoryActions : NetworkBehaviour {
    [SerializeField] private InventoryManager inventoryManager;
    [SerializeField] private GameObject cameraTarget;
    [SerializeField] private LayerMask layersYouCantDropItemInto;

    public override void OnStartNetwork() {
        if (!Owner.IsLocalClient) {
            this.enabled = false;
            return;
        }
    }

    public void DropCurrentItem() {
        ItemSO item = inventoryManager.GetSelectedItem(true);
        if (item) {
            Vector3 dropItemPosition = cameraTarget.transform.position + (cameraTarget.transform.forward * 1.5f);
            if (!Physics.Raycast(cameraTarget.transform.position, cameraTarget.transform.forward, 1.5f, layersYouCantDropItemInto)) {
                DropItemServerRpc(item.GroundLootObject, dropItemPosition, cameraTarget.transform.rotation);
            }
            else {
                dropItemPosition = cameraTarget.transform.position;
                DropItemServerRpc(item.GroundLootObject, dropItemPosition, cameraTarget.transform.rotation);
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void DropItemServerRpc(GameObject groundLootItemToDrop, Vector3 dropItemPosition, Quaternion quaternion) {
        GameObject item = Instantiate(groundLootItemToDrop, dropItemPosition, quaternion);
        ServerManager.Spawn(item);
    }

    public void PickupGroundLoot(GroundLoot groundLoot) {
        if (inventoryManager.CanAddItem(groundLoot.GetItem())) {
            groundLoot.gameObject.SetActive(false);
            PickupItemObjectServerRpc(base.LocalConnection, groundLoot.gameObject);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void PickupItemObjectServerRpc(NetworkConnection conn, GameObject itemToPickup) {
        if (!itemToPickup.GetComponent<NetworkObject>().IsDeinitializing) {
            ServerManager.Despawn(itemToPickup.gameObject, DespawnType.Destroy);
            PickupItemTargetRpc(conn, itemToPickup);
        }
    }

    [TargetRpc]
    private void PickupItemTargetRpc(NetworkConnection conn, GameObject itemToPickup) {
        GroundLoot groundLoot = itemToPickup.GetComponent<GroundLoot>();
        inventoryManager.AddItem(groundLoot.GetItem());
    }

    private void GameInput_OnDropAction(object sender, System.EventArgs e) {
        DropCurrentItem();
    }

    private void OnEnable() {
        GameInput.Instance.OnDropAction += GameInput_OnDropAction;
    }

    private void OnDisable() {
        GameInput.Instance.OnDropAction -= GameInput_OnDropAction;
    }
}