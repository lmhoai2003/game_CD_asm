using UnityEngine;
using Fusion;
using System.Collections;

public class VienDanQuai : NetworkBehaviour
{
    public float speed = 20f;
    public float lifeTime = 5f;
    private float timer;
    [SerializeField] private AudioSource audioSource;   // gắn AudioSource vào Inspector

    [SerializeField] private AudioClip startClip;       // âm thanh bắt đầu
    [SerializeField] private AudioClip hitClip;         // âm thanh trúng đích

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
    public override void Spawned()
    {
        // Phát âm thanh khi đạn được spawn
        if (audioSource != null && startClip != null)
            audioSource.PlayOneShot(startClip);
    }



    void OnTriggerEnter(Collider other)
    {
        if (Object != null && Object.HasStateAuthority)
        {
            if (other.CompareTag("Player"))
            {
                //phát âm thanh trúng đích

                PlayerProperties player = other.GetComponent<PlayerProperties>();
                if (player != null)
                {
                    player.TakeDamage(5);

                    // Phát âm thanh trúng đích
                    audioSource.PlayOneShot(hitClip);

                }
            }
            StartCoroutine(HideBullet());

        }
    }
    private IEnumerator HideBullet()
    {
        yield return new WaitForSeconds(0.3f);
        if (Object != null && Object.IsValid)
        {
            Runner.Despawn(Object);
        }
    }


}
