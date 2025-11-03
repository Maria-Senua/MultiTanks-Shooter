using Unity.Netcode;
using UnityEngine;

// keeps the camera active only for whoever owns this tank
// (basically: your tank = you see through it, others = camera off)
public class CameraOnOwner : NetworkBehaviour
{
    [SerializeField] Camera cam; // drag your Camera here in the inspector

    public override void OnNetworkSpawn()
    {
        // when the object actually exists in the network
        // check if it's mine, and toggle camera accordingly
        if (cam != null)
            cam.enabled = IsOwner;
    }
}
