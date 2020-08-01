using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class GroundItem : MonoBehaviour, ISerializationCallbackReceiver
{
    // Start is called before the first frame update
    public ItemSO item;
    public void OnAfterDeserialize()
    {
    }

    public void OnBeforeSerialize()
    {
#if UNITY_EDITOR
       // GetComponentInChildren<SpriteRenderer>().sprite = item.uiIcon;
        //EditorUtility.SetDirty(GetComponentInChildren<SpriteRenderer>());
#endif
    }
}
