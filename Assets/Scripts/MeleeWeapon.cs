using Unity.VisualScripting;
using UnityEngine;

public class MeleeWeapon : MonoBehaviour
{
    public SpriteRenderer characterRenderer, weaponRenderer;

    [Header("Visual & Animation")]
    public Transform weaponVisual; // ลาก Object "WeaponVisual" มาใส่ช่องนี้
    public float hoverSpeed = 5f;  // ความเร็วตอนลอย
    public float hoverAmount = 0.08f; // ระยะที่ลอยขึ้นลง

    [Header("Attack Settings")]
    public float swingAngle = 70f; // องศาที่จะง้างและฟาด

    [Header("Hitbox Settings")]
    public Transform attackPoint; // จุดศูนย์กลางวงกลม (เดี๋ยวเราสร้าง Object มารับ)
    public float attackRadius = 1.5f; // ความกว้างของวงกลมสแกน
    public LayerMask enemyLayers; // เลเยอร์ของศัตรู (เอาไว้กรองไม่ให้สแกนโดนอย่างอื่น)

    private float nextAttackTime = 0f;  // ตัวเก็บเวลาว่าจะตีได้อีกทีตอนไหน
    private bool isAttacking = false;
    private Vector3 visualStartPos; // เก็บตำแหน่งตั้งต้นของอาวุธ

    public PlayerStats playerStats;

    void Start()
    {
        // จำตำแหน่งตั้งต้นของดาบไว้ (เช่น ขยับออกมาขวา 1 หน่วย)
        visualStartPos = weaponVisual.localPosition;
    }

    void Update()
    {
        AimAtMouse();

        // รับ Input คลิกซ้าย +เช็คว่าไม่ได้ตีอยู่ + เช็คว่าพ้น Cooldown หรือยัง
        if (Input.GetMouseButtonDown(0) && !isAttacking && Time.time >= nextAttackTime)
        {
            // เซ็ตเวลาตีครั้งต่อไป = เวลาปัจจุบัน + ระยะเวลา Cooldown
            nextAttackTime = Time.time + playerStats.currentCooldown;
            StartCoroutine(SwingWeapon());
        }

        // ระบบลอยดุ๊กดิ๊ก (ทำเฉพาะตอนที่ไม่ได้ฟาดดาบอยู่)
        if (!isAttacking)
        {
            float hoverY = Mathf.Sin(Time.time * hoverSpeed) * hoverAmount;
            weaponVisual.localPosition = visualStartPos + new Vector3(0f, hoverY, 0f);
        }
    }

    void AimAtMouse()
    {
        // ดึงตำแหน่งเมาส์ในโลกของเกม
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f; // ล็อกแกน Z ไว้เสมอสำหรับเกม 2D ป้องกันบั๊ก

        // หาเวกเตอร์ทิศทางแบบในรูป
        Vector2 direction = (mousePos - transform.position).normalized;

        // 1. หมุนอาวุธหันหาเมาส์ (เทคนิคพระเอกของเรา)
        transform.right = direction;

        // 2. ลอจิก Flip อาวุธด้วย Scale
        Vector3 scale = transform.localScale;
        if (direction.x < 0)
        {
            scale.y = -1;
        }
        else if (direction.x > 0)
        {
            scale.y = 1;
        }
        transform.localScale = scale;

        if (transform.eulerAngles.z > 0 && transform.eulerAngles.z < 180)
        {
            weaponRenderer.sortingOrder = characterRenderer.sortingOrder - 1; // อาวุธอยู่ข้างหลังตัวละคร
        }
        else
        {
            weaponRenderer.sortingOrder = characterRenderer.sortingOrder + 1; // อาวุธอยู่ข้างหน้าตัวละคร
        }
    }

    System.Collections.IEnumerator SwingWeapon()
    {
        isAttacking = true;

        // ล็อกตำแหน่งดาบให้อยู่กับที่
        weaponVisual.localPosition = visualStartPos;

        DetectHits();

        // กำหนดองศา: ท่าเตรียม(0) -> ท่าง้างฟาด(70) -> ฟาดลงสุด(-70)
        Quaternion idleRot = Quaternion.identity;
        Quaternion startRot = Quaternion.Euler(0f, 0f, swingAngle);
        Quaternion endRot = Quaternion.Euler(0f, 0f, -swingAngle);

        // จังหวะที่ 1: ง้างดาบปุ๊บฟาดลงทันที (The Slash)
        float elapsedTime = 0f;
        while (elapsedTime < playerStats.currentAttackDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / playerStats.currentAttackDuration;

            // ใช้สมการ Ease-out แบบพุ่งแรงตอนต้น แล้วชะลอตอนปลาย (ฟีลฟาดดาบหนักๆ)
            float smoothT = 1f - Mathf.Pow(1f - t, 3f);

            weaponVisual.localRotation = Quaternion.Slerp(startRot, endRot, smoothT);
            yield return null;
        }

        // จังหวะที่ 2: ดึงดาบกลับท่าเตรียมแบบสมูทๆ (The Recovery)
        float recoveryTime = 0.15f; // ใช้เวลาดึงดาบกลับนิดนึง จะได้ไม่กระตุก
        elapsedTime = 0f;
        while (elapsedTime < recoveryTime)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / recoveryTime;

            weaponVisual.localRotation = Quaternion.Slerp(endRot, idleRot, t);
            yield return null;
        }

        // จัดระเบียบให้กลับมาที่ 0 องศาเป๊ะๆ ก่อนจบการตี
        weaponVisual.localRotation = idleRot;
        isAttacking = false;
    }
    void DetectHits()
    {
        // สร้างวงกลมสแกนหา Collider ทั้งหมดที่อยู่ในเลเยอร์ Enemy
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRadius, enemyLayers);

        // วนลูปเช็คว่าสแกนโดนใครบ้าง
        foreach (Collider2D enemy in hitEnemies)
        {
            EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();

            if (enemyHealth != null)
            {
                // ส่งค่า Damage จาก PlayerStats ไปให้ศัตรู
                enemyHealth.TakeDamage(playerStats.currentDamage);
                Debug.Log("Hit confirmed on: " + enemy.name);

                // ลอจิกสายที่ 1: ตีโดนแล้วเราเสียเลือดเอง (Everything Has a Cost)
                if (playerStats.isPower1)
                {
                    playerStats.TakeDamage(3f);
                    Debug.Log("Power 1 Active: Player lost HP due to blood trade!");
                }
            }
        }
    }

    // ฟังก์ชันนี้จะวาดเส้นใน Unity Editor ให้เราเห็นขอบเขต Hitbox
    void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
    }
}