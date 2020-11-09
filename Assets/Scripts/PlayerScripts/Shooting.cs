using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooting : MonoBehaviour
{
    public Transform gunPoint;
    //public ParticleSystem fireEffect; 
    public GameObject fireEffect;
    private LineRenderer bulletLine;
    private float attackRange = 50f;
    public Vector3 effctOffset = new Vector3(0, 0, 0);
    int shootableMask;
   
    private void Awake()
    {
        shootableMask = LayerMask.GetMask("Shootable");
        bulletLine = GetComponent<LineRenderer>();      
    }
    private void Start()
    {
        //fireEffect.SetActive(false);
        bulletLine.positionCount = 2;
        bulletLine.enabled = false;      

    }
    public void Shoot()
    {        
        RaycastHit hit;
        Vector3 hitPosition = Vector3.zero;        

        if (Physics.Raycast(gunPoint.position, gunPoint.forward, out hit, attackRange, shootableMask))
        {
            StartCoroutine(ShotEffect(hit.point));
        }       
    }

    private IEnumerator ShotEffect(Vector3 hitPosition)
    {
        
        bulletLine.SetPosition(0, gunPoint.position);
        bulletLine.SetPosition(1, hitPosition);

        yield return new WaitForSeconds(0.04f);

        fireEffect.transform.position = gunPoint.position + effctOffset;
        fireEffect.SetActive(true);   

        bulletLine.enabled = true;

        yield return new WaitForSeconds(0.08f);
        bulletLine.enabled = false;
        fireEffect.SetActive(false);

    }
    
    
}
