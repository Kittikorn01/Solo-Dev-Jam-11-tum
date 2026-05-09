using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("General Stats")]
    public float currentHP = 100f;
    public float gameTimer = 60f; // เวลาของด่าน (วินาที)

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
            currentDamage = 15f;    // สาย 2: ดาเมจเบาลง
            currentCooldown = 0.1f; // สาย 2: รัวเมาส์ยับๆ
            currentAttackDuration = 0.1f;
        }

        // ลอจิกของสายที่ 2: ถ้าเลือกสายนี้ เวลาจะลดเร็วขึ้น 2 เท่า
        float timeMultiplier = isPower2 ? 2f : 1f;
        if (gameTimer > 0)
        {
            gameTimer -= Time.deltaTime * timeMultiplier;
        }
    }

    public void TakeDamage(float amount)
    {
        currentHP -= amount;
        if (currentHP <= 0)
        {
            Debug.Log("Game Over!");
        }
    }
}