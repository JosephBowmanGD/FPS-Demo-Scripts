using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Refrences")]
    private Rigidbody rb;
    private Camera cam;

    [Header("Explosive Projectile")]
    public bool isExplosive;
    public bool isFlash;
    public float explosionRadius;
    public float explosionForce;
    public int explosionDamage;
    public GameObject explosionEffect;


    private bool hitTarget;

    [SerializeField] private int grenadeTime, flashTime;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        cam = Camera.main;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (hitTarget)
            return;
        else
            hitTarget = true;
            
        // explode projectile if it's explosive
        if (isExplosive)
        {
            Invoke(nameof(Explode), grenadeTime);
            return;
        }

        if (isFlash){
            Invoke(nameof(Flash), flashTime);
            return;
        }
    }

    private void Explode()
    {
        // spawn explosion effect (if assigned)
        if (explosionEffect != null)
            Instantiate(explosionEffect, transform.position, Quaternion.identity);

        // find all the objects that are inside the explosion range
        Collider[] objectsInRange = Physics.OverlapSphere(transform.position, explosionRadius);

        // loop through all of the found objects and apply damage and explosion force
        for (int i = 0; i < objectsInRange.Length; i++)
        {
            if (objectsInRange[i].gameObject == gameObject)
            {
                // don't break or return please, thanks
            }
            else
            {
                // check if object has a rigidbody
                if (objectsInRange[i].GetComponent<Rigidbody>() != null)
                {
                    // custom explosionForce
                    Vector3 objectPos = objectsInRange[i].transform.position;

                    // calculate force direction
                    Vector3 forceDirection = (objectPos - transform.position).normalized;

                    // apply force to object in range
                    objectsInRange[i].GetComponent<Rigidbody>().AddForceAtPosition(forceDirection * explosionForce + Vector3.up * explosionForce, transform.position + new Vector3(0,-0.5f,0), ForceMode.Impulse);
                }
            }
        }

        // destroy projectile with a delay
        Invoke(nameof(DestroyProjectile), .1f);
    }

    private void Flash(){
        if (checkVisibility())
            BlindEffect.activeInstance.GoBlind();
        //else

        Invoke(nameof(DestroyProjectile), .1f);
    }

    private bool checkVisibility()
    {
        var planes = GeometryUtility.CalculateFrustumPlanes(cam);
        var point = transform.position;

        foreach (var p in planes)
        {
            if(p.GetDistanceToPoint(point) > 0)
            {
                Ray ray = new Ray(cam.transform.position, transform.position - cam.transform.position);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                    return hit.transform.gameObject == this.gameObject;
                else return false;
            }
            else return false;
        }

        return false;
    }

    private void DestroyProjectile()
    {
        Destroy(gameObject);
    }
}