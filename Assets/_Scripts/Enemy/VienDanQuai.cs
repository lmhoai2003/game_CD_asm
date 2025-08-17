using UnityEngine;
using Fusion;

public class VienDanQuai : NetworkBehaviour
{
    public float speed = 20f;
    public float lifeTime = 5f;
    private float timer;

    public override void FixedUpdateNetwork()
    {
        // Đạn di chuyển trên tất cả client giống nhau
        transform.position += transform.forward * speed * Runner.DeltaTime;

        // Đếm thời gian sống
        timer += Runner.DeltaTime;
        if (timer >= lifeTime && Object != null && Object.HasStateAuthority)
        {
            Runner.Despawn(Object);
        }
    }



    void OnTriggerEnter(Collider other)
    {
        if (Object != null && Object.HasStateAuthority)
        {
            if (other.CompareTag("Player"))
            {
                PlayerProperties player = other.GetComponent<PlayerProperties>();
                if (player != null)
                {
                    player.TakeDamage(5);
                }
            }
            Runner.Despawn(Object);
        }
    }

}
