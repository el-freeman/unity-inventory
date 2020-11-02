using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{   
    NavMeshAgent navMeshAgent;
    public InventorySO inventory;
    public InventorySO equipment;
    public Canvas inventoryUI;
    public TextMeshProUGUI atkText;
    public TextMeshProUGUI defText;
    public TextMeshProUGUI hitText;   
    public Button groundItemName;
    public Animator anim; 
    public float attackRange = 15f;

    bool isMoveAndShoot = false;

    public Shooting shooting;

    public Attribute[] attributes;
    public void OnTriggerEnter(Collider other)
    {
        var groundItem = other.GetComponent<GroundItem>();
        if(groundItem)
        {
            Item item = new Item(groundItem.item);
            if(inventory.AddItem(item, 1))
            {
                Destroy(other.gameObject);
            }           
        }
    }
    public void AttributeModified(Attribute attribute)
    {
        Debug.Log(string.Concat(attribute.type, " was updated ! Value is now ",attribute.value.ModifiedValue ));
        switch(attribute.type)
        {
            case Attributes.Strength:
                atkText.text = attribute.value.ModifiedValue.ToString("n0");
                break;
            case Attributes.Stamina:
                defText.text = attribute.value.ModifiedValue.ToString("n0");
                break;
            default:
                break;
        }

    }
    private void OnApplicationQuit()
    {
        inventory.Clear();
        equipment.Clear();
    }
    public void OnBeforeSlotUpdate(InventorySlot slot)
    {
        if (slot.ItemObject == null)
            return;
        switch (slot.parent.inventory.type)
        {
            case InterfaceType.Inventory:
                break;
            case InterfaceType.Equipment:
                print(string.Concat("Removed ", slot.ItemObject, " on ", slot.parent.inventory.type, ", Allowed Items: ", string.Join(", ", slot.AllowedItems)));

                for (int i = 0; i < slot.item.buffs.Length; i++)
                {
                    for (int j = 0; j < attributes.Length; j++)
                    {
                        if (attributes[j].type == slot.item.buffs[i].attribute)
                            attributes[j].value.RemoveModifier(slot.item.buffs[i]);
                    }
                }
                break;
            case InterfaceType.Chest:
                break;
            default:
                break;
        }
    }
    public void OnAfterSlotUpdate(InventorySlot slot)
    {
        if (slot.ItemObject == null)
            return;
        switch (slot.parent.inventory.type)
        {
            case InterfaceType.Inventory:
                break;
            case InterfaceType.Equipment:
                print(string.Concat("Placed ", slot.ItemObject, " on ", slot.parent.inventory.type, ", Allowed Items: ", string.Join(", ", slot.AllowedItems)));

                for (int i = 0; i < slot.item.buffs.Length; i++)
                {
                    for (int j = 0; j < attributes.Length; j++)
                    {
                        if (attributes[j].type == slot.item.buffs[i].attribute)
                            attributes[j].value.AddModifier(slot.item.buffs[i]);
                    }
                }

                break;
            case InterfaceType.Chest:
                break;
            default:
                break;
        }
    }

    private void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        for(int i = 0; i < attributes.Length; i++)
        {
            attributes[i].SetParent(this);
        }
        for(int i = 0; i <equipment.GetSlots.Length; i++)
        {
            equipment.GetSlots[i].OnBeforeUpdate += OnBeforeSlotUpdate;
            equipment.GetSlots[i].OnAfterUpdate += OnAfterSlotUpdate;
        }
    }
  
    // Update is called once per frame
    void FixedUpdate()
    {
        if (Input.GetMouseButton(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            
            if (MouseData.tempItemBeingDragged == null && !EventSystem.current.IsPointerOverGameObject())
            {               
                if (Physics.Raycast(ray, out hit, 100))
                {
                    var getEnemy = hit.collider.CompareTag("Enemy");
                    var distance = Vector3.Distance(transform.position, hit.point);
                    
                    
                    if (getEnemy)
                    {           
                        if(distance <= attackRange)
                        {
                            navMeshAgent.SetDestination(this.transform.position);
                            LookatSlerp(hit);
                            anim.SetBool("isShooting", true);                        
                            //Debug.Log("attack");           
                            shooting.Shoot();                                                      

                        }
                        else
                        {
                            navMeshAgent.SetDestination(Vector3.MoveTowards(this.transform.position, hit.point, distance-attackRange));
                            LookatSlerp(hit);
                            isMoveAndShoot = true;                          
                        }
                    }
                    else
                    {
                        // Debug.Log("is running TRUE");
                        anim.SetBool("isRunning", true);
                        navMeshAgent.SetDestination(hit.point);
                    }                    
                }
            }
        }
        else if (navMeshAgent.hasPath)
        {
            anim.SetBool("isShooting", false);
            anim.SetBool("isRunning", true);
        }
        else if(isMoveAndShoot)
        {
            anim.SetBool("isShooting", true);
            anim.SetBool("isRunning", false);
            shooting.Shoot();
            isMoveAndShoot = false;
        }
        else
        {
            anim.SetBool("isShooting", false);
            anim.SetBool("isRunning", false);
        }
    }

    private void LookatSlerp(RaycastHit hit)
    {
        Vector3 dir = hit.collider.transform.position - transform.position;
        Quaternion toRotation = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, 35 * Time.deltaTime);
    }

    private void ShowGroundItemNameOnMouseOver()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100))
        {
            var groundItem = hit.collider.gameObject;
            var groundItemComponent = groundItem.GetComponent<GroundItem>();
            Vector3 offset = new Vector3(0, 20, 0);

            if (groundItemComponent && !EventSystem.current.IsPointerOverGameObject())
            {
                //Debug.Log("item is pointed now" + groundItem.name);               
                groundItemName.gameObject.SetActive(true);
                var screenPos = Camera.main.WorldToScreenPoint(groundItem.transform.position);
                groundItemName.transform.position = screenPos + offset;
                var text = groundItemName.gameObject.GetComponentInChildren<TextMeshProUGUI>();
                text.text = groundItem.name;
            }
            if (groundItemComponent == null)
            {
                groundItemName.gameObject.SetActive(false);              
            }
        }
    }

    private void Update()
    {
        ShowGroundItemNameOnMouseOver();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            inventory.Save();
            equipment.Save();
            Debug.Log("save complete");
        }
        if (Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            inventory.Load();
            equipment.Load();
            Debug.Log("load complete");
        }
        if(Input.GetKeyDown(KeyCode.I))
        {
            ToggleUI();
        }
    }
    
    public void ToggleUI()
    {
        if(inventoryUI.gameObject.activeSelf)
        {
            inventoryUI.gameObject.SetActive(false);
        }
        else
        {
            inventoryUI.gameObject.SetActive(true);
        }
        
    }
}

[System.Serializable]
public class Attribute
{
    [System.NonSerialized]
    public PlayerController parent;
    public Attributes type;
    public ModifiableInt value;

    public void SetParent(PlayerController player)
    {
        parent = player;
        value = new ModifiableInt(AttributeModified);
    }    
    public void AttributeModified()
    {
        parent.AttributeModified(this);
    }

}