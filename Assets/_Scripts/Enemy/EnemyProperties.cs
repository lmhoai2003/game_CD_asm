
using UnityEngine;
using Fusion;
using System.Collections;
using TMPro;
using UnityEngine.AI;
using System;
using Unity.VisualScripting;

public class EnemyProperties : NetworkBehaviour
{

    [SerializeField] private int _hpEnemy { get; set; } = 30;
    [SerializeField] private TextMeshPro hpText;
    [SerializeField] private GameObject effectHit;
    [SerializeField] private Animator animator;

    private bool _spawned;
    private int _pendingDamage;     // dồn damage từ va chạm 

    [SerializeField] private NavMeshAgent navMeshAgent;
    public bool isDie = false;

    public override void Spawned()
    {
        _spawned = true;
        if (hpText) hpText.text = _hpEnemy.ToString();
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_OnHPChangedEnemy()
    {
        _pendingDamage += 10;
        if (_pendingDamage > 0)
        {
            _hpEnemy = Mathf.Max(0, _hpEnemy - _pendingDamage);
            _pendingDamage = 0;
        }
        hpText.text = _hpEnemy.ToString();
    }

    private void OnTriggerEnter(Collider other)
    {
        // Có thể bị gọi trước khi Spawned: chặn lại
        if (!_spawned) return;

        // Đạn của bạn dùng tag "viendan"
        if (other.gameObject.CompareTag("viendan"))
        {
            RPC_OnHPChangedEnemy();
        }

        if (effectHit)
        {
            effectHit.SetActive(true);
            StartCoroutine(HideHitFx(0.3f));
        }

        // Để đạn tự hủy trong script Bullet; nếu cần:
        // var bulletNO = other.GetComponentInParent<NetworkObject>();
        // if (bulletNO && bulletNO.IsValid) Runner.Despawn(bulletNO);
    }



    private IEnumerator HideHitFx(float t)
    {
        yield return new WaitForSeconds(t);
        if (effectHit) effectHit.SetActive(false);
    }

    public override void FixedUpdateNetwork()
    {
        // Billboard HP
        if (hpText && Camera.main)
        {
            hpText.transform.LookAt(Camera.main.transform);
            hpText.transform.Rotate(0, 180, 0);
        }

        if (_hpEnemy <= 0)
        {
            //tắt script EnemyAttack và navMeshAgent
            var enemyAttack = GetComponent<EnemyAttack>();
            if (enemyAttack != null && isDie == false)
            {
                enemyAttack.enabled = false;
                navMeshAgent.enabled = false;
                RPC_Die();
            }

        }
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    void RPC_Die()
    {
        if (isDie == false) { animator.SetBool("enemyDie", true); }
        isDie = true;

        gameObject.tag = "Untagged";//tắt tag để người chơi không bắn được nữa
        hpText.gameObject.SetActive(false);
    }
}
