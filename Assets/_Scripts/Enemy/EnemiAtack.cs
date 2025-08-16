//gắn vào enemy để enemy có thể bắn player
using UnityEngine;
using Fusion;
using UnityEngine.AI;

public class EnemiAtack : NetworkBehaviour
{
    public NetworkObject bulletPrefabs;
    public Transform firePoint;
    public float shootCooldown = 5f;
    public float detectionRange = 20f;
    private float shootTimer;

    [SerializeField] private NavMeshAgent navMeshAgent;
    public GameObject[] targets;
    [SerializeField] private float distance;

    public override void FixedUpdateNetwork()
    {
        if (!Object.HasStateAuthority) return;

        targets = GameObject.FindGameObjectsWithTag("Player");
        if (targets.Length == 0) return;

        // Tính khoảng cách đến mục tiêu gần nhất
        GameObject target = null;
        float minDistance = Mathf.Infinity;
        foreach (var t in targets)
        {
            var distance = Vector3.Distance(t.transform.position, transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                target = t;
            }
        }

        // Luôn xoay về player gần nhất
        Vector3 lookDir = (target.transform.position - transform.position).normalized;
        lookDir.y = 0; // giữ hướng xoay trên mặt phẳng
        if (lookDir != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(lookDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
        }

        if (target != null && minDistance <= distance)
        {
            navMeshAgent.SetDestination(target.transform.position);
        }
        else
        {
            navMeshAgent.ResetPath(); // dừng lại nếu quá xa
        }







        shootTimer -= Time.deltaTime;

        // Raycast từ enemy về phía trước
        Ray ray = new Ray(transform.position + Vector3.up, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, detectionRange))
        {
            if (hit.collider.CompareTag("Player"))
            {
                if (shootTimer <= 0f)
                {
                    ShootAtPlayer(hit.collider.transform.position);
                    shootTimer = shootCooldown;
                }
            }
        }



    }

    void ShootAtPlayer(Vector3 targetPos)
    {
        Vector3 direction = (targetPos - firePoint.position).normalized;
        Quaternion rotation = Quaternion.LookRotation(direction);

        Runner.Spawn(bulletPrefabs, firePoint.position, rotation);
    }
}


