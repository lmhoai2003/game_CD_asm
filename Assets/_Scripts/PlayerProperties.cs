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
    [SerializeField] private Slider hpSlider;

    [SerializeField] private Slider _hpSliderScene;
    [SerializeField] private TextMeshProUGUI nameTextUI;
    private bool checkdie = false;

    [SerializeField] private RawImage imageThua;
    [SerializeField] private RawImage imageThang;
    [SerializeField] private NetworkBehaviour movementScript;
    [SerializeField] private Button loadSceneHomeButton;

    [SerializeField] private RawImage lowHpOverlay; // UI màu đỏ cảnh báo
    private Coroutine lowHpCoroutine;


    public void OnHPChanged()
    {
        if (Object.HasInputAuthority)
        {
            if (hpText != null) hpText.text = _hpPlayer.ToString();
            if (hpSlider != null) hpSlider.value = _hpPlayer;
            _hpSliderScene.value = _hpPlayer;

            // Kiểm tra HP thấp
            if (_hpPlayer < 30)
            {
                if (lowHpOverlay != null && lowHpCoroutine == null)
                {
                    lowHpOverlay.gameObject.SetActive(true);
                    lowHpCoroutine = StartCoroutine(LowHpFlash());
                }
            }
            else
            {
                if (lowHpOverlay != null)
                {
                    lowHpOverlay.gameObject.SetActive(false);
                    if (lowHpCoroutine != null)
                    {
                        StopCoroutine(lowHpCoroutine);
                        lowHpCoroutine = null;
                    }
                }
            }
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
            // Alpha dao động từ 0 -> 0.5 trong 1s
            float alpha = Mathf.PingPong(Time.time * 2f, 0.5f);
            cg.alpha = alpha;
            yield return null;
        }
    }

    public override void Spawned()
    {
        loadSceneHomeButton.onClick.AddListener(() =>
        {
            SceneManager.LoadScene("MenuScene");
        });

        nameTextUI.text = PlayerPrefs.GetString("name");

        _hpSliderScene = GameObject.Find("hpPlayer").GetComponent<Slider>();

    }

    private void DisableMovement()
    {
        if (movementScript != null)
        {
            movementScript.enabled = false;
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (!Object.HasStateAuthority) return;

        if (other.CompareTag("viendanquai"))
        {
            _hpPlayer -= 5;
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
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
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
            StartCoroutine(ThuaDelay(3f));
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    void RPC_Hit()
    {
        animator.SetTrigger("hitdame");
    }



    private IEnumerator ThuaDelay(float time)
    {
        yield return new WaitForSeconds(time);
        if (imageThua != null) imageThua.gameObject.SetActive(true);
        loadSceneHomeButton.gameObject.SetActive(true);
    }

    private void ShowWinScreen()
    {
        if (imageThang != null) imageThang.gameObject.SetActive(true);
        loadSceneHomeButton.gameObject.SetActive(true);
        DisableMovement();
    }

    public override void FixedUpdateNetwork()
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
