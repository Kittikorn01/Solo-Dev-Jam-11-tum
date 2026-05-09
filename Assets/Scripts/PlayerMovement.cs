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

        // 2. เรียกใช้ระบบ Animation
        HandleProceduralAnimation();
    }

    void FixedUpdate()
    {
        // 3. ขยับตัวละคร (ฟิสิกส์ต้องอยู่ใน FixedUpdate)
        rb.linearVelocity = movementInput * moveSpeed;
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