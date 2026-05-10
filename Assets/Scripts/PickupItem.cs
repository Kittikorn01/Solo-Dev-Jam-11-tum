using UnityEngine;

public class PickupItem : MonoBehaviour
{
    public enum ItemType { Health, Time } // สร้างตัวเลือกให้เลือกใน Inspector

    [Header("Item Settings")]
    public ItemType type;
    public float amount; // ถ้าเป็น Health คือจำนวนเลือด, ถ้าเป็น Time คือจำนวนวินาที

    // ฟังก์ชันนี้จะทำงานเมื่อมีอะไรมาชน
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // เช็คว่าคนที่มาชนคือ Player ใช่ไหม (อย่าลืมตั้ง Tag ของผู้เล่นว่า "Player" ด้วยนะ)
        if (collision.CompareTag("Player"))
        {
            Debug.Log("Player picked up an item: " + type.ToString());
            PlayerStats stats = collision.GetComponent<PlayerStats>();
            if (stats != null)
            {
                // เช็คว่าไอเทมนี้เป็นประเภทไหน แล้วส่งค่าไปให้ PlayerStats
                if (type == ItemType.Health)
                {
                    stats.Heal(amount);
                }
                else if (type == ItemType.Time)
                {
                    stats.AddTime(amount);
                }

                // สั่งทำลายตัวเองทิ้งหลังเก็บเสร็จ
                Destroy(gameObject);
            }
            else
            {
                Debug.LogWarning("PlayerStats component not found on the player!");
            }
        }
    }
}