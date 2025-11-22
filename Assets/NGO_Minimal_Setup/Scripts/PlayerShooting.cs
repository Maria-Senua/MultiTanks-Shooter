using UnityEngine;
using Unity.Netcode;

// shoots along shootPoint.forward with charge-based power.
// local muzzle particles + local shoot audio. server spawns the projectile.

public enum WeaponType
{
    Projectile,
    Flamethrower
}

public class PlayerShooting : NetworkBehaviour
{
    [Header("Weapon Switching")]
    public WeaponType currentWeapon = WeaponType.Projectile;
    
    [Header("Refs")]
    [SerializeField] private GameObject projectilePrefab; // networked projectile
    [SerializeField] private Transform shootPoint;        // barrel tip (aims forward)
    [SerializeField] private ParticleSystem muzzleFlash;  // small muzzle particle system on the tank
    [SerializeField] private AudioSource shootAudio;      // one-shot fire sound
    
    [Header("Ammo")]
    public int ammo = 10;

    [Header("Charge")]
    [SerializeField] private float maxCharge   = 20f;     // max launch speed
    [SerializeField] private float chargeSpeed = 10f;     // fill rate per second

    private float currentCharge = 0f;
    private bool  isCharging    = false;
    
    [Header("Flamethrower Weapon")]
    [SerializeField] private ParticleSystem flameParticles;   // flame effect
    [SerializeField] private FlameDamage flameDamage; 
    private NetworkObject spawnedFlame;

    private Collider[] ownerCols;

    public NetworkVariable<float> bulletScale = new NetworkVariable<float>(1);
    public NetworkVariable<float> damageMultiplier = new NetworkVariable<float>(1);

    public override void OnNetworkSpawn()
    {
        ownerCols = GetComponentsInChildren<Collider>(false);
        if (flameParticles != null)
            flameParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

        if (flameDamage != null)
            flameDamage.enabled = false;
    }

    void Update()
    {
        
        if (!IsOwner) return;
        
        SwitchWeapon();
        SwitchShootMode();
        
        if (IsServer && spawnedFlame != null)
        {
            spawnedFlame.transform.position = shootPoint.position + shootPoint.forward * 1.5f;
            spawnedFlame.transform.rotation = shootPoint.rotation;
        }
    }
    private void SwitchWeapon()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (currentWeapon == WeaponType.Projectile)
                currentWeapon = WeaponType.Flamethrower;
            else
                currentWeapon = WeaponType.Projectile;
        }
    }

    private void SwitchShootMode()
    {
        if (currentWeapon == WeaponType.Projectile)
        {
            ShootBullet();
        }
        else
        {
            ShootFlame();
        }
    }
    private void ShootBullet()
    {
        if (ammo <= 0) return;

        if (Input.GetKey(KeyCode.Space))
        {
            isCharging = true;
            currentCharge += chargeSpeed * Time.deltaTime;
            currentCharge = Mathf.Clamp(currentCharge, 0f, maxCharge);
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            PlayMuzzleLocal();
            PlayShootSoundLocal();

            ShootServerRpc(currentCharge, shootPoint.position, shootPoint.rotation);
            ammo--;

            currentCharge = 0f;
            isCharging = false;
        }
    }
    private void ShootFlame()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (IsOwner) flameParticles?.Play(); 
            if (IsServer)
                flameDamage.enabled = true;

            SpawnFlameServerRpc();

        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            flameParticles?.Stop();
            flameDamage.enabled = false;

            StopFlamethrowerServerRpc();
        }
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

        var spawnPos = pointPos + (pointRot * Vector3.forward) * 0.20f;
        var go = Instantiate(projectilePrefab, spawnPos, pointRot);
        go.transform.localScale *= bulletScale.Value;
        go.GetComponent<Projectile>().damage *= damageMultiplier.Value;

        var no = go.GetComponent<NetworkObject>();
        no.Spawn(); 

        var rb = go.GetComponent<Rigidbody>();
        if (rb)
        {
            rb.isKinematic = false;
            rb.useGravity  = true;
            rb.linearDamping = 0f; rb.angularDamping = 0f;
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

            rb.linearVelocity = Vector3.zero;
            Vector3 v0 = (pointRot * Vector3.forward) * power;

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

  
    [ServerRpc]
    void SpawnFlameServerRpc(ServerRpcParams rpc = default)
    {
        if (!flameParticles) return;

        GameObject go = Instantiate(flameParticles.gameObject, shootPoint.position, shootPoint.rotation);
        NetworkObject no = go.GetComponent<NetworkObject>();
        no.Spawn(); // networked object. client will spawn automatically
        FlameDamage flameDamage = go.GetComponent<FlameDamage>();
       if (flameDamage != null)
        {
            flameDamage.owner = gameObject; 
        }

        FlamethrowerFollow follow = go.GetComponent<FlamethrowerFollow>();
        if (follow != null)
        {
            follow.target = shootPoint;        
            follow.offset = shootPoint.forward * 1.2f;
        }

        spawnedFlame = no;
    }
    


    [ServerRpc]
    void StopFlamethrowerServerRpc()
    {
        if (spawnedFlame != null && spawnedFlame.IsSpawned)
        {
            spawnedFlame.Despawn(true);
            spawnedFlame = null;
        }
    }




    
    // getters for the preview
    public float MaxCharge => maxCharge;
    public float ChargeSpeed => chargeSpeed;
    public Transform ShootPoint => shootPoint;
}
