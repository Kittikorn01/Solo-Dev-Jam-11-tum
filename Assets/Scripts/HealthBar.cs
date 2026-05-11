using UnityEngine;
using UnityEngine.UI; // ต้องมีบรรทัดนี้ถึงจะสั่งงาน UI Slider ได้

public class HealthBar : MonoBehaviour
{
    [Header("UI References")]
    public Slider healthSlider;

    [Header("Settings")]
    public float smoothSpeed = 10f; // ความเร็วในการไหลของหลอดเลือด (ปรับให้หนืดหรือไวได้)

    private PlayerStats playerStats;
    private float maxHealth; // อิงจากที่นายตั้งไว้ใน PlayerStats ว่า Max คือ 100

    void Start()
    {
        // ดึง instance ของ PlayerStats มาใช้อัตโนมัติ (จากระบบ Singleton ที่เราทำไว้)
        playerStats = PlayerStats.instance;
        maxHealth = playerStats.currentHP; // สมมติว่า Max Health เท่ากับค่า HP เริ่มต้นที่นายตั้งไว้

        if (playerStats != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = playerStats.currentHP; // เซ็ตเลือดเต็มตอนเริ่มเกม
        }
    }

    void Update()
    {
        if (playerStats == null) return;

        // ระบบ Game Feel: ค่อยๆ ให้หลอดเลือดไหลไปหาค่าเลือดปัจจุบันแบบนุ่มนวล
        healthSlider.value = Mathf.Lerp(healthSlider.value, playerStats.currentHP, Time.deltaTime * smoothSpeed);
    }
}