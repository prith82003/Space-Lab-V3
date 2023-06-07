using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventoryItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
	[SerializeField] private Item item = null;
	public InventorySlot slot = null;

	private void Awake()
	{
		UpdateSprite();
	}

	private void UpdateSprite()
	{
		if (item == null) return;
		try
		{
			GetComponent<SpriteRenderer>().sprite = item.GetSprite();
		}
		catch
		{
			try
			{
				Image img = GetComponent<Image>();
				img.sprite = item.GetSprite();
			}
			catch { }
		}
	}

	public void SetItem(Item item)
	{
		this.item = item;
		UpdateSprite();
	}

	public Item GetItem() => item;

	public void PickUp(GameObject obj) => item.PickUp(obj);

	public void OnBeginDrag(PointerEventData eventData)
	{
		GetComponent<Image>().raycastTarget = false;
		InventoryManager.BeginDragItem(slot, this);
	}

	public void OnDrag(PointerEventData eventData)
	{
		transform.position += (Vector3)eventData.delta;
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		GetComponent<Image>().raycastTarget = true;
		InventoryManager.EndDragItem(this);
	}
}
