using UnityEngine;
using UnityEngine.UI;
using Fusion;
using System.Collections;
using TMPro;

public class GunScript : NetworkBehaviour
{
    public NetworkObject bulletPrefabs;
    public Transform firePoint;
    public Slider energySlider;
    public AudioSource audioSource; // AudioSource gắn trên súng hoặc nhân vật

    private int maxEnergy = 100;
    private int currentEnergy;
    private bool isRecharging = false;

    [SerializeField] private float delayMana = 1f;       // chờ xíu rồi hồi mana
    [SerializeField] private float timeMana = 0.1f;      // time hồi 1 lần
    [SerializeField] private int mana = 1;
    [SerializeField] private TextMeshProUGUI thongbaoText;
    [SerializeField] private Slider _manaPlayerSlider;

    // private void Start()
    // {
    //     currentEnergy = maxEnergy;
    //     if (energySlider != null)
    //     {
    //         energySlider.maxValue = maxEnergy;
    //         energySlider.value = currentEnergy;
    //     }
    // }
    public override void Spawned()
    {

        // _manaPlayerSlider = GameObject.Find("manaPlayer").GetComponent<Slider>();
        // _manaPlayerSlider.value = maxEnergy;


        currentEnergy = maxEnergy;
        if (energySlider != null)
        {
            energySlider.maxValue = maxEnergy;
            energySlider.value = currentEnergy;
        }
    }


    public override void FixedUpdateNetwork()
    {
        if (!Object.HasInputAuthority) return;

        if (Input.GetMouseButtonDown(1))
        {
            if (currentEnergy >= 15)
            {
                Shoot();
                currentEnergy -= 10;
                UpdateEnergyUI();

                if (!isRecharging)
                {
                    StartCoroutine(DelayedRechargeEnergy());
                }
            }
            else
            {
                thongbaoText.gameObject.SetActive(true);
                StartCoroutine(TextThongBao(1f));
            }
        }
    }

    void Shoot()
    {
        // Spawn đạn
        var bullet = Runner.Spawn(bulletPrefabs, firePoint.position, firePoint.rotation);

        // Phát tiếng bắn từ AudioSource
        if (audioSource != null)
        {
            audioSource.Play(); // Âm thanh đã gắn sẵn trong AudioSource
        }

        // Thêm lực cho đạn
        if (Camera.main != null)
        {
            Vector3 cameraForward = Camera.main.transform.forward;
            cameraForward.y = 0;
            cameraForward.Normalize();
            bullet.GetComponent<Rigidbody>().AddForce(cameraForward * 20f, ForceMode.Impulse);

            StartCoroutine(DespawnBulletAfterDelay(bullet, 3f));
        }
    }

    IEnumerator TextThongBao(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (thongbaoText != null)
        {
            thongbaoText.gameObject.SetActive(false);
        }
    }

    IEnumerator DespawnBulletAfterDelay(NetworkObject bullet, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (bullet != null)
        {
            Runner.Despawn(bullet);
        }
    }

    IEnumerator DelayedRechargeEnergy()
    {
        isRecharging = true;
        yield return new WaitForSeconds(delayMana);

        while (currentEnergy < maxEnergy)
        {
            currentEnergy += mana;
            currentEnergy = Mathf.Min(currentEnergy, maxEnergy); // giới hạn hồi
            UpdateEnergyUI();
            yield return new WaitForSeconds(timeMana);
        }

        isRecharging = false;
    }

    void UpdateEnergyUI()
    {
        if (energySlider != null)
        {
            energySlider.value = currentEnergy;
        }

        // _manaPlayerSlider.value = currentEnergy;
    }
}
