using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * This is old and I have made a better version
 */
public class Grenade : MonoBehaviour
{
    public float explosionDelay;
    public float explosionRadius;
    public float explosionForce;
    public float explosionLiftModifier;
    public int damage;
    bool explosionHappened;
    // Start is called before the first frame update
    void Start()
    {
        gameObject.GetComponent<Collider>().enabled = false;
        TriggerGrenadeCountdown();      //For testing purposes the countdown is triggered immediately. later something else will trigger it
    }

    public void TriggerGrenadeCountdown()
    {
        Debug.Log("TRIGGERED");
        explosionHappened = false;
        Invoke("Explode", explosionDelay);
    }

    void Explode()
    {
        gameObject.GetComponent<Collider>().enabled = true;
        SetGlobalScale(this.transform, new Vector3(explosionRadius, explosionRadius, explosionRadius));
    }

    void SetGlobalScale(Transform transform, Vector3 globalScale)
    {
        transform.localScale = Vector3.one;
        transform.localScale = new Vector3(globalScale.x / transform.lossyScale.x, globalScale.y / transform.lossyScale.y, globalScale.z / transform.lossyScale.z);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Rigidbody>() != null)
        {
            Debug.Log("BOOM");
            other.GetComponent<Rigidbody>().AddExplosionForce(explosionForce, transform.position, explosionRadius, explosionLiftModifier, ForceMode.Impulse);
            explosionHappened = true;
        }
        if (other.GetComponent<CharacterStatus>() != null)
        {
            float distance = Vector3.Distance(other.transform.position, transform.position);
            other.GetComponent<CharacterStatus>().DamageChar((int)(damage / (distance * distance)));        //damage / (distance * distance) is for really basic falloff curve
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (explosionHappened)
        {
            Debug.Log("DELETE");
            Destroy(transform.parent.gameObject);
        }
    }
}
