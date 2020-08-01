using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New ITem Database", menuName = "InventorySystem/Items/Database")]
public class ItemDatabaseSO : ScriptableObject, ISerializationCallbackReceiver
{
    public ItemSO[] ItemObjects;      

    public void UpdateID()
    {
        for (int i = 0; i < ItemObjects.Length; i++)
        {
            if(ItemObjects[i].data.Id != i)
            {
                ItemObjects[i].data.Id = i;
            }
        }
    }
    public void OnAfterDeserialize()
    {
        UpdateID();
    }

    public void OnBeforeSerialize()
    {       
    }
}
