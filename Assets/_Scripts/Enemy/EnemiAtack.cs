//gắn vào enemy để enemy có thể bắn player
using UnityEngine;
using Fusion;
using UnityEngine.AI;

public class EnemyAttack : NetworkBehaviour
{
    public NetworkObject bulletPrefab;
    public Transform firePoint;
    public float shootCooldown = 3f;  //time hồi chiêu
    public float detectionRange = 20f;  // khoảng cách phát hiện player
    private float shootTimer;
    [SerializeField] private Animator animator;

    [SerializeField] private NavMeshAgent navMeshAgent;

    public override void FixedUpdateNetwork()
    {
        if (!Object.HasStateAuthority) return;

        // Lấy danh sách player (chỉ cần 1 lần trong PlayerManager)
        var players = GameObject.FindGameObjectsWithTag("Player");
        if (players.Length == 0) return;

        // Chọn player gần nhất
        GameObject target = null;
        float minDistance = Mathf.Infinity;

        foreach (var p in players)
        {
            float d = Vector3.Distance(p.transform.position, transform.position);
            if (d < minDistance)
            {
                minDistance = d;
                target = p;
            }
        }

        if (target == null) return;

        // Enemy quay mặt
        Vector3 lookDir = (target.transform.position - transform.position).normalized;
        lookDir.y = 0;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookDir), Time.deltaTime * 5f);

        // Enemy di chuyển
        if (minDistance <= detectionRange)
        {
            navMeshAgent.SetDestination(target.transform.position);
        }
        else
        {
            navMeshAgent.ResetPath();
        }

        // Enemy bắn
        shootTimer -= Time.deltaTime;
        if (shootTimer <= 0f && minDistance <= detectionRange)
        {
            RPC_Shoot(target.transform.position);
            shootTimer = shootCooldown;
        }
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_Shoot(Vector3 targetPos)
    {
        Vector3 dir = (targetPos - firePoint.position).normalized;
        Quaternion rot = Quaternion.LookRotation(dir);

        Runner.Spawn(bulletPrefab, firePoint.position, rot, Object.InputAuthority);
        animator.SetTrigger("enemyAttack");
    }
}
