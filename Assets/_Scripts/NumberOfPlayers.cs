using UnityEngine;
using TMPro;
using Fusion;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class NumberOfPlayers : NetworkBehaviour
{
    [SerializeField] private TextMeshProUGUI playerCountText;
    [SerializeField] private TextMeshProUGUI textMessagePalyer;

    public bool isSpawned = false;

    [Networked, OnChangedRender(nameof(OnPlayerCountChanged))]
    private int playerCount { get; set; }

    private List<PlayerRef> previousPlayers = new List<PlayerRef>();

    public override void Spawned()
    {
        isSpawned = true;

        if (Object.HasStateAuthority)
        {
            playerCount = Runner.ActivePlayers.Count();
            previousPlayers = Runner.ActivePlayers.ToList();
            playerCountText.text = "Live: " + playerCount.ToString();
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (isSpawned && Object.HasStateAuthority)
        {
            List<PlayerRef> currentPlayers = Runner.ActivePlayers.ToList();
            int currentCount = currentPlayers.Count;

            // Kiểm tra nếu có người chơi vào hoặc rời
            if (currentCount != playerCount)
            {
                // So sánh danh sách
                var joinedPlayers = currentPlayers.Except(previousPlayers).ToList();
                var leftPlayers = previousPlayers.Except(currentPlayers).ToList();

                // Cập nhật số lượng
                playerCount = currentCount;

                // Gửi RPC tương ứng
                if (joinedPlayers.Count > 0)
                {
                    foreach (var player in joinedPlayers)
                    {
                        Rpc_ShowMessage(player, true); // true = vào
                    }
                }

                if (leftPlayers.Count > 0)
                {
                    foreach (var player in leftPlayers)
                    {
                        Rpc_ShowMessage(player, false); // false = rời
                    }
                }

                // Cập nhật danh sách cũ
                previousPlayers = currentPlayers;
            }
        }
    }

    public void OnPlayerCountChanged()
    {
        playerCountText.text = "Live: " + playerCount.ToString();
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void Rpc_ShowMessage(PlayerRef player, bool isJoined)
    {
        int id = player.PlayerId;
        playerCountText.text = "Live: " + playerCount.ToString();

        textMessagePalyer.gameObject.SetActive(true);

        if (isJoined)
            textMessagePalyer.text = "Đồng minh Hero " + id + " đã tham chiến";
        else
            textMessagePalyer.text = "Đồng minh Hero " + id + " đã thoát trận";

        StartCoroutine(TextThongBao(5f));
    }

    IEnumerator TextThongBao(float time)
    {
        yield return new WaitForSeconds(time);
        textMessagePalyer.gameObject.SetActive(false);
    }
}
