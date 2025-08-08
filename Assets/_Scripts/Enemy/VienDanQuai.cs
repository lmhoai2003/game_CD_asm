using UnityEngine;
using Fusion;
public class VienDanQuai : NetworkBehaviour
{
    public float speed = 20f;
    public float lifeTime = 5f;

    public override void Spawned()
    {
        Invoke(nameof(DestroySelf), lifeTime);
    }

    void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Gây sát thương nếu muốn
            Debug.Log("Enemy bắn trúng player");
        }

        DestroySelf();
    }

    void DestroySelf()
    {
        if (Object != null && Object.HasStateAuthority)
        {
            Runner.Despawn(Object);
        }
    }
}
