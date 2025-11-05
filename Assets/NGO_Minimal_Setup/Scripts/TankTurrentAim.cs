using Unity.Netcode;
using UnityEngine;

// Controls turret rotation (Y) and barrel elevation (X) with smooth mouse movement
public class TankTurretAim : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private Transform turretPivot;   // rotates horizontally (Y)
    [SerializeField] private Transform barrelPivot;   // rotates vertically (X)

    [Header("Settings")]
    [SerializeField] private float horizontalSpeed = 8f;   // more responsive
    [SerializeField] private float verticalSpeed = 6f;
    [SerializeField] private float rotationSmoothness = 10f; // smooth motion
    [SerializeField] private float minPitch = -5f;   // lower angle limit
    [SerializeField] private float maxPitch = 25f;   // upper angle limit

    private float yaw;   // horizontal rotation
    private float pitch; // vertical rotation

    private NetworkVariable<float> syncedYaw = new(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    private NetworkVariable<float> syncedPitch = new(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    void Update()
    {
        if (!IsSpawned) return;

        if (IsOwner)
        {
            HandleInput();
            syncedYaw.Value = yaw;
            syncedPitch.Value = pitch;
        }
        else
        {
            // smooth sync for other clients
            turretPivot.localRotation = Quaternion.Lerp(
                turretPivot.localRotation,
                Quaternion.Euler(0, syncedYaw.Value, 0),
                Time.deltaTime * rotationSmoothness);

            barrelPivot.localRotation = Quaternion.Lerp(
                barrelPivot.localRotation,
                Quaternion.Euler(syncedPitch.Value, 0, 0),
                Time.deltaTime * rotationSmoothness);
        }
    }

    void HandleInput()
    {
        // get mouse movement
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        // update rotation values
        yaw += mouseX * horizontalSpeed;
        pitch -= mouseY * verticalSpeed;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        // apply smooth rotations locally
        turretPivot.localRotation = Quaternion.Lerp(
            turretPivot.localRotation,
            Quaternion.Euler(0, yaw, 0),
            Time.deltaTime * rotationSmoothness);

        barrelPivot.localRotation = Quaternion.Lerp(
            barrelPivot.localRotation,
            Quaternion.Euler(pitch, 0, 0),
            Time.deltaTime * rotationSmoothness);
    }
}




