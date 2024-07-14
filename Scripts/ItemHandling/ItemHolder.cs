using FishNet.Connection;
using FishNet.Demo.AdditiveScenes;
using FishNet.Object;
using UnityEngine;

public class ItemHolder : NetworkBehaviour {
    [SerializeField] private InventoryManager inventoryManager;
    [SerializeField] private Transform rightHand;
    [SerializeField] private Animator animator;
    [SerializeField] private Collider hitboxCollider;

    private ItemSO currentItem;
    private GameObject currentItemObject;

    private WeaponCollisionDetector wcd;

    // Animations
    private const string TRIGGER_ATTACK = "Attack";

    private void InventoryManager_OnSelectedItemChanged(object sender, InventoryManager.OnSelectedItemChangedEventArgs e) {
        if (currentItemObject) {
            Destroy(currentItemObject);
        }

        if (e.itemId == -1) {
            currentItem = null;
            currentItemObject = null;
        }
        else {
            currentItem = ItemDatabase.Instance.GetItem(e.itemId);
            currentItemObject = Instantiate(currentItem.EquippedObject, rightHand);
            switch (currentItem.ItemType) {
                case (ItemType.Weapon):
                    wcd = currentItemObject.GetComponent<WeaponCollisionDetector>();
                    Physics.IgnoreCollision(hitboxCollider, wcd.WeaponCollider, true);
                    break;
            }
        }
    }

    private void GameInput_OnLeftClickStartedAction(object sender, System.EventArgs e) {
        if (!currentItem) {
            return;
        }

        switch (currentItem.ItemType) {
            case (ItemType.Weapon):
                AttackWithWeapon();
                break;
        }
    }

    private void AttackWithWeapon() {
        wcd.ClearAlreadyHitDamageReceivers();
        animator.SetTrigger(TRIGGER_ATTACK);
    }

    public void DisableWeaponCollider() {
        wcd.DisableCollider();
    }

    public void EnableWeaponCollider() {
        wcd.EnableCollider(); 
    }

    private void OnEnable() {
        inventoryManager.OnSelectedItemChanged += InventoryManager_OnSelectedItemChanged;
        GameInput.Instance.OnLeftClickStartedAction += GameInput_OnLeftClickStartedAction;
    }

    private void OnDisable() {
        inventoryManager.OnSelectedItemChanged -= InventoryManager_OnSelectedItemChanged;
        GameInput.Instance.OnLeftClickStartedAction -= GameInput_OnLeftClickStartedAction;
    }
}