using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    NavMeshAgent navMeshAgent;
    public InventorySO inventory;

    public void OnTriggerEnter(Collider other)
    {
        var groundItem = other.GetComponent<GroundItem>();
        if(groundItem)
        {
           // Item item = new Item(groundItem.item);
            //Debug.Log(item.Id);
            inventory.AddItem(new Item(groundItem.item), 1);
            Destroy(other.gameObject);
        }
    }
    private void OnApplicationQuit()
    {
       inventory.Container.Items = new InventorySlot[28];
    }

    //public void OnTriggerEnter(Collider other)
    //{
    //    var groundItem = other.GetComponent<GroundItem>();
    //    if(groundItem)
    //    {
    //        Item item = new Item(groundItem.item);
    //        Debug.Log(item.Id);
    //        inventory.AddItem(item, 1);
    //        Destroy(other.gameObject);
    //    }
    //}
    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if(Input.GetMouseButton(0))
        {
            if(Physics.Raycast(ray, out hit, 100))
            {
                navMeshAgent.SetDestination(hit.point);
            }
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            inventory.Save();
            Debug.Log("save complete");
        }
        if (Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            inventory.Load();
            Debug.Log("load complete");
        }
    }
}
