﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DynamicInterface : UserInterface
{
    public GameObject inventoryPrefab;
    public int xStart;
    public int yStart;
    public int xSpaceBetweenItem;
    public int ySpaceBetweenItem;
    public int NumberOfColumn;
    public override void CreateSlot()
    {
        slotsOnInterface = new Dictionary<GameObject, InventorySlot>();

        for (int i = 0; i < inventory.GetSlots.Length; i++)
        {
            var obj = Instantiate(inventoryPrefab, Vector3.zero, Quaternion.identity, transform);
            obj.GetComponent<RectTransform>().localPosition = GetPosition(i);

            AddEvent(obj, EventTriggerType.PointerEnter, delegate { OnEnter(obj); });
            AddEvent(obj, EventTriggerType.PointerExit, delegate { OnExit(obj); });
            AddEvent(obj, EventTriggerType.BeginDrag, delegate { OnDragStart(obj); });
            AddEvent(obj, EventTriggerType.EndDrag, delegate { OnDragEnd(obj); });
            AddEvent(obj, EventTriggerType.Drag, delegate { OnDrag(obj); });
            inventory.GetSlots[i].slotDisplay = obj;

            slotsOnInterface.Add(obj, inventory.GetSlots[i]);

        }
    }
    private Vector3 GetPosition(int i)
    {
        return new Vector3(xStart + (xSpaceBetweenItem * (i % NumberOfColumn)), yStart + (-ySpaceBetweenItem * (i / NumberOfColumn)), 0f);
    }
}
