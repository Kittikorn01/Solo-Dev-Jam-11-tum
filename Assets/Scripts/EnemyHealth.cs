using UnityEngine;
using System.Collections;
public class EnemyHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    [SerializeField] private float currentHealth;

    // [เพิ่มตรงนี้] ตัวล็อคสำคัญ ป้องกันการตายซ้ำซ้อน
    private bool isDead = false;

    [Header("Drop System")]
    public GameObject healthItemPrefab;
    public GameObject timeItemPrefab;
    [Range(0, 100)] public float dropChance = 30f; // โอกาสดรอป 30%

    [Header("Death Effect")]
    public GameObject deathEffectPrefab;

    [Header("Visual Effects")]
    public SpriteRenderer enemyVisual;

    void Start()
    {
        // เซ็ตเลือดเต็มตอนเริ่มเกม
        currentHealth = maxHealth;
    }

    // ฟังก์ชันนี้จะถูกเรียกจากดาบของผู้เล่น
    public void TakeDamage(float amount)
    {
        currentHealth -= amount;

        if (isDead) return;

        if (enemyVisual != null)
        {
            StartCoroutine(HitFlashSequence());
        }

        // Log ภาษาอังกฤษตามรีเควสครับ
        Debug.Log("Enemy took " + amount + " damage! HP left: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;
        Debug.Log("Enemy destroyed!");

        if (WaveManager.instance != null)
        {
            WaveManager.instance.EnemyDied();
        }

        if (deathEffectPrefab != null)
        {
            Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);
        }

        float roll = Random.Range(0f, 100f);

        if (roll <= dropChance)
        {
            int itemTypeRoll = Random.Range(0, 2);
            GameObject itemPrefab = null;

            if (itemTypeRoll == 0 && healthItemPrefab != null)
            {
                itemPrefab = healthItemPrefab;
            }
            else if (itemTypeRoll == 1 && timeItemPrefab != null)
            {
                itemPrefab = timeItemPrefab;
            }

            if (itemPrefab != null)
            {
                // 1. เสกไอเทมออกมาและเก็บตัวแปรไว้
                GameObject droppedItem = Instantiate(itemPrefab, transform.position, Quaternion.identity);

                // 2. สั่งให้มันเด้งออก (ต้องมี Rigidbody2D ที่ตัว Item Prefab นะ)
                Rigidbody2D itemRb = droppedItem.GetComponent<Rigidbody2D>();
                if (itemRb != null)
                {
                    // สุ่มทิศทางแบบ 360 องศา
                    Vector2 randomDir = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;

                    // สุ่มแรงดีด (เช่น ระหว่าง 3 ถึง 6)
                    float force = Random.Range(3f, 6f);

                    // ใช้ ForceMode2D.Impulse เพื่อให้แรงส่งแบบทีเดียวจบ (เหมือนโดนดีด)
                    itemRb.AddForce(randomDir * force, ForceMode2D.Impulse);
                }
            }
        }

        Destroy(gameObject);
    }

    IEnumerator HitFlashSequence()
    {
        // 1. เก็บสีเดิมเอาไว้ก่อน (เหลือง หรือ ดำ)
        Color originalColor = enemyVisual.color;

        // 2. เปลี่ยนเป็นสีขาวสว่างแวบเดียว (0.05 วิ) เพื่อความสะใจ
        enemyVisual.color = Color.white;
        yield return new WaitForSeconds(0.05f);

        // 3. คืนสีเดิม (ถ้ามันยังไม่ตายและตัวยังไม่โดนลบทิ้ง)
        if (!isDead && enemyVisual != null)
        {
            enemyVisual.color = originalColor;
        }
    }
}