// using UnityEngine;
// using TMPro;
// using Fusion;
// using UnityEngine.UI;
// using System;
// using System.Collections;


// public class PlayerProperties : NetworkBehaviour
// {
//     [Networked, OnChangedRender(nameof(OnHPChanged))]
//     public int _hpPlayer { get; set; } = 100;



//     public TextMeshPro hpText;
//     [SerializeField] private Animator animator;
//     [SerializeField] private Slider hpSlider;
//     [SerializeField] private TextMeshProUGUI nameTextUI;
//     [SerializeField] private TextMeshProUGUI expPointText;
//     private bool checkdie = false;

//     [SerializeField] private RawImage imageThua;
//     [SerializeField] private NetworkBehaviour movementScript;

//     public void OnHPChanged()
//     {
//         hpText.text = _hpPlayer.ToString();
//         hpSlider.value = _hpPlayer;
//     }

//     private void DisableMovement() //khóa dy chuyển
//     {
//         if (movementScript != null)
//         {
//             movementScript.enabled = false; // vô hiệu hóa script điều khiển
//         }
//     }

//     public override void Spawned()
//     {
//         string playerName = PlayerPrefs.GetString("name");
//         nameTextUI.text = playerName;
//     }

//     public void OnTriggerEnter(Collider other)
//     {
//         if (other.CompareTag("viendan"))
//         {
//             _hpPlayer -= 10;
//             if (_hpPlayer <= 0)
//             {
//                 _hpPlayer = 0;
//                 if (!checkdie)
//                 {
//                     checkdie = true;
//                     animator.SetTrigger("die");

//                     DisableMovement(); // khóa di chuyển khi chết
//                     StartCoroutine(ThuaDelay(3f)); 
//                     StartCoroutine(ExitDelay(6f)); 
//                 }
//             }
//             else
//             {
//                 animator.SetTrigger("hitdame");
//             }
//         }
//     }

//     private void Update()
//     {
//         if (hpText != null)
//         {
//             hpText.transform.LookAt(Camera.main.transform);
//             hpText.transform.Rotate(0, 180, 0);
//         }

//     }
//     private IEnumerator ThuaDelay(float time)
//     {
//         yield return new WaitForSeconds(time);
//         //hiện thua
//         if (imageThua != null)
//         {
//             imageThua.gameObject.SetActive(true);
//         }
//     }

//     private IEnumerator ExitDelay(float time)
//     {
//         yield return new WaitForSeconds(time);
//         //thoát game
//         if (Runner != null && Runner.IsServer)
//         {
//             Runner.Shutdown();
//         }
    
//     }
    
// }
using UnityEngine;
using TMPro;
using Fusion;
using UnityEngine.UI;
using System.Collections;

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

    public void OnHPChanged()
    {
        // Chỉ cập nhật UI máu cho player mình điều khiển
        if (Object.HasInputAuthority)
        {
            if (hpText != null) hpText.text = _hpPlayer.ToString();
            if (hpSlider != null) hpSlider.value = _hpPlayer;
        }
    }

    private void DisableMovement()
    {
        if (movementScript != null)
        {
            movementScript.enabled = false;
        }
    }

    public override void Spawned()
    {
        string playerName = PlayerPrefs.GetString("name");
        nameTextUI.text = playerName;

        // Nếu không phải player mình, ẩn thanh máu UI local
        if (!Object.HasInputAuthority)
        {
            if (hpSlider != null) hpSlider.gameObject.SetActive(false);
            if (imageThua != null) imageThua.gameObject.SetActive(false);
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (!Object.HasStateAuthority) return; // Chỉ xử lý damage ở server/state authority

        if (other.CompareTag("viendan"))
        {
            _hpPlayer -= 10;
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

        if (Object.HasInputAuthority) // chỉ player mình mới khóa di chuyển và hiện thua
        {
            DisableMovement();
            StartCoroutine(ThuaDelay(3f));
            StartCoroutine(ExitDelay(6f));
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    void RPC_Hit()
    {
        animator.SetTrigger("hitdame");
    }

    private IEnumerator ThuaDelay(float time)
    {
        if (imageThua != null) imageThua.gameObject.SetActive(true);
        yield return new WaitForSeconds(time);
        
    }

    private IEnumerator ExitDelay(float time)
    {
        yield return new WaitForSeconds(time);
        if (Runner != null && Runner.IsServer)
        {
            Runner.Shutdown();
        }
    }
}
