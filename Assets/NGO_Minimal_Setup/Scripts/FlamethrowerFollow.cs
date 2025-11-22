using UnityEngine;
using Unity.Netcode;

public class FlamethrowerFollow : NetworkBehaviour
{
    [HideInInspector] public Transform target;        
    [HideInInspector] public Vector3 offset = Vector3.zero;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (target != null)
        {
            transform.position = target.position + offset;
            transform.rotation = target.rotation;
        }
    }
}
