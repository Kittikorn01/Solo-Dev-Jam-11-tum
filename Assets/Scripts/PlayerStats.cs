using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats instance;

    [Header("General Stats")]
    public float currentHP = 120f;
    public float maxTime = 60f;
    public float gameTimer; // เวลาของด่าน (วินาที)
    public bool isDead = false;   // <--- [เพิ่มตรงนี้] ตัวแปรเช็คสถานะความตาย

    [Header("Power Selection")]
    public bool isPower1 = true; // สายตีแรง/เสียเลือด
    public bool isPower2 = false; // สายตีเร็ว/เวลาลดไว

    [Header("Combat Stats (Auto-Updated)")]
    public float currentDamage;
    public float currentCooldown;
    public float currentAttackDuration;

    [Header("UI & Game Over")]
    public GameObject gameOverPanel;

    [Header("Visual Effects")]
    public SpriteRenderer playerVisual; // เอาไว้ลากภาพ Player มาใส่

    [Header("Audio")]
    public AudioSource hitSound;

    void Awake()
    {
        instance = this; // [เพิ่มฟังก์ชันนี้] เซ็ตค่าตอนเริ่มเกม
    }
    void Start()
    {
        gameTimer = maxTime; // โหลดเวลาตั้งต้นตอนเริ่มเกม
    }

    void Update()
    {
        // จัดการสลับค่าพลังตามสายที่เลือกแบบ Real-time
        if (isPower1)
        {
            currentDamage = 100f;    // สาย 1: ดาเมจสูง
            currentCooldown = 0.45f; // สาย 1: ฟันช้า
            currentAttackDuration = 0.1f;
        }
        else if (isPower2)
        {
            currentDamage = 35f;    // สาย 2: ดาเมจเบาลง
            currentCooldown = 0.1f; // สาย 2: รัวเมาส์ยับๆ
            currentAttackDuration = 0.01f;
        }

        // ลอจิกของสายที่ 2: ถ้าเลือกสายนี้ เวลาจะลดเร็วขึ้น 2 เท่า
        float timeMultiplier = isPower2 ? 2f : 1f;

        if(gameTimer > maxTime) gameTimer = maxTime; // กันเวลาเกินหลอด

        if (gameTimer > 0 && !isDead && WaveManager.instance != null && WaveManager.instance.isWaveActive)
        {
            gameTimer -= Time.deltaTime * timeMultiplier;
            if (gameTimer <= 0)
            {
                gameTimer = 0;
                Die();
            }
        }
    }

    public void TakeDamage(float amount)
    {
        if (isDead) return;

        currentHP -= amount;

        if (hitSound != null)
        {
            hitSound.Play();
        }

        if (playerVisual != null)
        {
            StartCoroutine(DamageFlashSequence());
        }
        if (currentHP <= 0)
        {
            currentHP = 0;
            Die();
        }
    }

    public void Heal(float amount)
    {
        currentHP += amount;
        if (currentHP > 120f) currentHP = 120f; // กันเลือดทะลุหลอด (สมมติ Max HP คือ 100)
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

        MeleeWeapon meleeScript = GetComponentInChildren<MeleeWeapon>();
        if (meleeScript != null) meleeScript.enabled = false;

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
    }
    public void RestartGame()
    {
        // โหลด Scene ปัจจุบันซ้ำอีกรอบ (เหมือนกด F5 รีเฟรชเกม)
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    IEnumerator DamageFlashSequence()
    {
        // 1. แปลงรหัสสี #9BFF9E เป็นค่า Color ที่ Unity อ่านออก
        Color myNormalColor;
        ColorUtility.TryParseHtmlString("#9BFF9E", out myNormalColor);

        // 2. ให้กระพริบ 2 รอบ (แดง -> สีเขียวของนาย -> แดง -> สีเขียวของนาย)
        for (int i = 0; i < 2; i++)
        {
            playerVisual.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            playerVisual.color = myNormalColor; // <--- [แก้ตรงนี้] ใช้สีของนายแทนสีขาว
            yield return new WaitForSeconds(0.1f);
        }

        // กันเหนียว ตอนจบให้ชัวร์ว่าเป็นสี #9BFF9E ปกติ
        playerVisual.color = myNormalColor; // <--- [แก้ตรงนี้]
    }
}