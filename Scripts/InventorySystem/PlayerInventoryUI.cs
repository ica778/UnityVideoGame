using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventoryUI : MonoBehaviour {
    [SerializeField] private GameObject backpackInventoryUI;

    private void Start() {
        GameInput.Instance.OnInventoryAction += GameInput_OnInventoryAction;
        backpackInventoryUI.SetActive(false);
    }

    private void GameInput_OnInventoryAction(object sender, System.EventArgs e) {
        if (backpackInventoryUI.activeInHierarchy) {
            Hide();
            GameInput.Instance.LockCursor();
        }
        else {
            Show();
            GameInput.Instance.UnlockCursor();
        }
    }

    private void Hide() {
        backpackInventoryUI.SetActive(false);
    }

    private void Show() {
        backpackInventoryUI.SetActive(true);
    }

    private void OnDestroy() {
        GameInput.Instance.OnInventoryAction -= GameInput_OnInventoryAction;
    }
}