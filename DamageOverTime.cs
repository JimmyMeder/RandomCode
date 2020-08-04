using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageOverTime : MonoBehaviour
{
    GameObject parent;
    public int damage;
    public float tickInterval;
    public float duration;
    // Start is called before the first frame update
    void Start()
    {
        parent = gameObject.transform.parent.gameObject;
        if (parent.GetComponent<CharacterStatus>() != null)
        {
            InvokeRepeating("DoT", tickInterval, tickInterval);
            StartCoroutine(DamageOverTimeDuration());
        }
    }

    void DoT()
    {
        parent.GetComponent<CharacterStatus>().DamageChar(damage);
    }

    IEnumerator DamageOverTimeDuration()
    {
        yield return new WaitForSeconds(duration);
        CancelInvoke("DoT");
        Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
