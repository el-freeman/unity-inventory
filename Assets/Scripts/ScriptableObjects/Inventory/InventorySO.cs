using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEditor;
using System.Runtime.Serialization;

public enum InterfaceType
{
    Inventory,
    Equipment,
    Chest
}
[CreateAssetMenu(fileName = "New Inventory", menuName = "InventorySystem/Inventory")]
public class InventorySO : ScriptableObject
{
    public string savePath;
    public ItemDatabaseSO database;
    public InterfaceType type;
    public Inventory Container;
    public InventorySlot[] GetSlots { get { return Container.Slots; } }

    public bool AddItem(Item pickupItem, int pickupAmount)
    {
        if(EmptySlotCount<=0)
        {
            return false;
        }
        InventorySlot slot = FindItemOnInventory(pickupItem);
        if(!database.ItemObjects[pickupItem.Id].isStackable || slot == null)
        {
            SetEmptySlot(pickupItem, pickupAmount);
            return true;
        }
        slot.AddAmount(pickupAmount);
        return true;
    }
    public int EmptySlotCount
    {
        get
        {
            int counter = 0;
            for (int i = 0; i < GetSlots.Length; i++)
            {
                if(GetSlots[i].item.Id <= -1)
                {
                    counter++;
                }
            }
            return counter;
        }
    }
    public InventorySlot FindItemOnInventory(Item item)
    {
        for (int i = 0; i < GetSlots.Length; i++)
        {
            if(GetSlots[i].item.Id == item.Id)
            {
                return GetSlots[i];
            }
        }
        return null;
    }

    [ContextMenu("Save")]
    public void Save()
    {
        //string saveData = JsonUtility.ToJson(this, true);
        //BinaryFormatter bf = new BinaryFormatter();
        //FileStream file = File.Create(string.Concat(Application.persistentDataPath, savePath));
        //bf.Serialize(file, saveData);
        //file.Close();

        IFormatter formatter = new BinaryFormatter();
        Stream stream = new FileStream(string.Concat(Application.persistentDataPath, savePath), FileMode.Create, FileAccess.Write);
        formatter.Serialize(stream, Container);
        stream.Close();
    }
    [ContextMenu("Load")]
    public void Load()
    {
        if(File.Exists(string.Concat(Application.persistentDataPath, savePath)))
        {
            //BinaryFormatter bf = new BinaryFormatter();
            //FileStream file = File.Open(string.Concat(Application.persistentDataPath, savePath), FileMode.Open);
            //JsonUtility.FromJsonOverwrite(bf.Deserialize(file).ToString(), this);
            //file.Close();

            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(string.Concat(Application.persistentDataPath, savePath), FileMode.Open, FileAccess.Read);
            Inventory newContainer = (Inventory)formatter.Deserialize(stream);
            for (int i = 0; i < GetSlots.Length; i++)
            {
                GetSlots[i].UpdateSlot(newContainer.Slots[i].item, newContainer.Slots[i].amount);
            }
            stream.Close();
        }
    }
    [ContextMenu("Clear")]
    public void Clear()
    {
        Container.Clear();
    }
    
    public InventorySlot SetEmptySlot(Item pickupItem, int pickupAmount)
    {
        for (int i = 0; i < GetSlots.Length; i++)
        {
            if (GetSlots[i].item.Id <= -1)
            {
                GetSlots[i].UpdateSlot(pickupItem, pickupAmount);
                return GetSlots[i];
            }
        }
        //set up functionality for full inventory
        return null;
    }

    public void SwapItems(InventorySlot Item1, InventorySlot Item2)
    {
        if(Item2.CanPlaceInSlot(Item1.ItemObject) && Item1.CanPlaceInSlot(Item2.ItemObject))
        {
            InventorySlot temp = new InventorySlot(Item2.item, Item2.amount);
            Item2.UpdateSlot(Item1.item, Item1.amount);
            Item1.UpdateSlot(temp.item, temp.amount);
        }
    }

    public void RemoveItem(Item itemToRemove)
    {
        for (int i = 0; i < GetSlots.Length; i++)
        {
            if (GetSlots[i].item == itemToRemove)
            {
                GetSlots[i].UpdateSlot(null, 0);
            }
        }
    }
}

[System.Serializable]
public class Inventory
{
    public InventorySlot[] Slots = new InventorySlot[28];
    public void Clear()
    {
        for (int i = 0; i < Slots.Length; i++)
        {
            Slots[i].RemoveItem();
        }
    }
}

public delegate void SlotUpdated(InventorySlot slot);
[System.Serializable]
public class InventorySlot
{
    public ItemType[] AllowedItems = new ItemType[0];
    [System.NonSerialized]
    public UserInterface parent;
    [System.NonSerialized]
    public GameObject slotDisplay;
    [System.NonSerialized]
    public SlotUpdated OnAfterUpdate;
    [System.NonSerialized]
    public SlotUpdated OnBeforeUpdate;

    public Item item;  
    public int amount;

    public ItemSO ItemObject
    {
        get
        {
            if(item.Id >=0)
            {
                return parent.inventory.database.ItemObjects[item.Id];
            }
            return null;
        }
    }
    public InventorySlot()
    {
        UpdateSlot(new Item(), 0);        
    }

    public InventorySlot(Item pickupItem, int pickupAmount)
    {
        UpdateSlot(pickupItem, pickupAmount);
    }
    public void UpdateSlot(Item pickupItem, int pickupAmount)
    {       
        if(OnBeforeUpdate != null)
        {
            OnBeforeUpdate.Invoke(this);
        }
        item = pickupItem;
        amount = pickupAmount;
        if(OnAfterUpdate != null)
        {
            OnAfterUpdate.Invoke(this);
        }
    }
    public void RemoveItem()
    {
        UpdateSlot(new Item(), 0);
    }
    public void AddAmount(int value)
    {
        UpdateSlot(item, amount += value);        
    }
    public bool CanPlaceInSlot(ItemSO itemObject)
    {
        if(AllowedItems.Length <=0 || itemObject == null || itemObject.data.Id < 0)
        {
            return true;
        }
        for (int i = 0; i < AllowedItems.Length; i++)
        {
            if(itemObject.type == AllowedItems[i])
            {
                return true;
            }
        }
        return false;
    }
}
