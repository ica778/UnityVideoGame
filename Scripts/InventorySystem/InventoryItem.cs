using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {
    [SerializeField] private Image image;
    [SerializeField] private TextMeshProUGUI countText;

    private ItemSO item;
    private Transform parentBeforeDrag;
    private Transform parentAfterDrag;
    private int count = 1;

    public void InitialiseItem(ItemSO newItem) {
        item = newItem;
        image.sprite = newItem.Sprite;
        UpdateCount();
    }

    public void UpdateCount() {
        countText.text = count.ToString();
        countText.gameObject.SetActive(count > 1);
    }

    public void OnBeginDrag(PointerEventData eventData) {
        parentBeforeDrag = transform.parent;
        parentAfterDrag = transform.parent;
        transform.SetParent(transform.root);
        transform.SetAsLastSibling();
        image.raycastTarget = false;
        countText.raycastTarget = false;
    }

    public void OnDrag(PointerEventData eventData) {
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData) {
        transform.SetParent(parentAfterDrag);
        image.raycastTarget = true;
        countText.raycastTarget = true;
    }

    public Transform GetParentBeforeDrag() { 
        return parentBeforeDrag; 
    }
    
    public Transform GetParentAfterDrag() {
        return parentAfterDrag;
    }

    public void SetParentAfterDrag(Transform parentAfterDrag) {
        this.parentAfterDrag = parentAfterDrag;
    }

    public ItemSO GetItem() { 
        return item; 
    }

    public int GetCount() {
        return count;
    }

    public void SetCount(int count) {
        this.count = count;
    }

    public bool IsStackable() {
        return item.IsStackable;
    }

    public int GetMaxStackCount() {
        return item.GetMaxStackCount();
    }
}