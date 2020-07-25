using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEditor;
using System.Runtime.Serialization;

[CreateAssetMenu(fileName = "New Inventory", menuName = "InventorySystem/Inventory")]
public class InventorySO : ScriptableObject
{
    public string savePath;
    public ItemDatabaseSO database;
    public Inventory Container;

    public void AddItem(Item pickupItem, int pickupAmount)
    {
        if (pickupItem.buffs.Length > 0)
        {
            SetEmptySlot(pickupItem, pickupAmount);
            return;
        }

        for (int i = 0; i < Container.Items.Length; ++i)
        {
            if (Container.Items[i].ID == pickupItem.Id)
            {
                Container.Items[i].AddAmount(pickupAmount);
                return;
            }
        }
        SetEmptySlot(pickupItem, pickupAmount);
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
            for (int i = 0; i < Container.Items.Length; i++)
            {
                Container.Items[i].UpdateSlot(newContainer.Items[i].ID, newContainer.Items[i].item, newContainer.Items[i].amount);
            }
            stream.Close();
        }
    }
    [ContextMenu("Clear")]
    public void Clear()
    {
        Container = new Inventory();
    }


    //public string savePath;
    //public ItemDatabaseSO database;
    //public Inventory Container;

    //public void AddItem(Item pickupItem, int pickupAmount)
    //{
    //    if (pickupItem.buffs.Length > 0)
    //    {
    //        SetEmptySlot(pickupItem, pickupAmount);
    //        return;
    //    }
    //}

    public InventorySlot SetEmptySlot(Item pickupItem, int pickupAmount)
    {
        for (int i = 0; i < Container.Items.Length; i++)
        {
            if (Container.Items[i].ID <= -1)
            {
                Container.Items[i].UpdateSlot(pickupItem.Id, pickupItem, pickupAmount);
                return Container.Items[i];
            }
        }
        //set up functionality for full inventory
        return null;
    }

    public void MoveItem(InventorySlot Item1, InventorySlot Item2)
    {
        InventorySlot temp = new InventorySlot(Item2.ID, Item2.item, Item2.amount);
        Item2.UpdateSlot(Item1.ID, Item1.item, Item1.amount);
        Item1.UpdateSlot(temp.ID, temp.item, temp.amount);
    }

    public void RemoveItem(Item itemToRemove)
    {
        for (int i = 0; i < Container.Items.Length; i++)
        {
            if (Container.Items[i].item == itemToRemove)
            {
                Container.Items[i].UpdateSlot(-1, null, 0);
            }
        }
    }
}

[System.Serializable]
public class Inventory
{
    public InventorySlot[] Items = new InventorySlot[28];
}

[System.Serializable]
public class InventorySlot
{
    public int ID = -1;
    public Item item;  
    public int amount;
    public InventorySlot()
    {
        ID = -1;
        item = null;
        amount = 0;
    }

    public InventorySlot(int pickupID, Item pickupItem, int pickupAmount)
    {
        ID = pickupID;
        item = pickupItem;
        amount = pickupAmount;
    }
    public void UpdateSlot(int pickupID, Item pickupItem, int pickupAmount)
    {
        ID = pickupID;
        item = pickupItem;
        amount = pickupAmount;
    }
    public void AddAmount(int value)
    {
        amount += value;
    }
}
