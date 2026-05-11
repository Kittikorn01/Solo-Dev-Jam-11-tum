using UnityEngine;

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

        // ทอยเต๋าสุ่มตัวเลข 0 ถึง 100
        float roll = Random.Range(0f, 100f);

        // ถ้าเลขที่ทอยได้ น้อยกว่าหรือเท่ากับโอกาสดรอป (dropChance) = ดรอปของ!
        if (roll <= dropChance)
        {
            // สุ่มอีกทีว่าจะดรอปอะไร (สุ่ม 0 หรือ 1)
            int itemTypeRoll = Random.Range(0, 2);

            if (itemTypeRoll == 0 && healthItemPrefab != null)
            {
                Instantiate(healthItemPrefab, transform.position, Quaternion.identity); // เสกขวดเลือด
            }
            else if (itemTypeRoll == 1 && timeItemPrefab != null)
            {
                Instantiate(timeItemPrefab, transform.position, Quaternion.identity); // เสกขวดเวลา
            }
        }

        Destroy(gameObject);
    }
}