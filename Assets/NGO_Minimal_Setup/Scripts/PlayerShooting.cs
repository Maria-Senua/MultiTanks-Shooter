using UnityEngine;
using Unity.Netcode;

// shoots along shootPoint.forward with charge-based power.
// local muzzle particles + local shoot audio. server spawns the projectile.
public class PlayerShooting : NetworkBehaviour
{
    [Header("Refs")]
    [SerializeField] private GameObject projectilePrefab; // networked projectile
    [SerializeField] private Transform shootPoint;        // barrel tip (aims forward)
    [SerializeField] private ParticleSystem muzzleFlash;  // small muzzle particle system on the tank
    [SerializeField] private AudioSource shootAudio;      // one-shot fire sound

    [Header("Charge")]
    [SerializeField] private float maxCharge   = 20f;     // max launch speed
    [SerializeField] private float chargeSpeed = 10f;     // fill rate per second

    private float currentCharge = 0f;
    private bool  isCharging    = false;

    private Collider[] ownerCols;

    public NetworkVariable<float> bulletScale = new NetworkVariable<float>(1);
    public NetworkVariable<float> damageMultiplier = new NetworkVariable<float>(1);

    public override void OnNetworkSpawn()
    {
        ownerCols = GetComponentsInChildren<Collider>(false);
    }

    void Update()
    {
        if (!IsOwner) return;

        // hold to charge
        if (Input.GetKey(KeyCode.Space))
        {
            isCharging = true;
            currentCharge += chargeSpeed * Time.deltaTime;
            currentCharge  = Mathf.Clamp(currentCharge, 0f, maxCharge);
            PowerBarUI.Instance?.SetCharge(currentCharge / maxCharge);
        }

        // release to shoot
        if (Input.GetKeyUp(KeyCode.Space))
        {
            PlayMuzzleLocal();
            PlayShootSoundLocal();

            // ask server to spawn the projectile
            ShootServerRpc(currentCharge, shootPoint.position, shootPoint.rotation);

            // reset charge / UI
            currentCharge = 0f;
            isCharging = false;
            PowerBarUI.Instance?.SetCharge(0f);

            // tiny local recoil
            if (shootPoint && shootPoint.parent != null)
                shootPoint.parent.localPosition -= shootPoint.forward * 0.1f;
        }

        // bring barrel back smoothly (local cosmetic)
        if (shootPoint && shootPoint.parent != null)
            shootPoint.parent.localPosition = Vector3.Lerp(shootPoint.parent.localPosition, Vector3.zero, 0.2f);
    }

    private void PlayMuzzleLocal()
    {
        if (!muzzleFlash) return;
        var t = muzzleFlash.transform;
        t.SetPositionAndRotation(shootPoint.position, shootPoint.rotation);
        muzzleFlash.Clear(true);
        muzzleFlash.Play(true);
    }

    private void PlayShootSoundLocal()
    {
        if (!shootAudio) return;
        shootAudio.pitch = Random.Range(0.95f, 1.05f);
        shootAudio.Play();
    }

    [ServerRpc]
    private void ShootServerRpc(float power, Vector3 pointPos, Quaternion pointRot, ServerRpcParams _ = default)
    {
        if (!projectilePrefab) return;

        // small spawn offset so the projectile doesn't clip the barrel
        var spawnPos = pointPos + (pointRot * Vector3.forward) * 0.20f;
        var go = Instantiate(projectilePrefab, spawnPos, pointRot);
        go.transform.localScale *= bulletScale.Value;
        go.GetComponent<Projectile>().damage *= damageMultiplier.Value;

        var no = go.GetComponent<NetworkObject>();
        if (!no) { Debug.LogError("[PlayerShooting] projectilePrefab needs NetworkObject"); Destroy(go); return; }
        no.Spawn(); // replicated to all clients

        // give it the initial velocity (mass=1 → speed == power)
        var rb = go.GetComponent<Rigidbody>();
        if (rb)
        {
            rb.isKinematic = false;
            rb.useGravity  = true;
            rb.linearDamping = 0f; rb.angularDamping = 0f;
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

            rb.linearVelocity = Vector3.zero;
            Vector3 v0 = (pointRot * Vector3.forward) * power;

            // some Unity 6 variants expose linearVelocity; set it if available
            var prop = typeof(Rigidbody).GetProperty("linearVelocity");
            if (prop != null && prop.CanWrite) { try { prop.SetValue(rb, v0, null); } catch { } }

            rb.linearVelocity = v0;
        }

        IgnoreSelf(go);
    }

    private void IgnoreSelf(GameObject projectile)
    {
        if (ownerCols == null || ownerCols.Length == 0)
            ownerCols = GetComponentsInChildren<Collider>(false);

        var projCols = projectile.GetComponentsInChildren<Collider>(false);
        foreach (var oc in ownerCols)
            foreach (var pc in projCols)
                if (oc && pc) Physics.IgnoreCollision(pc, oc, true);
    }

    // getters for the preview
    public float MaxCharge => maxCharge;
    public float ChargeSpeed => chargeSpeed;
    public Transform ShootPoint => shootPoint;
}
