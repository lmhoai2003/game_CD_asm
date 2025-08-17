using UnityEngine;
using TMPro;
using Fusion;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Linq;

public class PlayerProperties : NetworkBehaviour
{
    [Networked, OnChangedRender(nameof(OnHPChanged))]
    public int _hpPlayer { get; set; } = 100;

    public TextMeshPro hpText;
    [SerializeField] private Animator animator;

    [SerializeField] private Slider _hpSliderScene;
    [SerializeField] private TextMeshProUGUI nameTextUI;
    private bool checkdie = false;
    [SerializeField] private PlayerMovement movementScript;


    [SerializeField] private RawImage lowHpOverlay; // UI màu đỏ cảnh báo



    public void OnHPChanged()
    {
        if (Object.HasInputAuthority)
        {
            if (hpText != null) hpText.text = _hpPlayer.ToString();
            // if (hpSlider != null) hpSlider.value = _hpPlayer;
            _hpSliderScene.value = _hpPlayer;

            // Kiểm tra HP thấp
             
        }
    }

    private IEnumerator LowHpFlash()
    {
        CanvasGroup cg = lowHpOverlay.GetComponent<CanvasGroup>();
        if (cg == null)
        {
            cg = lowHpOverlay.gameObject.AddComponent<CanvasGroup>();
        }

        while (true)
        {
            float alpha = Mathf.PingPong(Time.time * 2f, 0.5f);
            cg.alpha = alpha;
            yield return null;
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

        if (Object.HasStateAuthority)
        {
            gameObject.tag = "Untagged";
        }

        if (Object.HasInputAuthority)
        {
            DisableMovement();
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
        if (_hpPlayer <= 0) return;
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
