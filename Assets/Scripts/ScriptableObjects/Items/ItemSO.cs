using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    Food,
    Equipment,
    Default
}

public enum Attribute
{
    Strength,
    Intellect,
    Agility,
    Stamina
}

public abstract class ItemSO : ScriptableObject
{
    public int Id;
    public Sprite uiIcon;  
    public ItemType type;
    public string description;
    public ItemBuff[] buffs;

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
    public int Id;
    public ItemBuff[] buffs;

    public Item(ItemSO item)
    {
        name = item.name;
        Id = item.Id;
        buffs = new ItemBuff[item.buffs.Length];
        for (int i = 0; i < buffs.Length; i++)
        {
            buffs[i] = new ItemBuff(item.buffs[i].min, item.buffs[i].max);
            buffs[i].attribute = item.buffs[i].attribute;
        }
    }

}


//[System.Serializable]
//public class Item
//{
//    public string name;
//    public int Id;
//    public ItemBuff[] buffs;

//    public Item(ItemSO item)
//    {
//        name = item.name;
//        Id = item.Id;
//        buffs = new ItemBuff[item.buffs.Length];
//        for(int i = 0; i <buffs.Length; i++)
//        {
//            buffs[i] = new ItemBuff(item.buffs[i].min, item.buffs[i].max)
//            {
//                attribute = item.buffs[i].attribute
//            };
//        }
//    }

//}

[System.Serializable]
public class ItemBuff
{
    public Attribute attribute;
    public int value;
    public int min;
    public int max;

    public ItemBuff(int minValue, int maxValue)
    {
        min = minValue;
        max = maxValue;
        GenerateValue();

    }
    public void GenerateValue()
    {
        value = UnityEngine.Random.Range(min, max);
    }
}
