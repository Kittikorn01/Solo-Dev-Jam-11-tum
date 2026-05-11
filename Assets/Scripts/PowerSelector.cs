using UnityEngine;

public class PowerSelector : MonoBehaviour
{
    [Header("UI References")]
    public GameObject selectionPanel; // ลาก Panel เลือกพลังมาใส่ตรงนี้

    // ฟังก์ชันสำหรับปุ่มที่ 1 (สายตีแรง/ฟันช้า)
    public void SelectPower1()
    {
        if (PlayerStats.instance != null)
        {
            PlayerStats.instance.isPower1 = true;
            PlayerStats.instance.isPower2 = false;
        }
        StartGame();
    }

    // ฟังก์ชันสำหรับปุ่มที่ 2 (สายตีเบา/รัวเมาส์)
    public void SelectPower2()
    {
        if (PlayerStats.instance != null)
        {
            PlayerStats.instance.isPower1 = false;
            PlayerStats.instance.isPower2 = true;
        }
        StartGame();
    }

    private void StartGame()
    {
        // 1. ปิดหน้าต่างเลือกพลัง
        if (selectionPanel != null)
        {
            selectionPanel.SetActive(false);
        }

        // 2. สั่งให้ WaveManager เริ่มนับถอยหลัง 3..2..1 แล้วปล่อยมอนสเตอร์
        if (WaveManager.instance != null)
        {
            WaveManager.instance.BeginGame();
        }
    }
}