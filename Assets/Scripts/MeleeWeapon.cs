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
    public float attackDuration = 0.15f; // เวลาที่ใช้ฟาด (ยิ่งน้อยยิ่งฟาดเร็ว)
    public float attackCooldown = 0.5f; // เพิ่ม Cooldown (วินาที)

    private float nextAttackTime = 0f;  // ตัวเก็บเวลาว่าจะตีได้อีกทีตอนไหน
    private bool isAttacking = false;
    private Vector3 visualStartPos; // เก็บตำแหน่งตั้งต้นของอาวุธ

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
            nextAttackTime = Time.time + attackCooldown;
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

        // ดาบกลับมาอยู่ที่ตำแหน่งตั้งต้นเป๊ะๆ ก่อนฟาด (กันมันลอยค้าง)
        weaponVisual.localPosition = visualStartPos;

        // กำหนดมุมตั้งต้น (ง้างดาบขึ้น) และมุมจบ (ฟาดดาบลง)
        // สั่งหมุนเฉพาะแกน Z
        Quaternion startRot = Quaternion.Euler(0f, 0f, swingAngle);
        Quaternion endRot = Quaternion.Euler(0f, 0f, -swingAngle);

        float elapsedTime = 0f;

        while (elapsedTime < attackDuration)
        {
            elapsedTime += Time.deltaTime;
            // คำนวณเปอร์เซ็นต์เวลา (0 ถึง 1)
            float t = elapsedTime / attackDuration;

            // ใช้คณิตศาสตร์ (Ease-out) ทำให้จังหวะฟาดดูมีน้ำหนัก ไม่แข็งเป็นหุ่นยนต์
            float smoothT = t * t * (3f - 2f * t);

            // สั่งหมุน WeaponVisual (ตัวลูก) 
            weaponVisual.localRotation = Quaternion.Slerp(startRot, endRot, smoothT);

            yield return null; // รอเฟรมถัดไป
        }

        // คืนดาบกลับสู่สภาพเดิมเมื่อฟาดเสร็จ
        weaponVisual.localRotation = Quaternion.identity;
        isAttacking = false;
    }
}