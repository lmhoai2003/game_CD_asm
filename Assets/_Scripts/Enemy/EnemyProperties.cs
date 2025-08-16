using UnityEngine;
using Fusion;
using System.Collections;
using TMPro;

public class EnemyProperties : NetworkBehaviour
{
    [Networked, OnChangedRender(nameof(OnHPChangedEnemy))]
    public int _hpEnemy { get; set; } = 100;

    [SerializeField] private TextMeshPro hpText;
    [SerializeField] private TextMeshProUGUI textThongBaoHaGuc;
    [SerializeField] private GameObject effectHit;

    private bool _spawned;
    private int _pendingDamage;     // dồn damage từ va chạm (không đụng networked prop ở đây)

    public override void Spawned()
    {
        _spawned = true;
        if (hpText) hpText.text = _hpEnemy.ToString();
    }

    public void OnHPChangedEnemy()
    {
        if (hpText) hpText.text = _hpEnemy.ToString();
    }

    private void OnTriggerEnter(Collider other)
    {
        // Có thể bị gọi trước khi Spawned: chặn lại
        if (!_spawned) return;

        // Đạn của bạn dùng tag "viendan"
        if (!other.CompareTag("viendan")) return;

        // ❌ Không được đụng _hpEnemy ở đây
        _pendingDamage += 10;

        // Hiệu ứng local cho client nào cũng chạy
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

        // Chỉ StateAuthority được ghi networked prop
        if (!HasStateAuthority) return;

        if (_pendingDamage > 0)
        {
            // ✅ Truy cập/ghi _hpEnemy TRONG tick mạng
            _hpEnemy = Mathf.Max(0, _hpEnemy - _pendingDamage);
            _pendingDamage = 0;

            if (_hpEnemy == 0)
            {
                Runner.Despawn(Object);
            }
        }
    }
}
