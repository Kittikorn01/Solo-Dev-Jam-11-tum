using System.Collections;
using UnityEngine;
using TMPro;

[System.Serializable]
public class WaveData
{
    public string waveName = "Wave 1";
    public int totalEnemies = 5;
    public float spawnDelay = 2f;
    public GameObject[] enemyPrefabs;
}

public class WaveManager : MonoBehaviour
{
    public static WaveManager instance;

    [Header("Wave Configuration")]
    public WaveData[] waves;
    private int currentWaveIndex = 0;

    private int enemiesSpawnedThisWave = 0;
    private int actualEnemiesSpawned = 0;
    private int enemiesKilledThisWave = 0;
    public bool isWaveActive = false;

    [Header("Spawning Settings")]
    public Transform[] spawnPoints;
    public GameObject warningCirclePrefab;
    public float warningTime = 1f;

    [Header("UI & References")]
    public TextMeshProUGUI waveText;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI amountText;
    public TextMeshProUGUI countdownText;
    public PlayerStats playerStats;

    [Header("Win UI")]
    public GameObject winPanel;

    void Start()
    {
        if (playerStats == null)
            playerStats = FindFirstObjectByType<PlayerStats>();
    }

    public void BeginGame()
    {
        StartCoroutine(StartWave());
    }

    void Awake()
    {
        instance = this;
    }

    public void EnemyDied()
    {
        if (!isWaveActive) return;

        enemiesKilledThisWave++;
        Debug.Log("Enemy Killed! Total: " + enemiesKilledThisWave);

        // เช็คจบเวฟ
        if (enemiesKilledThisWave >= waves[currentWaveIndex].totalEnemies)
        {
            isWaveActive = false; // ปิดเวฟทันทีเพื่อล็อคไม่ให้รันคำสั่งซ้ำ
            StartCoroutine(CompleteWaveRoutine()); // เปลี่ยนไปเรียกโครูทีนเพื่อหน่วงเวลา
        }
    }

    // --- ฟังก์ชันใหม่: หน่วงเวลาก่อนตัดจบเวฟ ---
    IEnumerator CompleteWaveRoutine()
    {
        UpdateUI(); // บังคับอัปเดต UI ให้โชว์เลข 5/5 ทันที

        // หน่วงเวลา 2 วินาที ให้ผู้เล่นได้เห็นเลข 5/5 ค้างไว้บนจอก่อน
        yield return new WaitForSeconds(2f);

        currentWaveIndex++;

        if (currentWaveIndex < waves.Length)
        {
            StartCoroutine(StartWave());
        }
        else
        {
            isWaveActive = false;
            Debug.Log("All Waves Cleared! YOU WIN!");

            if (winPanel != null)
            {
                winPanel.SetActive(true); // เปิดหน้าจอ RGB สุดเท่!
            }

            waveText.text = "ALL CLEARED!";
        }
    }

    void Update()
    {
        UpdateUI();
    }

    IEnumerator StartWave()
    {
        // 1. ล้างค่าทั้งหมดเตรียมพร้อมสำหรับเวฟใหม่
        enemiesSpawnedThisWave = 0;
        actualEnemiesSpawned = 0;
        enemiesKilledThisWave = 0;
        UpdateUI(); // รีเซ็ต UI เป็น 0/X

        WaveData currentWave = waves[currentWaveIndex];

        countdownText.gameObject.SetActive(true);

        // 2. รีเซ็ตเวลา Timer ใหม่ (ให้เริ่มนับใหม่ตั้งแต่ต้นเวฟ)
        if (playerStats != null)
        {
            playerStats.gameTimer = playerStats.maxTime;
        }

        // 3. ระบบนับถอยหลัง 3 วินาทีก่อนเริ่มเสก พร้อมโชว์ตัวเลขบน UI
        for (int i = 3; i > 0; i--)
        {
            countdownText.text = i.ToString(); // เปลี่ยนเลข 3, 2, 1

            // แถม: ทำ Effect ให้เลขมันกระตุกนิดนึงตอนเปลี่ยนเลข (Tangible Feel)
            countdownText.transform.localScale = Vector3.one * 1.5f;
            // โค้ดนี้จะค่อยๆ หดเลขกลับมาขนาดปกติ (Optional)

            yield return new WaitForSeconds(1f);
        }

        countdownText.text = "GO!";
        yield return new WaitForSeconds(0.5f);
        countdownText.gameObject.SetActive(false);

        // 4. เริ่มเวฟของจริง
        waveText.text = currentWave.waveName;
        isWaveActive = true;

        while (enemiesSpawnedThisWave < currentWave.totalEnemies)
        {
            if (playerStats != null && playerStats.isDead) yield break;

            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            GameObject enemyToSpawn = currentWave.enemyPrefabs[Random.Range(0, currentWave.enemyPrefabs.Length)];

            StartCoroutine(SpawnSequence(spawnPoint.position, enemyToSpawn));

            enemiesSpawnedThisWave++;

            yield return new WaitForSeconds(currentWave.spawnDelay);
        }
    }

    IEnumerator SpawnSequence(Vector2 position, GameObject enemyPrefab)
    {
        GameObject warning = Instantiate(warningCirclePrefab, position, Quaternion.identity);

        yield return new WaitForSeconds(warningTime);

        if (warning != null) Destroy(warning);
        Instantiate(enemyPrefab, position, Quaternion.identity);

        actualEnemiesSpawned++;
    }

    void UpdateUI()
    {
        if (playerStats != null)
        {
            // 1. เปลี่ยนการแสดงผลเป็นวินาทีอย่างเดียว (จำนวนเต็ม)
            timerText.text = Mathf.FloorToInt(playerStats.gameTimer).ToString();

            // 2. เช็คสายพลัง Chrono Overdrive (isPower2) เพื่อเปลี่ยนสีตัวอักษรเป็นสีแดง
            if (playerStats.isPower2)
            {
                timerText.color = Color.red;
            }
            else
            {
                // กลับเป็นสีปกติ (ขาว) เมื่อใช้สาย Juggernaut หรือตอนยังไม่ได้เลือก
                timerText.color = Color.white;
            }
        }

        if (currentWaveIndex < waves.Length)
        {
            // อัปเดตจำนวน Enemy ที่กำจัดได้ (เช่น 0 / 5)
            amountText.text = enemiesKilledThisWave + " / " + waves[currentWaveIndex].totalEnemies;
        }
    }
}