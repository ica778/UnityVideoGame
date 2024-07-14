using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.Progress;

public class InventorySyncing : NetworkBehaviour {
    [SerializeField] private InventoryManager inventoryManager;
    [SerializeField] private Player player;

    // this array holds item id's of contents
    [SerializeField] private int[] inventoryItemIds;
    [SerializeField] private int[] inventoryItemCounts;

    public int[] InventoryItemIds => inventoryItemIds;
    public int[] InventoryItemCounts => inventoryItemCounts;

    public override void OnStartClient() {
        if (!(Owner.IsLocalClient || base.IsServerInitialized)) {
            this.gameObject.SetActive(false);
            return;
        }

        InitializeServerRpc(inventoryManager.GetInventorySize());
    }

    [ServerRpc(RequireOwnership = false)]
    private void InitializeServerRpc(int size) {
        inventoryItemIds = new int[size];
        inventoryItemCounts = new int[size];
    }

    [ServerRpc(RequireOwnership = false)]
    public void AddItemServerRpc(int pos, int itemId, int count) {
        inventoryItemIds[pos] = itemId;
        inventoryItemCounts[pos] = count;
    }

    [ServerRpc(RequireOwnership = false)]
    public void UpdateItemCountServerRpc(int pos, int newCount) {
        inventoryItemCounts[pos] = newCount;
    }
}
