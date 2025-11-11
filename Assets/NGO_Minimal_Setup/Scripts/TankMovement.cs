using Unity.Netcode;
using UnityEngine;

// basic tank movement using Rigidbody
// physics > translate, so we don't ghost through stuff
[RequireComponent(typeof(Rigidbody))]
public class TankMovement : NetworkBehaviour
{
    [SerializeField] public float moveSpeed = 7f;   // how fast we scoot
    [SerializeField] float rotateSpeed = 70f;// how fast we turn

    Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        // tanks don't need to faceplant
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }

    void FixedUpdate()
    {
        if (!IsOwner || !IsSpawned) return;

        float move = Input.GetAxis("Vertical");   // W/S
        float turn = Input.GetAxis("Horizontal"); // A/D

        // forward/backward motion (world-friendly)
        Vector3 step = transform.forward * move * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + step);

        // yaw rotation only
        Quaternion delta = Quaternion.Euler(0f, turn * rotateSpeed * Time.fixedDeltaTime, 0f);
        rb.MoveRotation(rb.rotation * delta);
    }
}
