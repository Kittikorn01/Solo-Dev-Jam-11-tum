using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("General Stats")]
    public float currentHP = 100f;
    public float gameTimer = 60f; // เวลาของด่าน (วินาที)
    public bool isDead = false;   // <--- [เพิ่มตรงนี้] ตัวแปรเช็คสถานะความตาย

    [Header("Power Selection")]
    public bool isPower1 = true; // สายตีแรง/เสียเลือด
    public bool isPower2 = false; // สายตีเร็ว/เวลาลดไว

    [Header("Combat Stats (Auto-Updated)")]
    public float currentDamage;
    public float currentCooldown;
    public float currentAttackDuration;

    void Update()
    {
        // จัดการสลับค่าพลังตามสายที่เลือกแบบ Real-time
        if (isPower1)
        {
            currentDamage = 50f;    // สาย 1: ดาเมจสูง
            currentCooldown = 0.6f; // สาย 1: ฟันช้า
            currentAttackDuration = 0.4f;
        }
        else if (isPower2)
        {
            currentDamage = 40f;    // สาย 2: ดาเมจเบาลง
            currentCooldown = 0.01f; // สาย 2: รัวเมาส์ยับๆ
            currentAttackDuration = 0.02f;
        }

        // ลอจิกของสายที่ 2: ถ้าเลือกสายนี้ เวลาจะลดเร็วขึ้น 2 เท่า
        float timeMultiplier = isPower2 ? 2f : 1f;

        if (gameTimer > 0 && !isDead)
        {
            gameTimer -= Time.deltaTime * timeMultiplier;
            if (gameTimer <= 0)
            {
                gameTimer = 0; // กันตัวเลขติดลบ
                Die(); // เวลาหมด = ตาย
            }
        }
    }

    public void TakeDamage(float amount)
    {
        if (isDead) return;

        currentHP -= amount;
        if (currentHP <= 0)
        {
            currentHP = 0;
            Die();
        }
    }

    public void Heal(float amount)
    {
        currentHP += amount;
        if (currentHP > 100f) currentHP = 100f; // กันเลือดทะลุหลอด (สมมติ Max HP คือ 100)
        Debug.Log("Healed! Current HP: " + currentHP);
    }

    // ฟังก์ชันสำหรับเพิ่มเวลา
    public void AddTime(float amount)
    {
        gameTimer += amount;
        Debug.Log("Time Added! Current Timer: " + gameTimer);
    }

    // --- [เพิ่มฟังก์ชันนี้ด้านล่างสุด] ---
    public void Die()
    {
        if (isDead) return; // กันตายซ้ำสอง

        isDead = true;
        Debug.Log("Player is Dead! Game Over.");

        // 1. ปิด Collider จะได้ไม่มีมอนสเตอร์เดินชนศพหรือตีซ้ำ
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        // 2. หยุดการเคลื่อนที่ทางฟิสิกส์
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null) rb.linearVelocity = Vector2.zero;

        // 3. ปิดสคริปต์บังคับตัวละคร (ถ้านายใช้ชื่ออื่น ให้แก้ชื่อ PlayerMovement ให้ตรงกัน)
        PlayerMovement movementScript = GetComponent<PlayerMovement>();
        if (movementScript != null) movementScript.enabled = false;

        // หมายเหตุ: เดี๋ยวเราค่อยให้ WaveManager มาเช็คค่า isDead จากสคริปต์นี้เพื่อโชว์หน้า UI แพ้
    }
}