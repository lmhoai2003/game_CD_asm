
using UnityEngine;
using TMPro;
using Fusion;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class PlayerProperties : NetworkBehaviour
{
    [Networked, OnChangedRender(nameof(OnHPChanged))]
    public int _hpPlayer { get; set; } = 100;

    public TextMeshPro hpText;
    [SerializeField] private Animator animator;
    [SerializeField] private Slider hpSlider;
    [SerializeField] private TextMeshProUGUI nameTextUI;
    private bool checkdie = false;

    [SerializeField] private RawImage imageThua;
    [SerializeField] private NetworkBehaviour movementScript;
    [SerializeField] private Button loadSceneHomeButton;

    public void OnHPChanged()
    {
        // Chỉ cập nhật UI máu cho player mình điều khiển
        if (Object.HasInputAuthority)
        {
            if (hpText != null) hpText.text = _hpPlayer.ToString();
            if (hpSlider != null) hpSlider.value = _hpPlayer;
        }
    }

    public override void FixedUpdateNetwork()
    {

    }

    public override void Spawned()
    {
        loadSceneHomeButton.onClick.AddListener(() =>
        {
            //load lại scene khi click nút
            SceneManager.LoadScene("MenuScene");
        });

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
        if (!Object.HasStateAuthority) return; // Chỉ xử lý damage ở server/state authority

        if (other.CompareTag("viendan"))
        {
            _hpPlayer -= 5;
            if (_hpPlayer <= 0)
            {
                _hpPlayer = 0;
                if (!checkdie)
                {
                    checkdie = true;
                    RPC_Die(); // gọi RPC để mọi client thấy animation
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

        //tắt  tag Player
        if (Object.HasStateAuthority)
        {
            gameObject.tag = "Untagged"; // tắt tag Player
        }

        if (Object.HasInputAuthority) // chỉ player mình mới khóa di chuyển và hiện thua
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


}
