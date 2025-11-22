using UnityEngine;
using Unity.Netcode;

// simple parabola preview: same direction (shootPoint.forward) and same charge model.
// it stops at ground (y = 0) so it's clean and readable.
[RequireComponent(typeof(LineRenderer))]
public class TrajectoryPreview : NetworkBehaviour
{
    [Header("Refs")]
    [SerializeField] private PlayerShooting shooting; // to read ShootPoint / MaxCharge / ChargeSpeed

    [Header("Line")]
    [SerializeField] private int steps = 25;   // number of samples
    [SerializeField] private float dt  = 0.04f; // time between samples

    private LineRenderer lr;
    private float tempCharge = 0f;

    void Awake()
    {
        lr = GetComponent<LineRenderer>();
        lr.positionCount = 0;
    }

    void Update()
    {
        if (!shooting || !shooting.IsOwner) { Hide(); return; }
        if (!shooting.ShootPoint) { Hide(); return; }
        if (shooting.currentWeapon == WeaponType.Flamethrower) {Hide();return;}

        if (Input.GetKey(KeyCode.Space))
        {
            // mirror the same charge growth the shooter uses
            tempCharge += shooting.ChargeSpeed * Time.deltaTime;
            tempCharge  = Mathf.Clamp(tempCharge, 0f, shooting.MaxCharge);

            Vector3 dir = shooting.ShootPoint.forward.normalized;
            Vector3 p0  = shooting.ShootPoint.position + dir * 0.20f; // same offset as the real shot
            Vector3 v0  = dir * tempCharge;                           // matches server v0 (power as speed)

            DrawParabolaToGround(p0, v0);
        }
        else
        {
            Hide();
            tempCharge = 0f;
        }
    }

    private void DrawParabolaToGround(Vector3 p0, Vector3 v0)
    {
        Vector3 g = Physics.gravity;

        lr.positionCount = 1;
        lr.SetPosition(0, p0);
        Vector3 prev = p0;

        for (int i = 1; i < steps; i++)
        {
            float t = i * dt;
            Vector3 p = p0 + v0 * t + 0.5f * g * (t * t);

            if (p.y <= 0f) // cut at ground plane
            {
                Vector3 a = prev, b = p;
                float dy = b.y - a.y;
                float k  = Mathf.Approximately(dy, 0f) ? 0f : (0f - a.y) / dy;
                Vector3 hit = Vector3.Lerp(a, b, Mathf.Clamp01(k));

                lr.positionCount = i + 1;
                lr.SetPosition(i, hit);
                return;
            }

            lr.positionCount = i + 1;
            lr.SetPosition(i, p);
            prev = p;
        }
    }

    private void Hide()
    {
        if (lr.positionCount != 0) lr.positionCount = 0;
    }
}
