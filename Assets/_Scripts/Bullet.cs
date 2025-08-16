using UnityEngine;
using Fusion;
using System.Collections;

public class Bullet : NetworkBehaviour
{
    [SerializeField] private float speed = 20f;
    [SerializeField] private float lifeTime = 3f;
    [SerializeField] private float homingRange = 3f;
    [SerializeField] private float rotateSpeed = 15f;
    [SerializeField] private string enemyTag = "Emeny"; // đúng tag bạn dùng

    private Rigidbody rb;
    private Transform target;
    private Collider col;

    public override void Spawned()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        rb.linearVelocity = transform.forward * speed;

        // disable collider 0.05s để chắc chắn mọi thứ đã Spawned
        if (col) StartCoroutine(EnableColliderSoon(0.05f));

        if (Runner != null && Runner.IsRunning)
        {
            Invoke(nameof(DespawnSelf), lifeTime);
        }
        Invoke(nameof(DespawnSelf), lifeTime);
    }

    private IEnumerator EnableColliderSoon(float t)
    {
        if (col) col.enabled = false;
        yield return new WaitForSeconds(t);
        if (col) col.enabled = true;
    }

    private void FixedUpdate()
    {
        // Tìm target trong tầm khi đang bay
        target = FindNearestEnemyInRange() ?? target;

        if (target)
        {
            Vector3 dir = (target.position - transform.position).normalized;
            Vector3 newDir = Vector3.RotateTowards(transform.forward, dir, rotateSpeed * Time.fixedDeltaTime, 0f);
            rb.linearVelocity = newDir * speed;
            transform.rotation = Quaternion.LookRotation(newDir);
        }
        else
        {
            rb.linearVelocity = transform.forward * speed;
        }
    }

    private Transform FindNearestEnemyInRange()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);
        Transform nearest = null;
        float min = homingRange;

        foreach (var e in enemies)
        {
            float d = Vector3.Distance(transform.position, e.transform.position);
            if (d < min)
            {
                min = d;
                nearest = e.transform;
            }
        }
        return nearest;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(enemyTag))
        {
            // đạn tự hủy – damage thực sự xử lý trong EnemyProperties (pendingDamage)
            DespawnSelf();
        }
    }

    private void DespawnSelf()
    {
        if (Object && Object.IsValid) Runner.Despawn(Object);
        else Destroy(gameObject);
    }
}
