using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Food Object", menuName = "InventorySystem/Items/Food")]
public class FoodSO : ItemSO
{
    public void Awake()

    {
        type = ItemType.Food;
    }
}
