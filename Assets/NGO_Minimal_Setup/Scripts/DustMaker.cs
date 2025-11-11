using System;
using UnityEngine;

public class DustMaker : MonoBehaviour
{
    [SerializeField] private GameObject dust;

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Destructible")
        {
            var contactPoint = other.contacts[0].point;
            var newDust = Instantiate(dust, contactPoint, Quaternion.identity);
            Destroy(newDust, 10f);
        }
    }
}
