// using Fusion;
// using UnityEngine;
// using UnityEngine.SceneManagement;
// using System.Threading.Tasks;

// public class AutoNetworkStart : MonoBehaviour
// {
//     private async void Start()
//     {
//         NetworkRunner runner = GetComponent<NetworkRunner>();
//         HideDefaultFusionUI();

//         if (runner == null)
//         {
//             Debug.LogError("Không tìm thấy NetworkRunner!");
//             return;
//         }

//         // Tránh khởi động nhiều lần
//         if (runner.IsRunning) return;

//         // Tự động khởi động Shared Mode và vào phòng
//         await runner.StartGame(new StartGameArgs
//         {
//             GameMode = GameMode.Shared,
//             SessionName = "default_room",
//             SceneManager = GetComponent<NetworkSceneManagerDefault>()
//         });

//         // 👉 Ẩn UI mặc định sau khi StartGame

//     }

//     private void HideDefaultFusionUI()
//     {
//         GameObject uiPanel = GameObject.Find("Prototype Network Start"); // Đổi nếu tên khác
//         if (uiPanel != null)
//         {
//             uiPanel.SetActive(false);
//         }
//     }
// }
using Fusion;
using UnityEngine;

public class AutoNetworkStart : MonoBehaviour
{
    private async void Start()
    {
        var runner = GetComponent<NetworkRunner>();
        HideDefaultFusionUI();

        if (runner == null)
        {
            Debug.LogError("Không tìm thấy NetworkRunner!");
            return;
        }

        if (runner.IsRunning) return;

        // // Người chơi đầu tiên sẽ tạo phòng "room1", những người sau tự join
        // await runner.StartGame(new StartGameArgs
        // {
        //     GameMode = GameMode.Shared,   // Có thể đổi sang Client/Server nếu muốn 1 host authority rõ ràng
        //     SessionName = "room1",        // Tên phòng cố định
        //     SceneManager = GetComponent<NetworkSceneManagerDefault>()
        // });

        await runner.StartGame(new StartGameArgs
        {
            GameMode = GameMode.AutoHostOrClient,
            SessionName = "room1",
            SceneManager = runner.GetComponent<NetworkSceneManagerDefault>()
        });

    }

    private void HideDefaultFusionUI()
    {
        var uiPanel = GameObject.Find("Prototype Network Start");
        if (uiPanel != null)
        {
            uiPanel.SetActive(false);
        }
    }
}
