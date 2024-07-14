using FishNet.Object;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class InventoryManager : MonoBehaviour {
    [SerializeField] private InventorySyncing inventorySyncing;
    [SerializeField] private InventorySlot[] inventorySlots;
    [SerializeField] private GameObject inventoryItemPrefab;
    [SerializeField] private int inventoryToolbarSize = 6;

    private int selectedSlotIndex;

    public event EventHandler<OnSelectedItemChangedEventArgs> OnSelectedItemChanged;
    public class OnSelectedItemChangedEventArgs : EventArgs { 
        public int itemId;
    }

    private void OnEnable() {
        ChangeSelectedSlotIndex(0);

        GameInput.Instance.OnScrollUpAction += GameInput_OnScrollUpAction;
        GameInput.Instance.OnScrollDownAction += GameInput_OnScrollDownAction;
    }

    private void OnDisable() {
        GameInput.Instance.OnScrollUpAction -= GameInput_OnScrollUpAction;
        GameInput.Instance.OnScrollDownAction -= GameInput_OnScrollDownAction;
    }

    private void GameInput_OnScrollUpAction(object sender, System.EventArgs e) {
        DecrementSelectedSlotIndex();
    }

    private void GameInput_OnScrollDownAction(object sender, System.EventArgs e) {
        IncrementSelectedSlotIndex();
    }

    private void ChangeSelectedSlotIndex(int slotIndex) {
        if (selectedSlotIndex >= 0) {
            inventorySlots[selectedSlotIndex].Deselect();
        }

        inventorySlots[slotIndex].Select();
        selectedSlotIndex = slotIndex;
        if (inventorySlots[selectedSlotIndex].GetComponentInChildren<InventoryItem>()) {
            OnSelectedItemChanged?.Invoke(this, new OnSelectedItemChangedEventArgs {
                //meshFilter = GetSelectedItem(false).GetGroundLootPrefab().GetComponentInChildren<MeshFilter>()
                itemId = GetSelectedItem(false).Id
            });
        }
        else {
            OnSelectedItemChanged?.Invoke(this, new OnSelectedItemChangedEventArgs {
                //meshFilter = null
                itemId = -1
            });
        }
    }

    private void IncrementSelectedSlotIndex() {
        if (selectedSlotIndex >= inventoryToolbarSize - 1) {
            ChangeSelectedSlotIndex(0);
        }
        else {
            ChangeSelectedSlotIndex(selectedSlotIndex + 1);
        }
    }

    private void DecrementSelectedSlotIndex() {
        if (selectedSlotIndex <= 0) {
            ChangeSelectedSlotIndex(inventoryToolbarSize - 1);
        }
        else {
            ChangeSelectedSlotIndex(selectedSlotIndex - 1);
        }
    }

    public bool AddItem(ItemSO item) {
        for (int i = 0; i < inventorySlots.Length; i++) {
            InventorySlot currentInventorySlot = inventorySlots[i];
            InventoryItem currentInventoryItem = currentInventorySlot.GetComponentInChildren<InventoryItem>();
            if (
                currentInventoryItem && 
                currentInventoryItem.GetItem() == item && 
                currentInventoryItem.IsStackable() && 
                currentInventoryItem.GetCount() < currentInventoryItem.GetMaxStackCount()
             ){
                currentInventoryItem.SetCount(currentInventoryItem.GetCount() + 1);
                inventorySyncing.AddItemServerRpc(i, item.Id, currentInventoryItem.GetCount());

                currentInventoryItem.UpdateCount();

                return true;
            }
        }

        for (int i = 0; i < inventorySlots.Length; i++) {
            InventorySlot currentInventorySlot = inventorySlots[i];
            InventoryItem currentInventoryItem = currentInventorySlot.GetComponentInChildren<InventoryItem>();
            if (!currentInventoryItem) {
                SpawnNewItem(item, currentInventorySlot);
                inventorySyncing.AddItemServerRpc(i, item.Id, 1);
                if (i == selectedSlotIndex) {
                    OnSelectedItemChanged?.Invoke(this, new OnSelectedItemChangedEventArgs {
                        itemId = item.Id
                    });
                }
                return true;
            }
        }
        return false;
    }

    public bool CanAddItem(ItemSO item) {
        for (int i = 0; i < inventorySlots.Length; i++) {
            InventorySlot currentInventorySlot = inventorySlots[i];
            InventoryItem currentInventoryItem = currentInventorySlot.GetComponentInChildren<InventoryItem>();
            if (
                currentInventoryItem &&
                currentInventoryItem.GetItem() == item &&
                currentInventoryItem.IsStackable() &&
                currentInventoryItem.GetCount() < currentInventoryItem.GetMaxStackCount()
             ) {
                return true;
            }
        }

        for (int i = 0; i < inventorySlots.Length; i++) {
            InventorySlot currentInventorySlot = inventorySlots[i];
            InventoryItem currentInventoryItem = currentInventorySlot.GetComponentInChildren<InventoryItem>();
            if (!currentInventoryItem) {
                return true;
            }
        }
        return false;
    }

    private void SpawnNewItem(ItemSO item, InventorySlot inventorySlot) {
        GameObject newItem = Instantiate(inventoryItemPrefab, inventorySlot.transform);
        InventoryItem inventoryItem = newItem.GetComponent<InventoryItem>();
        inventoryItem.InitialiseItem(item);
    }

    public ItemSO GetSelectedItem(bool decrementItemCount) {
        InventorySlot inventorySlot = inventorySlots[selectedSlotIndex];
        InventoryItem inventoryItemInSlot = inventorySlot.GetComponentInChildren<InventoryItem>();
        if (inventoryItemInSlot) {
            ItemSO item = inventoryItemInSlot.GetItem();
            if (decrementItemCount) {
                inventoryItemInSlot.SetCount(inventoryItemInSlot.GetCount() - 1);
                if (inventoryItemInSlot.GetCount() <= 0) {
                    Destroy(inventoryItemInSlot.gameObject);
                    OnSelectedItemChanged?.Invoke(this, new OnSelectedItemChangedEventArgs {
                        //meshFilter = null
                        itemId = -1
                    });
                }
                else {
                    inventoryItemInSlot.UpdateCount();
                }
                inventorySyncing.UpdateItemCountServerRpc(selectedSlotIndex, inventoryItemInSlot.GetCount());
            }

            return item;
        }
        return null;
    }

    public int GetInventorySize() {
        return inventorySlots.Length;
    }
}