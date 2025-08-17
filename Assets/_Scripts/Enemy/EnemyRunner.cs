// using System.Collections;
// using UnityEngine;
// using Fusion;
// using System.Collections.Generic;
// using TMPro;
// using UnityEngine.UI;
// using UnityEngine.SceneManagement;

// public class EnemyRunner : NetworkBehaviour
// {
//     [Header("Enemy Settings")]
//     public List<NetworkObject> listEnemy;
//     public int enemyCount = 5;

//     [Header("UI")]
//     [SerializeField] private TextMeshProUGUI countdownText;
//     [SerializeField] private TextMeshProUGUI textThongBao;

//     [Networked] private TickTimer CountdownTimer { get; set; }
//     [Networked] private bool enemiesSpawned { get; set; } // đảm bảo chỉ spawn 1 lần
//     [SerializeField] private RawImage imageWin;
//     [SerializeField] private Button btn_veHome;


//     [Header("Audio Settings")]
//     [SerializeField] private AudioSource audioSource;   // gắn AudioSource vào Inspector
//     [SerializeField] private AudioClip startClip;       // âm thanh bắt đầu
//     [SerializeField] private AudioClip winClip;         // âm thanh chiến thắng


//     private const int countdownDuration = 30;
//     private int checkSoEnemy;

//     public override void Spawned()
//     {
//         if (Object.HasStateAuthority)
//         {
//             if (CountdownTimer.ExpiredOrNotRunning(Runner))
//             {
//                 CountdownTimer = TickTimer.CreateFromSeconds(Runner, countdownDuration);
//                 enemiesSpawned = false;
//             }
//         }
//         btn_veHome.onClick.AddListener(() =>
//         {
//             SceneManager.LoadScene("SettingScene");
//         });

//     }

//     public override void FixedUpdateNetwork()
//     {
//         RPC_SpawnEnemiesTime();
//     }


//     [Rpc(RpcSources.All, RpcTargets.All)]
//     void RPC_SpawnEnemiesTime()
//     {
//         if (CountdownTimer.IsRunning)
//         {
//             int timeLeft = Mathf.CeilToInt(CountdownTimer.RemainingTime(Runner) ?? 0);

//             if (countdownText != null)
//             {
//                 countdownText.text = "Bắt đầu sau: " + timeLeft + "s";
//                 if (timeLeft <= 0 && enemiesSpawned)
//                 {
//                     countdownText.gameObject.SetActive(false);
//                     textThongBao.gameObject.SetActive(false);

//                     Debug.Log("bat dau kiem tra tieu diet quai!");
//                     checkSoEnemy = GameObject.FindGameObjectsWithTag("Emeny").Length;
//                     if (checkSoEnemy == 0)
//                     {
//                         StartCoroutine(WaitAndShowWinScreen(3f));

//                     }


//                 }

//             }

//             if (CountdownTimer.Expired(Runner) && !enemiesSpawned)
//             {
//                 if (Object.HasStateAuthority)
//                 {
//                     SpawnEnemies();
//                     enemiesSpawned = true;
//                 }

//                 if (countdownText != null)
//                     countdownText.text = "";
//             }
//         }
//     }

//     private IEnumerator WaitAndShowWinScreen(float time)
//     {
//         yield return new WaitForSeconds(time);


//             imageWin.gameObject.SetActive(true);
//             btn_veHome.gameObject.SetActive(true);

//     }
//     private void SpawnEnemies()
//     {
//         for (int i = 0; i < enemyCount; i++)
//         {
//             Vector3 randomPos = new Vector3(
//                 Random.Range(-25, 30),
//                 13,
//                 Random.Range(-25, 30)
//             );

//             NetworkObject enemyPrefab = listEnemy[Random.Range(0, listEnemy.Count)];
//             Runner.Spawn(enemyPrefab, randomPos, Quaternion.identity);
//         }

//         Debug.Log("Đã spawn " + enemyCount + " quái vật (chỉ 1 đợt)");
//     }
// }
using System.Collections;
using UnityEngine;
using Fusion;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EnemyRunner : NetworkBehaviour
{
    [Header("Enemy Settings")]
    public List<NetworkObject> listEnemy;
    public int enemyCount = 5;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI countdownText;
    [SerializeField] private TextMeshProUGUI textThongBao;
    [SerializeField] private RawImage imageWin;
    [SerializeField] private Button btn_veHome;

    [Header("Audio Settings")]
    [SerializeField] private AudioSource audioSource;   // gắn AudioSource vào Inspector
    [SerializeField] private AudioClip startClip;       // âm thanh bắt đầu
    [SerializeField] private AudioClip winClip;         // âm thanh chiến thắng

    [Networked] private TickTimer CountdownTimer { get; set; }
    [Networked] private bool enemiesSpawned { get; set; } // đảm bảo chỉ spawn 1 lần

    private const int countdownDuration = 10;
    private int checkSoEnemy;
    private bool checkAudioWin = true;

    public override void Spawned()
    {
        if (Object.HasStateAuthority)
        {
            if (CountdownTimer.ExpiredOrNotRunning(Runner))
            {
                CountdownTimer = TickTimer.CreateFromSeconds(Runner, countdownDuration);
                enemiesSpawned = false;
            }
        }

        btn_veHome.onClick.AddListener(() =>
        {
            SceneManager.LoadScene("SettingScene");
        });
        if (audioSource != null && startClip != null)
            audioSource.PlayOneShot(startClip);
    }

    public override void FixedUpdateNetwork()
    {
        RPC_SpawnEnemiesTime();
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    void RPC_SpawnEnemiesTime()
    {
        if (CountdownTimer.IsRunning)
        {
            int timeLeft = Mathf.CeilToInt(CountdownTimer.RemainingTime(Runner) ?? 0);

            if (countdownText != null)
            {
                countdownText.text = "Bắt đầu sau: " + timeLeft + "s";

                if (timeLeft <= 0 && enemiesSpawned)
                {
                    countdownText.gameObject.SetActive(false);
                    textThongBao.gameObject.SetActive(false);

                    Debug.Log("Bắt đầu kiểm tra tiêu diệt quái!");
                    checkSoEnemy = GameObject.FindGameObjectsWithTag("Emeny").Length;

                    if (checkSoEnemy == 0)
                    {
                        StartCoroutine(WaitAndShowWinScreen(3f));
                    }
                }
            }

            if (CountdownTimer.Expired(Runner) && !enemiesSpawned)
            {
                if (Object.HasStateAuthority)
                {
                    SpawnEnemies();
                    enemiesSpawned = true;

                    // Phát âm thanh bắt đầu (spawn xong)

                }

                if (countdownText != null)
                    countdownText.text = "";
            }
        }
    }

    private IEnumerator WaitAndShowWinScreen(float time)
    {
        yield return new WaitForSeconds(time);

        imageWin.gameObject.SetActive(true);
        btn_veHome.gameObject.SetActive(true);

        // Phát âm thanh chiến thắng
        if (checkAudioWin && audioSource != null && winClip != null)
        {
            audioSource.PlayOneShot(winClip);
            checkAudioWin = false; // Đảm bảo chỉ phát 1 lần
        }

    }

    private void SpawnEnemies()
    {
        for (int i = 0; i < enemyCount; i++)
        {
            Vector3 randomPos = new Vector3(
                Random.Range(-25, 30),
                13,
                Random.Range(-25, 30)
            );

            NetworkObject enemyPrefab = listEnemy[Random.Range(0, listEnemy.Count)];
            Runner.Spawn(enemyPrefab, randomPos, Quaternion.identity);
        }

        Debug.Log("Đã spawn " + enemyCount + " quái vật (chỉ 1 đợt)");
    }
}
