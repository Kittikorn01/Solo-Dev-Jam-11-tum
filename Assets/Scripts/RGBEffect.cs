using UnityEngine;
using UnityEngine.UI;

public class RGBEffect : MonoBehaviour
{
    private Image panelImage;
    public float cycleSpeed = 0.5f; // ความเร็วในการเปลี่ยนสี

    void Start()
    {
        panelImage = GetComponent<Image>();
    }

    void Update()
    {
        // ใช้สูตรเปลี่ยนสีตามค่า Hue (0 ถึง 1) ในระบบสี HSV
        // Mathf.Repeat จะวนลูปค่า 0 -> 1 ไปเรื่อยๆ
        float h = Mathf.Repeat(Time.time * cycleSpeed, 1f);

        // แปลง HSV เป็น RGB โดยให้ Saturation 0.6 และ Value 0.8 เพื่อให้สีดูเป็นพาสเทลนีออน ไม่แสบตาเกินไป
        panelImage.color = Color.HSVToRGB(h, 0.6f, 0.8f);
    }
}