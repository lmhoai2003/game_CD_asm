// using UnityEngine;
// using TMPro;
// using Fusion;
// using System.Collections;
// using System.Collections.Generic;
// using System.Linq;

// public class NumberOfPlayers : NetworkBehaviour
// {
//     [Header("UI Player")]
//     [SerializeField] private TextMeshProUGUI playerCountText;
//     [SerializeField] private TextMeshProUGUI textMessagePalyer;

//     [Header("UI Enemy")]
//     [SerializeField] private TextMeshProUGUI enemyCountText;
//     [SerializeField] private TextMeshProUGUI enemyKilledText; // Hiện "Quái đã bị diệt"

//     public bool isSpawned = false;

//     // ================== Networked properties ==================
//     [Networked, OnChangedRender(nameof(OnPlayerCountChanged))]
//     private int playerCount { get; set; }

//     [Networked, OnChangedRender(nameof(OnEnemyCountChanged))]
//     private int enemyCount { get; set; }

//     private List<PlayerRef> previousPlayers = new List<PlayerRef>();

//     public override void Spawned()
//     {
//         isSpawned = true;

//         if (Object.HasStateAuthority)
//         {
//             playerCount = Runner.ActivePlayers.Count();
//             previousPlayers = Runner.ActivePlayers.ToList();
//             enemyCount = GameObject.FindGameObjectsWithTag("Emeny").Length;
//         }

//         UpdatePlayerCountUI();
//         UpdateEnemyCountUI();
//     }

//     public override void FixedUpdateNetwork()
//     {
//         if (!isSpawned) return;

//         // ===== Player count chỉ do Host quản lý =====
//         if (Object.HasStateAuthority)
//         {
//             List<PlayerRef> currentPlayers = Runner.ActivePlayers.ToList();
//             int currentCount = currentPlayers.Count;

//             if (currentCount != playerCount)
//             {
//                 var joinedPlayers = currentPlayers.Except(previousPlayers).ToList();
//                 var leftPlayers = previousPlayers.Except(currentPlayers).ToList();

//                 playerCount = currentCount;

//                 foreach (var player in joinedPlayers)
//                     Rpc_ShowMessage(player, true);

//                 foreach (var player in leftPlayers)
//                     Rpc_ShowMessage(player, false);

//                 previousPlayers = currentPlayers;
//             }

//             // ===== Enemy count chỉ do Host quản lý =====
//             int currentEnemyCount = GameObject.FindGameObjectsWithTag("Emeny").Length;

//             if (currentEnemyCount != enemyCount)
//             {
//                 if (enemyCount != 0 && currentEnemyCount < enemyCount)
//                 {
//                     Rpc_ShowEnemyKilled();
//                 }

//                 enemyCount = currentEnemyCount;
//             }
//         }
//     }

//     // ================== UI Update Methods ==================
//     private void UpdatePlayerCountUI()
//     {
//         if (playerCountText != null)
//         {
//             playerCountText.text = string.Empty;       // clear tránh đè chữ
//             playerCountText.ForceMeshUpdate(true);
//             playerCountText.text = "Live: " + playerCount;
//         }
//     }

//     private void UpdateEnemyCountUI()
//     {
//         if (enemyCountText != null)
//         {
//             enemyCountText.text = string.Empty;        // clear tránh đè chữ
//             enemyCountText.ForceMeshUpdate(true);
//             enemyCountText.text = "Enemy: " + enemyCount;
//         }
//     }

//     // ================== OnChanged Callbacks ==================
//     private void OnPlayerCountChanged()
//     {
//         UpdatePlayerCountUI();
//     }

//     private void OnEnemyCountChanged()
//     {
//         UpdateEnemyCountUI();
//     }

//     // ================== RPC ==================
//     [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
//     public void Rpc_ShowMessage(PlayerRef player, bool isJoined)
//     {
//         int id = player.PlayerId;

//         UpdatePlayerCountUI();

//         if (textMessagePalyer != null)
//         {
//             textMessagePalyer.gameObject.SetActive(true);
//             textMessagePalyer.text = isJoined
//                 ? "Đồng minh Hero " + id + " đã tham chiến"
//                 : "Đồng minh Hero " + id + " đã thoát trận";

//             StopCoroutine(nameof(TextThongBao));
//             StartCoroutine(TextThongBao(2f));
//         }
//     }

//     [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
//     private void Rpc_ShowEnemyKilled()
//     {
//         StopCoroutine(nameof(ShowEnemyKilledMessage));
//         StartCoroutine(ShowEnemyKilledMessage());
//     }

//     // ================== Coroutine ==================
//     private IEnumerator TextThongBao(float time)
//     {
//         yield return new WaitForSeconds(time);
//         textMessagePalyer.gameObject.SetActive(false);
//     }

//     private IEnumerator ShowEnemyKilledMessage()
//     {
//         int capDoQuai = Random.Range(1, 3);
//         enemyKilledText.text = "[ Quái tinh anh cấp " + capDoQuai + " đã bị tiêu diệt! ]";
//         enemyKilledText.gameObject.SetActive(true);
//         yield return new WaitForSeconds(2f);
//         enemyKilledText.gameObject.SetActive(false);
//     }
// }


using UnityEngine;
using TMPro;
using Fusion;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class NumberOfPlayers : NetworkBehaviour
{
    // ========== Singleton ==========
    public static NumberOfPlayers Instance { get; private set; }

    [Header("UI Player")]
    [SerializeField] private TextMeshProUGUI playerCountText;
    [SerializeField] private TextMeshProUGUI textMessagePlayer;

    [Header("UI Enemy")]
    [SerializeField] private TextMeshProUGUI enemyCountText;
    [SerializeField] private TextMeshProUGUI enemyKilledText;

    // ================== Networked properties ==================
    [Networked, OnChangedRender(nameof(OnPlayerCountChanged))]
    private int playerCount { get; set; }

    [Networked, OnChangedRender(nameof(OnEnemyCountChanged))]
    private int enemyCount { get; set; }

    private List<PlayerRef> previousPlayers = new List<PlayerRef>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public override void Spawned()
    {
        if (Object.HasStateAuthority)
        {
            playerCount = Runner.ActivePlayers.Count();
            previousPlayers = Runner.ActivePlayers.ToList();
            enemyCount = GameObject.FindGameObjectsWithTag("Emeny").Length;
            

        }

        UpdatePlayerCountUI();
        UpdateEnemyCountUI();
    }

    public override void FixedUpdateNetwork()
    {
        if (!Object.HasStateAuthority) return;

        // ===== Player count =====
        List<PlayerRef> currentPlayers = Runner.ActivePlayers.ToList();
        int currentCount = GameObject.FindGameObjectsWithTag("Player").Count();

        if (currentCount != playerCount)
        {
            var joinedPlayers = currentPlayers.Except(previousPlayers).ToList();
            var leftPlayers = previousPlayers.Except(currentPlayers).ToList();

            playerCount = currentCount;

            foreach (var player in joinedPlayers)
                Rpc_ShowMessage(player, true);

            foreach (var player in leftPlayers)
                Rpc_ShowMessage(player, false);

            previousPlayers = currentPlayers;
        }

        // ===== Enemy count =====
        int currentEnemyCount = GameObject.FindGameObjectsWithTag("Emeny").Length;

        if (currentEnemyCount != enemyCount)
        {
            if (enemyCount != 0 && currentEnemyCount < enemyCount)
            {
                Rpc_ShowEnemyKilled();
            }
            enemyCount = currentEnemyCount;
        }
    }

    // ================== UI Update Methods ==================
    private void UpdatePlayerCountUI()
    {
        if (playerCountText != null)
            playerCountText.text = "Live: " + playerCount;
    }

    private void UpdateEnemyCountUI()
    {
        if (enemyCountText != null)
            enemyCountText.text = "Enemy: " + enemyCount;
    }

    // ================== OnChanged Callbacks ==================
    private void OnPlayerCountChanged() => UpdatePlayerCountUI();
    private void OnEnemyCountChanged() => UpdateEnemyCountUI();

    // ================== RPC ==================
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void Rpc_ShowMessage(PlayerRef player, bool isJoined)
    {
        int id = player.PlayerId;
        UpdatePlayerCountUI();

        if (textMessagePlayer != null)
        {
            textMessagePlayer.gameObject.SetActive(true);
            textMessagePlayer.text = isJoined
                ? $"Đồng minh Hero {id} đã tham chiến"
                : $"Đồng minh Hero {id} đã thoát trận";

            StopCoroutine(nameof(TextThongBao));
            StartCoroutine(TextThongBao(2f));
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void Rpc_ShowEnemyKilled()
    {
        StopCoroutine(nameof(ShowEnemyKilledMessage));
        StartCoroutine(ShowEnemyKilledMessage());
    }

    // ================== Coroutine ==================
    private IEnumerator TextThongBao(float time)
    {
        yield return new WaitForSeconds(time);
        textMessagePlayer.gameObject.SetActive(false);
    }

    private IEnumerator ShowEnemyKilledMessage()
    {
        int capDoQuai = Random.Range(1, 3);
        enemyKilledText.text = $"[ Quái tinh anh cấp {capDoQuai} đã bị tiêu diệt! ]";
        enemyKilledText.gameObject.SetActive(true);
        yield return new WaitForSeconds(2f);
        enemyKilledText.gameObject.SetActive(false);
    }
}
