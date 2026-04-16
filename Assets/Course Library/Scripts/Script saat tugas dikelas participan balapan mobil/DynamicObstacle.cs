using UnityEngine;

/// <summary>
/// DynamicObstacle — Rintangan bergerak dengan beberapa pola:
///   • Patrol   : bolak-balik antara dua titik
///   • Rotate   : berputar di tempat (seperti baling-baling / pendulum)
///   • Pendulum : ayun kiri-kanan dengan SinWave
///   • Orbit    : mengelilingi titik pusat
///
/// Pasang pada GameObject rintangan dinamis.
/// </summary>
public class DynamicObstacle : MonoBehaviour
{
    public enum MovePattern { Patrol, Rotate, Pendulum, Orbit }

    [Header("Pattern")]
    public MovePattern pattern = MovePattern.Patrol;

    // ─── Patrol ─────────────────────────────────────────────────────────────
    [Header("Patrol Settings")]
    [Tooltip("Titik A dan B relatif terhadap posisi awal")]
    public Vector3 patrolOffset = new Vector3(4f, 0f, 0f);
    public float   patrolSpeed  = 3f;

    // ─── Rotate ─────────────────────────────────────────────────────────────
    [Header("Rotate Settings")]
    public Vector3 rotateAxis   = Vector3.up;
    public float   rotateSpeed  = 90f;        // derajat/detik

    // ─── Pendulum ────────────────────────────────────────────────────────────
    [Header("Pendulum Settings")]
    public float pendulumAngle  = 45f;        // sudut ayun maksimum
    public float pendulumSpeed  = 2f;

    // ─── Orbit ──────────────────────────────────────────────────────────────
    [Header("Orbit Settings")]
    public Transform orbitCenter;
    public float     orbitRadius = 4f;
    public float     orbitSpeed  = 60f;       // derajat/detik
    public float     orbitHeight = 0f;

    // ─── Collision ──────────────────────────────────────────────────────────
    [Header("Collision")]
    public string carTag = "Player";
    public float  knockbackForce = 600f;
    public ParticleSystem hitParticle;

    // ─── Internal ──────────────────────────────────────────────────────────
    private Vector3 startPos;
    private Vector3 pointA, pointB;
    private bool    movingToB = true;
    private float   orbitAngle;

    void Start()
    {
        startPos = transform.position;
        pointA   = startPos - patrolOffset * 0.5f;
        pointB   = startPos + patrolOffset * 0.5f;
    }

    void Update()
    {
        if (GameManager.Instance?.CurrentState != GameManager.GameState.Playing) return;

        switch (pattern)
        {
            case MovePattern.Patrol:    DoPatrol();    break;
            case MovePattern.Rotate:    DoRotate();    break;
            case MovePattern.Pendulum:  DoPendulum();  break;
            case MovePattern.Orbit:     DoOrbit();     break;
        }
    }

    // ─── Pola gerakan ────────────────────────────────────────────────────────
    void DoPatrol()
    {
        Vector3 target = movingToB ? pointB : pointA;
        transform.position = Vector3.MoveTowards(transform.position, target, patrolSpeed * Time.deltaTime);
        if (Vector3.Distance(transform.position, target) < 0.05f)
            movingToB = !movingToB;

        // Hadap arah gerak
        Vector3 dir = (target - transform.position);
        if (dir.sqrMagnitude > 0.001f)
            transform.rotation = Quaternion.LookRotation(dir);
    }

    void DoRotate()
    {
        transform.Rotate(rotateAxis, rotateSpeed * Time.deltaTime, Space.Self);
    }

    void DoPendulum()
    {
        float angle = Mathf.Sin(Time.time * pendulumSpeed) * pendulumAngle;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    void DoOrbit()
    {
        if (orbitCenter == null) return;
        orbitAngle += orbitSpeed * Time.deltaTime;
        float rad = orbitAngle * Mathf.Deg2Rad;
        Vector3 pos = orbitCenter.position
            + new Vector3(Mathf.Sin(rad) * orbitRadius, orbitHeight, Mathf.Cos(rad) * orbitRadius);
        transform.position = pos;
        transform.LookAt(orbitCenter.position);
    }

    // ─── Tabrakan dengan mobil ───────────────────────────────────────────────
    void OnCollisionEnter(Collision col)
    {
        if (!col.gameObject.CompareTag(carTag)) return;
        AudioManager.Instance?.PlayCrash();
        hitParticle?.Play();

        // Dorong mobil menjauh
        Rigidbody rb = col.gameObject.GetComponent<Rigidbody>();
        if (rb != null)
        {
            Vector3 dir = col.contacts[0].point - transform.position;
            dir.y = 0.3f;
            rb.AddForce(dir.normalized * knockbackForce, ForceMode.Impulse);
        }
    }
}
