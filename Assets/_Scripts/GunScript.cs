using UnityEngine;
using UnityEngine.UI;
using Fusion;
using System.Collections;
using TMPro;

public class GunScript : NetworkBehaviour
{
    public NetworkObject bulletPrefabs;
    public Transform firePoint;
    // public Slider energySlider;
    public AudioSource audioSource; // AudioSource gắn trên súng hoặc nhân vật

    private int maxEnergy = 100;
    private int currentEnergy;
    private bool isRecharging = false;

    [SerializeField] private float delayMana = 5f;       // chờ xíu rồi hồi mana
    [SerializeField] private float timeMana = 0.1f;      // time hồi 1 lần
    [SerializeField] private int mana = 1;
    [SerializeField] private TextMeshProUGUI thongbaoText;
    [SerializeField] private Slider _manaPlayerSlider;


    // public override void Spawned()
    // {

    //     _manaPlayerSlider = GameObject.Find("manaPlayer").GetComponent<Slider>();

    //     _manaPlayerSlider.value = maxEnergy;


    //     currentEnergy = maxEnergy;
    //     if (_manaPlayerSlider != null)
    //     {
    //         _manaPlayerSlider.maxValue = maxEnergy;
    //         _manaPlayerSlider.value = currentEnergy;
    //     }
    // }

    public override void Spawned()
    {
        _manaPlayerSlider = GameObject.Find("manaPlayer").GetComponent<Slider>();
        _manaPlayerSlider.maxValue = maxEnergy;

        currentEnergy = maxEnergy;
        _manaPlayerSlider.value = currentEnergy;

        // chạy hồi mana suốt game
        StartCoroutine(RechargeEnergy());
    }



    public override void FixedUpdateNetwork()
    {
        if (!Object.HasInputAuthority) return;

        // if (Input.GetMouseButtonDown(1))
        // {
        //     if (currentEnergy >= 15)
        //     {
        //         Shoot();
        //         currentEnergy -= 10;
        //         UpdateEnergyUI();

        //         if (!isRecharging)
        //         {
        //             StartCoroutine(DelayedRechargeEnergy());
        //         }
        //     }
        //     else
        //     {
        //         thongbaoText.gameObject.SetActive(true);
        //         StartCoroutine(TextThongBao(1f));
        //     }
        // }

        if (Input.GetMouseButtonDown(1))
        {
            if (currentEnergy >= 15)
            {
                Shoot();
                currentEnergy -= 10;
                UpdateEnergyUI();
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
            audioSource.Play();
        }

        // Chỉ cần spawn, không cần AddForce nữa
        StartCoroutine(DespawnBulletAfterDelay(bullet, 3f));
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
        if (_manaPlayerSlider != null)
        {
            _manaPlayerSlider.value = currentEnergy;
        }

        // _manaPlayerSlider.value = currentEnergy;
    }

    IEnumerator RechargeEnergy()
    {
        while (true)
        {
            if (!isRecharging && currentEnergy < maxEnergy)
            {
                isRecharging = true;

                yield return new WaitForSeconds(delayMana); // chờ một chút rồi bắt đầu hồi

                while (currentEnergy < maxEnergy)
                {
                    currentEnergy += mana;
                    currentEnergy = Mathf.Min(currentEnergy, maxEnergy);
                    UpdateEnergyUI();
                    yield return new WaitForSeconds(timeMana);
                }

                isRecharging = false;
            }

            yield return null;
        }
    }

}
