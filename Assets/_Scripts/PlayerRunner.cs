using UnityEngine;
using Fusion;
using ExitGames.Client.Photon.StructWrapping;

public class PlayerRunner : SimulationBehaviour, IPlayerJoined
{
    [SerializeField] private GameObject playerPrefab1;
    [SerializeField] private GameObject playerPrefab2;

    private readonly Vector3[] spawnPoints = new Vector3[]
    {
        new Vector3(10, 2, 35),
        new Vector3(60, 2, 35)    
    };


    public void PlayerJoined(PlayerRef player)
    {
        if (player == Runner.LocalPlayer)
        {
            // Ví dụ: Lấy lựa chọn nhân vật từ PlayerPrefs (giả định đã lưu trước đó)
            // 0 = nhân vật 1, 1 = nhân vật 2
            int selectedCharacterIndex = PlayerPrefs.GetInt("player", 0);
            // int selectedCharacterIndex = 0;

            GameObject selectedPrefab = selectedCharacterIndex == 1 ? playerPrefab2 : playerPrefab1;

            // Chọn ngẫu nhiên một điểm spawn
            int randomIndex = Random.Range(0, spawnPoints.Length);
            Vector3 spawnPos = spawnPoints[randomIndex];

            // Spawn nhân vật đã chọn
            Runner.Spawn(selectedPrefab, spawnPos, Quaternion.identity, Runner.LocalPlayer, (runner, obj) =>
            {
                var playerSetup = obj.GetComponent<PlayerSetup>();
                if (playerSetup != null)
                {
                    playerSetup.SetupCamera();
                    
                }
                
            });
            
            Debug.Log("tao ra plsyer");

        }
    }



}
