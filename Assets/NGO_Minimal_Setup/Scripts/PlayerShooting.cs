using UnityEngine;
using Unity.Netcode;

public class PlayerShooting : NetworkBehaviour
{
    [SerializeField] GameObject projectilePrefab;
    [SerializeField] Transform shootPoint;
    [SerializeField] ParticleSystem muzzleFlash;
    [SerializeField] AudioSource shootAudio;
    [SerializeField] float maxCharge = 20f;
    [SerializeField] float chargeSpeed = 10f;

    float currentCharge = 0f;
    bool isCharging = false;

    public NetworkVariable<float> bulletScale = new NetworkVariable<float>(1f);

    void Update()
    {
        if (!IsOwner) return;

        // Charging
        if (Input.GetKey(KeyCode.Space))
        {
            isCharging = true;
            currentCharge += chargeSpeed * Time.deltaTime;
            currentCharge = Mathf.Clamp(currentCharge, 0, maxCharge);
            PowerBarUI.Instance?.SetCharge(currentCharge / maxCharge);
        }

        // Release and shoot
        if (Input.GetKeyUp(KeyCode.Space))
        {
            ShootServerRpc(currentCharge);
            currentCharge = 0f;
            isCharging = false;
            PowerBarUI.Instance?.SetCharge(0);
        }
    }

    [ServerRpc]
    void ShootServerRpc(float power)
    {
        var projectileObj = Instantiate(projectilePrefab, shootPoint.position, shootPoint.rotation);
        projectileObj.transform.localScale *= bulletScale.Value;
        projectileObj.GetComponent<NetworkObject>().Spawn(true);
        projectileObj.GetComponent<Rigidbody>().linearVelocity = shootPoint.forward * power;

        ShootClientRpc(); // trigger visuals for everyone
    }

    [ClientRpc]
    void ShootClientRpc()
    {
        // Recoil
        if (shootPoint && shootPoint.parent != null)
            shootPoint.parent.localPosition -= shootPoint.forward * 0.1f;

        // Muzzle flash
        if (muzzleFlash)
        {
            muzzleFlash.Play();
        }

        // Sound
        if (shootAudio)
        {
            shootAudio.pitch = Random.Range(0.9f, 1.1f);
            shootAudio.Play();
        }

        // Return barrel smoothly to position
        if (shootPoint && shootPoint.parent != null)
            shootPoint.parent.localPosition = Vector3.Lerp(shootPoint.parent.localPosition, Vector3.zero, 0.2f);
    }
}
