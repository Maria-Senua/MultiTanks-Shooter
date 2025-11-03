using Unity.Netcode;
using UnityEngine;

// turret aiming: point towards mouse-on-ground
// trick: remember last valid hit so when the ray misses,
// we keep the previous direction instead of snapping/freezing.
public class TankTurretAim : NetworkBehaviour
{
    [Header("Refs")]
    [SerializeField] Transform tankBody;     // base of the tank (position ref)
    [SerializeField] Transform turretPivot;  // rotates only this (upper part)
    [SerializeField] LayerMask groundMask;   // set to your "Ground" layer
    [SerializeField] float rotationSpeed = 8f;

    Camera cam;

    // network-sync so everyone sees the same turret direction
    NetworkVariable<Quaternion> netTurretRot = new(
        default,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Owner
    );

    // last valid aim target (prevents tiny stutters when ray misses)
    Vector3 lastAimPoint;
    bool hasAimPoint = false;

    void Start()
    {
        if (IsOwner) cam = Camera.main;
    }

    void Update()
    {
        if (!IsSpawned) return;

        if (IsOwner)
        {
            AimLocal();
            netTurretRot.Value = turretPivot.rotation;
        }
        else
        {
            // smooth on remotes
            turretPivot.rotation = Quaternion.Lerp(
                turretPivot.rotation,
                netTurretRot.Value,
                Time.deltaTime * rotationSpeed
            );
        }
    }

    void AimLocal()
    {
        if (!cam) cam = Camera.main;

        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        // try to hit ground; if not, keep the last good point -> no snap
        if (Physics.Raycast(ray, out RaycastHit hit, 1000f, groundMask))
        {
            lastAimPoint = hit.point;
            hasAimPoint = true;
        }

        if (!hasAimPoint) return; // nothing valid yet, chill

        // build a flat direction (we don't pitch the turret here)
        Vector3 dir = lastAimPoint - tankBody.position;
        dir.y = 0f;
        if (dir.sqrMagnitude < 0.0005f) return;

        Quaternion target = Quaternion.LookRotation(dir, Vector3.up);

        // smooth rotate so it feels weighty
        turretPivot.rotation = Quaternion.Lerp(
            turretPivot.rotation,
            target,
            Time.deltaTime * rotationSpeed
        );
    }
}
