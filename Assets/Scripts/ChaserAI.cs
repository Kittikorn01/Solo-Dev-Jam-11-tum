using UnityEngine;

public class ChaserAI : MonoBehaviour
{
    public enum EnemyState { Chasing, Charging, Lunge, Recovery} //
    public EnemyState currentState = EnemyState.Chasing;

    [Header("Movement")]
    public float moveSpeed = 3f;
    public float lungeSpeed = 15f;
    public float stopDistance = 3f; // ระยะที่เริ่มชาร์จ

    [Header("Visual")]
    public Transform visualChild;
    private float angularOffset;
    private Transform player;
    private Rigidbody2D rb;
    private Vector2 attackDirection;

    [Header("Animation")]
    public float bobSpeed = 12f;     // ความเร็วดุ๊กดิ๊ก
    public float bobAmount = 0.15f;  // ความยืดหด
    private Vector3 defaultScale;    // เก็บสเกลตั้งต้น

    [Header("Combat")]
    public float attackDamage = 15f; // ความแรงที่พุ่งชน Player

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player").transform;

        // สุ่มองศาเบี่ยงเบน
        angularOffset = Random.Range(-60f, 60f);

        // สุ่มความเร็วให้ต่างกันนิดหน่อย (ทริคทำลายแถวรถไฟ)
        // เช่น ถ้าตั้งไว้ 3 มันจะสุ่มเป็น 2.7 - 3.3
        moveSpeed = moveSpeed * Random.Range(0.9f, 1.1f);

        defaultScale = visualChild.localScale;
    }
    void Update()
    {
        // กันเหนียว เผื่อ Player ตายหรือหาไม่เจอ
        if (player == null) return;

        if (currentState == EnemyState.Chasing)
        {
            // 1. เดินเข้าหาแบบมี Angular Offset (Cheaty AI)
            Vector2 dir = GetDirectionWithOffset();
            rb.linearVelocity = dir * moveSpeed;

            // --- เพิ่มแอนิเมชันเดิน (ดุ๊กดิ๊ก) ---
            // ใช้ Sine Wave: ถ้าแกน Y ยืดออก แกน X จะหดลง (Squash & Stretch)
            float sineValue = Mathf.Sin(Time.time * bobSpeed) * bobAmount;
            visualChild.localScale = new Vector3(defaultScale.x - sineValue, defaultScale.y + sineValue, defaultScale.z);

            // 2. เช็คระยะห่าง ถ้าเข้าใกล้ถึงจุด stopDistance ให้เริ่มโจมตี
            float distance = Vector2.Distance(transform.position, player.position);
            if (distance <= stopDistance)
            {
                StartCoroutine(AttackSequence());
            }
        }
        else if (currentState == EnemyState.Recovery)
        {
            // ให้มันหายใจหอบ = สั่นเร็วกว่าปกติ (คูณ 1.5) แต่ยืดหดเบาลงนิดนึง (คูณ 0.8)
            float pantingSpeed = bobSpeed * 1.5f;
            float pantingAmount = bobAmount * 0.8f;
            float sineValue = Mathf.Sin(Time.time * pantingSpeed) * pantingAmount;

            // รูปร่างพื้นฐานตอนพักคือ ตัวแบน (Squish) ที่เราทำไว้ใน Coroutine
            float squishX = defaultScale.x * 1.2f;
            float squishY = defaultScale.y * 0.8f;

            // เอาทรงแบนๆ มาบวกคลื่น Sine ให้มันกระเพื่อม
            visualChild.localScale = new Vector3(squishX + sineValue, squishY - sineValue, defaultScale.z);
        }
    }

    Vector2 GetDirectionWithOffset()
    {
        Vector2 dir = (player.position - transform.position).normalized;
        // บิดเวกเตอร์ทิศทางด้วยองศาที่สุ่มไว้
        float rad = angularOffset * Mathf.Deg2Rad;
        float newX = dir.x * Mathf.Cos(rad) - dir.y * Mathf.Sin(rad);
        float newY = dir.x * Mathf.Sin(rad) + dir.y * Mathf.Cos(rad);
        return new Vector2(newX, newY);
    }

    System.Collections.IEnumerator AttackSequence()
    {
        // --- State 1: CHARGING ---
        currentState = EnemyState.Charging;
        rb.linearVelocity = Vector2.zero;

        // คืนสเกลกลับเป็นปกติก่อนเริ่มสั่น
        visualChild.localScale = defaultScale;

        float chargeDuration = 0.5f;
        float elapsed = 0f;
        Vector3 originalVisualPos = visualChild.localPosition;

        while (elapsed < chargeDuration)
        {
            elapsed += Time.deltaTime;
            visualChild.localPosition = originalVisualPos + (Vector3)Random.insideUnitCircle * 0.1f;
            yield return null;
        }

        visualChild.localPosition = originalVisualPos;

        // --- State 2: LUNGE ---
        currentState = EnemyState.Lunge;

        // เอฟเฟกต์ตอนพุ่ง: บีบข้างๆ ให้ผอมลง และยืดตัวให้ยาวขึ้น!
        //visualChild.localScale = new Vector3(defaultScale.x * 0.6f, defaultScale.y * 1.4f, defaultScale.z);

        Vector2 lungeDir = (player.position - transform.position).normalized;
        rb.linearVelocity = lungeDir * lungeSpeed;

        yield return new WaitForSeconds(0.2f);

        // --- State 3: RECOVERY ---
        currentState = EnemyState.Recovery;
        rb.linearVelocity = Vector2.zero;

        visualChild.localScale = new Vector3(defaultScale.x * 1.2f, defaultScale.y * 0.8f, defaultScale.z);

        // คืนสเกลกลับเป็นปกติเพื่อยืนพักเหนื่อย
        visualChild.localScale = defaultScale;

        yield return new WaitForSeconds(1f);

        currentState = EnemyState.Chasing;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerStats playerStats = other.GetComponent<PlayerStats>();
            if (playerStats != null && !playerStats.isDead)
            {
                playerStats.TakeDamage(attackDamage);

                if (currentState == EnemyState.Lunge)
                {
                    StopAllCoroutines();

                    // เปลี่ยนไปเรียกฟังก์ชันใหม่ที่จัดการเรื่องการถอยและพัก
                    StartCoroutine(HitBounceRecovery(other.transform));
                }
            }
        }
    }

    System.Collections.IEnumerator HitBounceRecovery(Transform playerTransform)
    {
        currentState = EnemyState.Recovery;

        // 1. ถอยหลังนิดหน่อย (Micro-bounce)
        Vector2 pushDir = (transform.position - playerTransform.position).normalized;
        rb.linearVelocity = pushDir * 6f; // ความเร็วถอยเบาๆ (ไม่กระเด็นไกล)

        // 2. เปลี่ยนท่าทางเป็น "หอบเหนื่อย" (Squish)
        // ทำให้ตัวแบนลง (Y ลด) และบานออกข้าง (X เพิ่ม) 
        visualChild.localScale = new Vector3(defaultScale.x * 1.2f, defaultScale.y * 0.8f, defaultScale.z);

        // 3. ปล่อยให้มันไถลถอยหลังไปแค่ 0.15 วินาที แล้วเหยียบเบรก
        yield return new WaitForSeconds(0.15f);
        rb.linearVelocity = Vector2.zero;

        // 4. ยืนหอบเหนื่อยต่อให้ครบเวลาพัก (สมมติพัก 1 วิ ก็รออีก 0.85 วิ)
        yield return new WaitForSeconds(0.85f);

        // 5. คืนร่างเดิมและกลับไปวิ่งไล่
        visualChild.localScale = defaultScale;
        currentState = EnemyState.Chasing;
    }
}
