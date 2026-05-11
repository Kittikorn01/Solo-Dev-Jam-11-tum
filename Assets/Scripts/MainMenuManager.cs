using UnityEngine;
using UnityEngine.SceneManagement; // จำเป็นสำหรับการโหลด Scene

public class MainMenuManager : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject mainArea;        // หน้าที่มีปุ่ม Start
    public GameObject tutorialArea;    // หน้าที่มีคำอธิบายและปุ่ม Go

    // 1. ฟังก์ชันสำหรับปุ่ม Start (กดแล้วไปหน้าอธิบาย)
    public void ClickStart()
    {
        mainArea.SetActive(false);     // ปิดหน้าเมนูหลัก
        tutorialArea.SetActive(true);  // เปิดหน้าคำอธิบาย
    }

    // 2. ฟังก์ชันสำหรับปุ่ม Go (กดแล้วเริ่มเกมจริง)
    public void ClickGo()
    {
        // โหลดฉากที่ชื่อว่า SampleScene
        // อย่าลืมไปเพิ่ม Scene ใน Build Settings ก่อนนะ!
        SceneManager.LoadScene("SampleScene");
    }

    // 3. ฟังก์ชันแถม: ปุ่ม Quit (เผื่ออยากให้กดออกเกมได้)
    public void ClickQuit()
    {
        Application.Quit();
        Debug.Log("Game Exited");
    }
}