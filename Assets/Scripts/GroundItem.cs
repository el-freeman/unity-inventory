using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundItem : MonoBehaviour, ISerializationCallbackReceiver
{
    // Start is called before the first frame update
    public ItemSO item;
    public void OnAfterDeserialize()
    {
    }

    public void OnBeforeSerialize()
    {
        //GetComponentInChildren<SpriteRenderer>().sprite = item.uiDisplay;
        //EditorUtility.SetDirty(GetComponentInChildren<SpriteRenderer>());
    }
}
