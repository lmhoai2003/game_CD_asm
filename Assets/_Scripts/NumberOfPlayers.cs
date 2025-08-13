// using UnityEngine;
// using TMPro;
// using Fusion;
// using System.Collections;
// using System.Collections.Generic;
// using System.Linq;

// public class NumberOfPlayers : NetworkBehaviour
// {
//     [SerializeField] private TextMeshProUGUI playerCountText;
//     [SerializeField] private TextMeshProUGUI textMessagePalyer;

//     public bool isSpawned = false;

//     [Networked, OnChangedRender(nameof(OnPlayerCountChanged))]
//     private int playerCount { get; set; }

//     private List<PlayerRef> previousPlayers = new List<PlayerRef>();

//     public override void Spawned()
//     {
//         isSpawned = true;

//         if (Object.HasStateAuthority)
//         {
//             playerCount = Runner.ActivePlayers.Count();
//             previousPlayers = Runner.ActivePlayers.ToList();
//             playerCountText.text = "Live: " + playerCount.ToString();
//         }
//     }

//     public override void FixedUpdateNetwork()
//     {
//         if (isSpawned && Object.HasStateAuthority)
//         {
//             List<PlayerRef> currentPlayers = Runner.ActivePlayers.ToList();
//             int currentCount = currentPlayers.Count;

//             // Kiểm tra nếu có người chơi vào hoặc rời
//             if (currentCount != playerCount)
//             {
//                 // So sánh danh sách
//                 var joinedPlayers = currentPlayers.Except(previousPlayers).ToList();
//                 var leftPlayers = previousPlayers.Except(currentPlayers).ToList();

//                 // Cập nhật số lượng
//                 playerCount = currentCount;

//                 // Gửi RPC tương ứng
//                 if (joinedPlayers.Count > 0)
//                 {
//                     foreach (var player in joinedPlayers)
//                     {
//                         Rpc_ShowMessage(player, true); // true = vào
//                     }
//                 }

//                 if (leftPlayers.Count > 0)
//                 {
//                     foreach (var player in leftPlayers)
//                     {
//                         Rpc_ShowMessage(player, false); // false = rời
//                     }
//                 }

//                 // Cập nhật danh sách cũ
//                 previousPlayers = currentPlayers;
//             }
//         }
//     }

//     public void OnPlayerCountChanged()
//     {
//         playerCountText.text = "Live: " + playerCount.ToString();
//     }

//     [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
//     public void Rpc_ShowMessage(PlayerRef player, bool isJoined)
//     {
//         int id = player.PlayerId;
//         playerCountText.text = "Live: " + playerCount.ToString();

//         textMessagePalyer.gameObject.SetActive(true);

//         if (isJoined)
//             textMessagePalyer.text = "Đồng minh Hero " + id + " đã tham chiến";
//         else
//             textMessagePalyer.text = "Đồng minh Hero " + id + " đã thoát trận";

//         StartCoroutine(TextThongBao(5f));
//     }

//     IEnumerator TextThongBao(float time)
//     {
//         yield return new WaitForSeconds(time);
//         textMessagePalyer.gameObject.SetActive(false);
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
    [Header("UI Player")]
    [SerializeField] private TextMeshProUGUI playerCountText;
    [SerializeField] private TextMeshProUGUI textMessagePalyer;

    [Header("UI Enemy")]
    [SerializeField] private TextMeshProUGUI enemyCountText;
    [SerializeField] private TextMeshProUGUI enemyKilledText; // Hiện "Quái đã bị diệt"

    public bool isSpawned = false;

    [Networked, OnChangedRender(nameof(OnPlayerCountChanged))]
    private int playerCount { get; set; }

    private List<PlayerRef> previousPlayers = new List<PlayerRef>();

    private int lastEnemyCount = -1; // Lưu số enemy ở frame trước

    public override void Spawned()
    {
        isSpawned = true;

        if (Object.HasStateAuthority)
        {
            playerCount = Runner.ActivePlayers.Count();
            previousPlayers = Runner.ActivePlayers.ToList();
        }

        // Cập nhật ngay ban đầu
        UpdateEnemyCount();
        UpdatePlayerCountUI();
    }

    public override void FixedUpdateNetwork()
    {
        if (!isSpawned) return;

        // ====== 1. Kiểm tra thay đổi số player ======
        if (Object.HasStateAuthority)
        {
            List<PlayerRef> currentPlayers = Runner.ActivePlayers.ToList();
            int currentCount = currentPlayers.Count;

            if (currentCount != playerCount)
            {
                var joinedPlayers = currentPlayers.Except(previousPlayers).ToList();
                var leftPlayers = previousPlayers.Except(currentPlayers).ToList();

                playerCount = currentCount;

                foreach (var player in joinedPlayers)
                {
                    Rpc_ShowMessage(player, true);
                }

                foreach (var player in leftPlayers)
                {
                    Rpc_ShowMessage(player, false);
                }

                previousPlayers = currentPlayers;
            }
        }

        // ====== 2. Kiểm tra thay đổi số enemy ======
        UpdateEnemyCount();
    }

    private void UpdateEnemyCount()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Emeny");
        int currentEnemyCount = enemies.Length;

        // Cập nhật text số lượng enemy
        if (enemyCountText != null)
            enemyCountText.text = "Enemy: " + currentEnemyCount;

        // Nếu số enemy giảm -> hiện thông báo "Quái đã bị diệt"
        if (lastEnemyCount != -1 && currentEnemyCount < lastEnemyCount)
        {
            if (enemyKilledText != null)
            {
                StopCoroutine(nameof(ShowEnemyKilledMessage));
                StartCoroutine(ShowEnemyKilledMessage());
            }
        }

        lastEnemyCount = currentEnemyCount;
    }

    private IEnumerator ShowEnemyKilledMessage()
    {
        int capDoQuai = Random.Range(1, 3);
        enemyKilledText.gameObject.SetActive(true);
        enemyKilledText.text = "[ Quái tinh anh cấp "+capDoQuai+" đã bị tiêu diệt! ]";
        yield return new WaitForSeconds(2f);
        enemyKilledText.gameObject.SetActive(false);
    }

    private void UpdatePlayerCountUI()
    {
        if (playerCountText != null)
            playerCountText.text = "Live: " + playerCount.ToString();
    }

    public void OnPlayerCountChanged()
    {
        UpdatePlayerCountUI();
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void Rpc_ShowMessage(PlayerRef player, bool isJoined)
    {
        int id = player.PlayerId;

        UpdatePlayerCountUI();

        if (textMessagePalyer != null)
        {
            textMessagePalyer.gameObject.SetActive(true);
            textMessagePalyer.text = isJoined
                ? "Đồng minh Hero " + id + " đã tham chiến"
                : "Đồng minh Hero " + id + " đã thoát trận";

            StopCoroutine(nameof(TextThongBao));
            StartCoroutine(TextThongBao(5f));
        }
    }

    IEnumerator TextThongBao(float time)
    {
        yield return new WaitForSeconds(time);
        textMessagePalyer.gameObject.SetActive(false);
    }
}
