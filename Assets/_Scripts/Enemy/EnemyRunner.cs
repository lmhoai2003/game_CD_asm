//tạo game object enemy gắn vào
using UnityEngine;
using Fusion;
using System.Collections.Generic;

public class EnemyRunner : NetworkBehaviour
{
    public List<NetworkObject> listEnemy;
    public int enemyCount = 5;

    public override void Spawned()
    {
        if (Object.HasStateAuthority)
        {
            for (int i = 0; i < enemyCount; i++)
            {
                Vector3 randomPos = new Vector3(Random.Range(30, 70), 10, Random.Range(30, 70)); //random vị trí trong pham vi

                NetworkObject enemyPrefab = listEnemy[Random.Range(0, listEnemy.Count)];
                Runner.Spawn(enemyPrefab, randomPos, Quaternion.identity);
            }
        }
    }

   
}
