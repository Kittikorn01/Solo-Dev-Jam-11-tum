using UnityEngine;
using System.Collections;

public class BoomerAI : MonoBehaviour
{
    public enum BoomerState { Creeping, Chasing, Fusing, Dead }
    public BoomerState currentState = BoomerState.Creeping;

    [Header("Movement & Trigger Distances")]
    public float creepSpeed = 1.5f;     // Phase 1: ความเร็วเดินช้าๆ
    public float chaseSpeed = 5.0f;     // Phase 2: ความเร็วตอนเครื่องติด
    public float chaseDistance = 6.0f;  // ระยะที่เริ่มพุ่งเข้าหา
    public float fuseDistance = 2.0f;   // ระยะที่เริ่มนับถอยหลังระเบิด

    [Header("Explosion Settings")]
    public float fuseTime = 1.5f;       // เวลานับถอยหลังก่อนบึ้ม
    public float explosionRadius = 3.5f;// รัศมีวงระเบิด
    public float maxDamage = 80.0f;     // ดาเมจตรงกลางไข่แดง

    [Header("Visual & Animation")]
    public SpriteRenderer visualChild;
    public float bobSpeed = 8f;         // ความเร็วดุ๊กดิ๊ก
    public float bobAmount = 0.1f;      // ความยืดหด
    private Vector3 defaultScale;       // เก็บขนาดเดิมไว้
    private float angularOffset;
    private Vector3 originalPos;

    [Header("Drop System")]
    public GameObject healthItemPrefab;
    public GameObject timeItemPrefab;
    [Range(0, 100)] public float dropChance = 30f; // โอกาสดรอป 30%

    private Transform player;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        // หาตัว Player เจอตั้งแต่เริ่ม
        player = GameObject.FindGameObjectWithTag("Player").transform;

        defaultScale = visualChild.transform.localScale;
        originalPos = visualChild.transform.localPosition;

        angularOffset = Random.Range(-60f, 60f);
    }

    void Update()
    {
        if (player == null || currentState == BoomerState.Dead || currentState == BoomerState.Fusing)
            return;

        float distance = Vector2.Distance(transform.position, player.position);

        // --- แอนิเมชันดุ๊กดิ๊ก (Sine Wave) เอาไว้ข้างนอกเลย จะได้ดุ๊กดิ๊กตลอดเวลาที่ขยับตัว ---
        float sineValue = Mathf.Sin(Time.time * bobSpeed) * bobAmount;
        visualChild.transform.localScale = new Vector3(defaultScale.x - sineValue, defaultScale.y + sineValue, defaultScale.z);

        // เช็คระยะแบบไล่จาก "ใกล้สุด" ไป "ไกลสุด"
        if (distance <= fuseDistance)
        {
            // เข้า Phase 3: ประชิดตัว เริ่มนับถอยหลังระเบิด!
            visualChild.transform.localPosition = originalPos; // คืนตำแหน่งภาพให้ตรงกลางก่อนระเบิด
            StartCoroutine(FuseSequence());
        }
        else if (distance <= chaseDistance)
        {
            currentState = BoomerState.Chasing;
            MoveTowardsPlayer(chaseSpeed);

            // เปลี่ยนท่าทาง: เร่งความเร็ว Sine Wave ให้ดุ๊กดิ๊กถี่ขึ้น (สมมติปกติ bobSpeed = 8)
            float sprintBobSpeed = bobSpeed * 2.5f;
            sineValue = Mathf.Sin(Time.time * sprintBobSpeed) * bobAmount;
            visualChild.transform.localScale = new Vector3(defaultScale.x - sineValue, defaultScale.y + sineValue, defaultScale.z);

            // คืนตำแหน่งภาพเป็นจุดศูนย์กลาง (เลิกสั่น)
            visualChild.transform.localPosition = originalPos;
        }
        else
        {
            // Phase 1: คืบคลานอยู่นอกวงเหลือง
            if (currentState != BoomerState.Creeping)
            {
                currentState = BoomerState.Creeping;
                visualChild.transform.localPosition = originalPos; // คืนตำแหน่งภาพไม่ให้สั่นค้าง
            }

            MoveTowardsPlayer(creepSpeed);
        }
    }

    void MoveTowardsPlayer(float speed)
    {
        Vector2 dir = (player.position - transform.position).normalized;

        // --- บิดเวกเตอร์ทิศทางด้วยองศาที่สุ่มไว้ (Angular Offset) ---
        float rad = angularOffset * Mathf.Deg2Rad;
        float newX = dir.x * Mathf.Cos(rad) - dir.y * Mathf.Sin(rad);
        float newY = dir.x * Mathf.Sin(rad) + dir.y * Mathf.Cos(rad);
        Vector2 offsetDir = new Vector2(newX, newY);

        rb.linearVelocity = offsetDir * speed;
    }

    // --- Phase 3: The Fuse (นับถอยหลังและกระพริบตา) ---
    IEnumerator FuseSequence()
    {
        currentState = BoomerState.Fusing;
        rb.linearVelocity = Vector2.zero; // เบรกหัวทิ่ม หยุดอยู่กับที่

        // --- เพิ่มบรรทัดนี้เพื่อให้มันกลับมาเป็นทรงกลม/รูปเดิมทันที ---
        visualChild.transform.localScale = defaultScale;
        visualChild.transform.localPosition = originalPos;

        Debug.Log("Boomer Phase 3: Fusing! Run away!");

        float elapsed = 0f;
        float blinkTimer = 0f;
        bool isRed = false;

        while (elapsed < fuseTime)
        {
            elapsed += Time.deltaTime;
            blinkTimer += Time.deltaTime;

            // ทำให้กระพริบถี่ขึ้นเรื่อยๆ เมื่อใกล้จะหมดเวลา (Mathf.Lerp ช่วยลดเวลาจังหวะกระพริบลงเรื่อยๆ)
            float currentBlinkInterval = Mathf.Lerp(0.3f, 0.05f, elapsed / fuseTime);

            if (blinkTimer >= currentBlinkInterval)
            {
                blinkTimer = 0f;
                isRed = !isRed;
                // สลับสีไปมา ขาว-แดง
                visualChild.color = isRed ? Color.red : Color.white;
            }

            yield return null; // รอเฟรมถัดไป
        }

        // คืนสีเดิมก่อนระเบิด (เผื่อไว้)
        visualChild.color = Color.white;

        // เข้า Phase 4: บึ้ม!
        Explode();
    }

    // --- Phase 4: Detonation (ระเบิดลง) ---
    void Explode()
    {
        currentState = BoomerState.Dead;
        Debug.Log("Boomer Phase 4: KABOOM!");

        // ตรวจจับทุกอย่างในรัศมีระเบิด (โดนหมดทั้งมอนและคน)
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, explosionRadius);

        foreach (Collider2D hit in hits)
        {
            // คำนวณระยะห่างเพื่อทำ Falloff Damage
            float distanceToHit = Vector2.Distance(transform.position, hit.transform.position);

            // สูตร Falloff: ตรงกลาง (0) = 1 (เต็ม 100%), ขอบรัศมี = 0 (รอดตาย)
            float falloffMultiplier = 1f - Mathf.Clamp01(distanceToHit / explosionRadius);
            float finalDamage = maxDamage * falloffMultiplier;

            // ตรวจสอบว่าโดนใคร?
            if (hit.CompareTag("Player"))
            {
                // ส่งดาเมจเข้า PlayerStats (เช็ค GetComponent ให้ดี เผื่อนายเอาไว้ที่ Game Manager)
                PlayerStats stats = hit.GetComponentInParent<PlayerStats>();
                if (stats != null)
                {
                    stats.TakeDamage(finalDamage);
                    Debug.Log("Player took explosion damage: " + finalDamage);
                }
            }
            else
            {
                // Friendly Fire: โดนเพื่อนมอนสเตอร์ด้วยกันเอง
                EnemyHealth enemyHealth = hit.GetComponent<EnemyHealth>();
                if (enemyHealth != null && hit.gameObject != this.gameObject) // ไม่ทำดาเมจใส่ตัวเองซ้ำ
                {
                    enemyHealth.TakeDamage(finalDamage);
                    Debug.Log("Friendly Fire on " + hit.name + "! Damage: " + finalDamage);
                }
            }
        }

        // เดี๋ยว Phase Polish เราค่อยมาใส่ระบบ Particle แตกกระจายตรงนี้นะ
        Die();
    }

    void Die()
    {
        Debug.Log("Enemy destroyed!");
        // ทอยเต๋าสุ่มตัวเลข 0 ถึง 100
        float roll = Random.Range(0f, 100f);

        // ถ้าเลขที่ทอยได้ น้อยกว่าหรือเท่ากับโอกาสดรอป (dropChance) = ดรอปของ!
        if (roll <= dropChance)
        {
            // สุ่มอีกทีว่าจะดรอปอะไร (สุ่ม 0 หรือ 1)
            int itemTypeRoll = Random.Range(0, 2);

            if (itemTypeRoll == 0 && healthItemPrefab != null)
            {
                Instantiate(healthItemPrefab, transform.position, Quaternion.identity); // เสกขวดเลือด
            }
            else if (itemTypeRoll == 1 && timeItemPrefab != null)
            {
                Instantiate(timeItemPrefab, transform.position, Quaternion.identity); // เสกขวดเวลา
            }
        }

        Destroy(gameObject);
    }

    // 💡 ทริคแถม: วาดเส้นรัศมีให้เห็นในหน้า Scene ของ Unity จะได้จูนค่าง่ายๆ
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseDistance); // วงเหลือง = ระยะเริ่มวิ่งตาม

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, fuseDistance);  // วงแดง = ระยะเริ่มนับถอยหลังระเบิด

        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, explosionRadius); // วงขาว = รัศมีวงระเบิด
    }
}