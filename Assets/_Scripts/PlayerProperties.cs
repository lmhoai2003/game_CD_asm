using UnityEngine;
using TMPro;
using Fusion;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Linq;
// using UnityEditor.Rendering.LookDev;

public class PlayerProperties : NetworkBehaviour
{
    [Networked, OnChangedRender(nameof(OnHPChanged))]
    public int _hpPlayer { get; set; } = 100;

    // public TextMeshPro hpText;
    [SerializeField] private Animator animator;

    [SerializeField] private Slider _hpSliderScene;
    [SerializeField] private TextMeshProUGUI nameTextUI;
    private bool checkdie = false;
    [SerializeField] private PlayerMovement movementScript;
    [SerializeField] private GameObject effectHoiSinh;
    [SerializeField] private RawImage lowHpOverlay; // UI màu đỏ cảnh báo
    [SerializeField] private RawImage imageHoisinh;



    public void OnHPChanged()
    {
        if (Object.HasInputAuthority)
        {
            // if (hpText != null) hpText.text = _hpPlayer.ToString();
            // if (hpSlider != null) hpSlider.value = _hpPlayer;
            _hpSliderScene.value = _hpPlayer;

            // Kiểm tra HP thấp

        }
    }



    public override void Spawned()
    {
        _hpSliderScene = GameObject.Find("hpPlayer").GetComponent<Slider>();
        nameTextUI = GameObject.Find("tmp_namePlayer").GetComponent<TextMeshProUGUI>();
        nameTextUI.text = PlayerPrefs.GetString("name");
    }

    private void DisableMovement()
    {
        if (movementScript != null)
        {
            movementScript.enabled = false;
        }
    }


    public void TakeDamage(int amount)
    {
        if (!Object.HasStateAuthority) return;

        _hpPlayer -= amount;
        if (_hpPlayer <= 0)
        {
            _hpPlayer = 0;
            if (!checkdie)
            {
                checkdie = true;
                RPC_Die();
            }
        }
        else
        {
            RPC_Hit();
        }
    }


    [Rpc(RpcSources.All, RpcTargets.All)]
    void RPC_Die()
    {
        animator.SetTrigger("die");

        if (Object.HasInputAuthority)
        {
            gameObject.tag = "Untagged";
        }

        if (Object.HasInputAuthority)
        {
            DisableMovement();
        }

        imageHoisinh.gameObject.SetActive(true);

        StartCoroutine(HoiSinhPlayer(10f));
    }

    IEnumerator HoiSinhPlayer(float time)
    {
        yield return new WaitForSeconds(time);

        if (Object.HasInputAuthority)
        {
            //hồi sinh ở vị trí 5;13;5
            transform.position = new Vector3(8, 10, 0);
            transform.rotation = Quaternion.Euler(0, 0, 0);

            _hpPlayer = 100;
            checkdie = false;
            gameObject.tag = "Player";
            animator.SetTrigger("hoisinh");
            if (movementScript != null) movementScript.enabled = true;

            // Reset UI
            // if (hpText != null) hpText.text = _hpPlayer.ToString();
            _hpSliderScene.value = _hpPlayer;
            imageHoisinh.gameObject.SetActive(false);
        }
    }



    [Rpc(RpcSources.All, RpcTargets.All)]
    void RPC_Hit()
    {
        animator.SetTrigger("hitdame");
    }


    public override void FixedUpdateNetwork()
    {
        // nếu hp <==0 thì không hồi máu
        if (_hpPlayer > 0)
        {
            //hồi 2 máu mỗi giây
            if (Object.HasStateAuthority && _hpPlayer <= 98)
            {
                int checkHoimau = Random.Range(1, 200);

                if (checkHoimau >= 195 && _hpPlayer < 100)
                {
                    int hpHoi = Random.Range(1, 2);
                    _hpPlayer += hpHoi;
                }
            }
        }



    }



}
