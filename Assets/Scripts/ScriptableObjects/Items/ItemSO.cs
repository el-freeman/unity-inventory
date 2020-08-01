using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    Food,
    Helmet,
    Weapon,
    Shield,
    Chest,
    Boots,   
    Default
}

public enum Attributes
{
    Strength,
    Intellect,
    Agility,
    Stamina
}

public abstract class ItemSO : ScriptableObject
{
    public Sprite uiIcon;
    public bool isStackable;
    public ItemType type;
    public string description;
    public Item data = new Item();

    public Item CreateItem()
    {
        Item newItem = new Item(this);
        return newItem;
    }
   
}

[System.Serializable]
public class Item
{
    public string name;
    public int Id = -1;
    public ItemBuff[] buffs;
    public Item()
    {
        name = "";
        Id = -1;
    }

    public Item(ItemSO item)
    {
        name = item.name;
        Id = item.data.Id;
        buffs = new ItemBuff[item.data.buffs.Length];
        for (int i = 0; i < buffs.Length; i++)
        {
            buffs[i] = new ItemBuff(item.data.buffs[i].min, item.data.buffs[i].max);
            buffs[i].attribute = item.data.buffs[i].attribute;
        }
    }

}

[System.Serializable]
public class ItemBuff : IModifier
{
    public Attributes attribute;
    public int value;
    public int min;
    public int max;

    public ItemBuff(int minValue, int maxValue)
    {
        min = minValue;
        max = maxValue;
        GenerateValue();

    }

    public void AddValue(ref int baseValue)
    {
        baseValue += value;
    }

    public void GenerateValue()
    {
        value = UnityEngine.Random.Range(min, max);
    }
}
