using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    private Vector2 movementInput;
    private Rigidbody2D rb;

    [Header("Animation Settings")]
    public Transform visualTransform; // ลาก GameObject "Visual" มาใส่ตรงนี้ใน Inspector
    public float bounceSpeed = 15f;   // ความเร็วในการเด้ง (ความถี่คลื่น)
    public float bounceAmount = 0.2f; // ความสูงในการเด้ง (แอมพลิจูด)

    [Header("Dash Settings")]
    public float dashSpeed = 15f;      // ความเร็วตอนพุ่ง
    public float dashDuration = 0.2f;  // พุ่งนานแค่ไหน
    public float dashCooldown = 1f;    // รอคูลดาวน์กี่วินาที

    private float dashCounter;         // ตัวนับเวลาพุ่ง
    private float dashCoolCounter;     // ตัวนับเวลาคูลดาวน์
    private Vector2 lastMoveDirection; // เก็บค่าทิศทางล่าสุด

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // 1. รับค่า Input แบบ Snappy (กดปุ๊บเดินปั๊บ ปล่อยปุ๊บหยุดปั๊บ)
        movementInput.x = Input.GetAxisRaw("Horizontal");
        movementInput.y = Input.GetAxisRaw("Vertical");

        // แก้ปัญหาเดินเฉียงไวเกินไป
        movementInput = movementInput.normalized;

        // เก็บค่าทิศทางล่าสุดไว้ใช้ตอน Dash (เช็คว่ามีการกดปุ่มทิศทาง)
        if (movementInput.x != 0 || movementInput.y != 0)
        {
            lastMoveDirection = movementInput;
        }

        // ลอจิกจับเวลา Cooldown ของ Dash
        if (dashCoolCounter > 0)
        {
            dashCoolCounter -= Time.deltaTime;
        }

        // กดปุ่ม Spacebar เพื่อ Dash (เช็คว่าไม่ได้กำลัง Dash อยู่ และพ้น Cooldown แล้ว)
        if (Input.GetKeyDown(KeyCode.Space) && dashCoolCounter <= 0 && dashCounter <= 0)
        {
            dashCounter = dashDuration;
        }

        // 2. เรียกใช้ระบบ Animation
        HandleProceduralAnimation();
    }

    void FixedUpdate()
    {
        // เช็คว่าอยู่ในสถานะ Dash หรือเปล่า
        if (dashCounter > 0)
        {
            // ลบเวลา Dash ลงเรื่อยๆ
            dashCounter -= Time.fixedDeltaTime;

            // ขยับตัวละครด้วยความเร็ว Dash ในทิศทางล่าสุด
            rb.linearVelocity = lastMoveDirection * dashSpeed;

            // เมื่อ Dash เสร็จแล้ว ให้เริ่มนับ Cooldown ทันที
            if (dashCounter <= 0)
            {
                dashCoolCounter = dashCooldown;
            }
        }
        else
        {
            // ถ้าไม่ได้ Dash ก็เดินปกติ
            rb.linearVelocity = movementInput * moveSpeed;
        }
    }

    void HandleProceduralAnimation()
    {
        // เช็คว่าผู้เล่นกำลังเดินอยู่ไหม (magnitude > 0 คือมีการขยับ)
        if (movementInput.magnitude > 0.1f)
        {
            // สูตร Sine Wave: sin(เวลา * ความเร็ว) * ความแรง
            float bounce = Mathf.Sin(Time.time * bounceSpeed) * bounceAmount;

            // สั่งให้ยืดหดแค่แกน Y (หรือจะบวกแกน X ให้มันหดสวนทางกันเพื่อความสมจริง (Squash & Stretch) ก็ได้)
            visualTransform.localScale = new Vector3(1f, 1f + bounce, 1f);
        }
        else
        {
            // ถ้าหยุดเดิน ให้ค่อยๆ คืนร่างกลับเป็นสเกล 1 เท่าเดิมแบบนุ่มนวล
            visualTransform.localScale = Vector3.Lerp(visualTransform.localScale, Vector3.one, Time.deltaTime * 10f);
        }
    }
}