
// using UnityEngine;
// using TMPro;
// using Fusion;
// using UnityEngine.UI;
// using System.Collections;
// using UnityEngine.SceneManagement;

// public class PlayerProperties : NetworkBehaviour
// {
//     [Networked, OnChangedRender(nameof(OnHPChanged))]
//     public int _hpPlayer { get; set; } = 100;

//     public TextMeshPro hpText;
//     [SerializeField] private Animator animator;
//     [SerializeField] private Slider hpSlider;
//     [SerializeField] private TextMeshProUGUI nameTextUI;
//     private bool checkdie = false;

//     [SerializeField] private RawImage imageThua;
//     [SerializeField] private RawImage iamgeThang;
//     [SerializeField] private NetworkBehaviour movementScript;
//     [SerializeField] private Button loadSceneHomeButton;

//     public void OnHPChanged()
//     {
//         // Chỉ cập nhật UI máu cho player mình điều khiển
//         if (Object.HasInputAuthority)
//         {
//             if (hpText != null) hpText.text = _hpPlayer.ToString();
//             if (hpSlider != null) hpSlider.value = _hpPlayer;
//         }
//     }

//     public override void FixedUpdateNetwork()
//     {

//     }

//     public override void Spawned()
//     {
//         loadSceneHomeButton.onClick.AddListener(() =>
//         {
//             SceneManager.LoadScene("MenuScene");
//         });
//         nameTextUI.text = PlayerPrefs.GetString("name");


//     }

//     private void DisableMovement()
//     {
//         if (movementScript != null)
//         {
//             movementScript.enabled = false;
//         }
//     }


//     public void OnTriggerEnter(Collider other)
//     {
//         if (!Object.HasStateAuthority) return; // Chỉ xử lý damage ở server/state authority

//         if (other.CompareTag("viendan"))
//         {
//             _hpPlayer -= 5;
//             if (_hpPlayer <= 0)
//             {
//                 _hpPlayer = 0;
//                 if (!checkdie)
//                 {
//                     checkdie = true;
//                     RPC_Die(); // gọi RPC để mọi client thấy animation
//                 }
//             }
//             else
//             {
//                 RPC_Hit();
//             }
//         }
//     }

//     [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
//     void RPC_Die()
//     {
//         animator.SetTrigger("die");

//         //tắt  tag Player
//         if (Object.HasStateAuthority)
//         {
//             gameObject.tag = "Untagged"; // tắt tag Player
//         }

//         if (Object.HasInputAuthority) // chỉ player mình mới khóa di chuyển và hiện thua
//         {
//             DisableMovement();
//             StartCoroutine(ThuaDelay(3f));
//         }
//     }

//     [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
//     void RPC_Hit()
//     {
//         animator.SetTrigger("hitdame");
//     }

//     private IEnumerator ThuaDelay(float time)
//     {

//         yield return new WaitForSeconds(time);
//         if (imageThua != null) imageThua.gameObject.SetActive(true);
//         loadSceneHomeButton.gameObject.SetActive(true);
//     }


// }


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

    // [Networked, OnChangedRender(nameof(SoLuongQuaiConLai))]
    // public int _countQuai { get; set; }

    public TextMeshPro hpText;
    [SerializeField] private Animator animator;
    [SerializeField] private Slider hpSlider;
    [SerializeField] private TextMeshProUGUI nameTextUI;
    private bool checkdie = false;

    [SerializeField] private RawImage imageThua;
    [SerializeField] private RawImage imageThang;
    [SerializeField] private NetworkBehaviour movementScript;
    [SerializeField] private Button loadSceneHomeButton;
    [SerializeField] private TextMeshProUGUI textThongBaoSoLuongEnemy;

    private bool hasWon = false; // Đảm bảo hiệu ứng Win chỉ chạy 1 lần

    public void OnHPChanged()
    {
        // Chỉ cập nhật UI máu cho player mình điều khiển
        if (Object.HasInputAuthority)
        {
            if (hpText != null) hpText.text = _hpPlayer.ToString();
            if (hpSlider != null) hpSlider.value = _hpPlayer;
        }
    }

    public override void Spawned()
    {
        loadSceneHomeButton.onClick.AddListener(() =>
        {
            SceneManager.LoadScene("MenuScene");
        });
        nameTextUI.text = PlayerPrefs.GetString("name");
        // _checkSlEnemy = GameObject.FindGameObjectsWithTag("Emeny").Count();
        // textThongBaoSoLuongEnemy.text = "Quái tinh anh cấp 1 đã bị tiêu diệt";

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

        // tắt tag Player
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

    private void Update()
    {
        if (!Object.HasInputAuthority) return; // Chỉ player điều khiển mới check win

        if (!hasWon && GameObject.FindGameObjectsWithTag("Emeny").Length == 0)
        {
            hasWon = true;
            ShowWinScreen();
        }
        // _checkSlEnemy = GameObject.FindGameObjectsWithTag("Emeny").Length;
        // if (_checkSlEnemy< _countQuai)
        // {
        //     _countQuai = _checkSlEnemy;
        // }

        int timeHoiMau = Random.Range(1, 150);
        if (timeHoiMau >= 149 && _hpPlayer <= 95)
        {
            int hpHoi = Random.Range(1, 5);
            _hpPlayer += hpHoi;
        }


    }

    private void ShowWinScreen()
    {
        if (imageThang != null) imageThang.gameObject.SetActive(true);
        loadSceneHomeButton.gameObject.SetActive(true);
        DisableMovement();
    }
    private IEnumerator HideTextThongBao(float time)
    {
        yield return new WaitForSeconds(time);
        textThongBaoSoLuongEnemy.gameObject.SetActive(false);
    }
}
