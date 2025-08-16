using UnityEngine;
using Fusion;

public class PlayerProperties2 : NetworkBehaviour
{
    [Header("Player Stats")]
    [Networked, OnChangedRender(nameof(OnHPChanged))]
    public int HP { get; set; } = 100;

    [Networked, OnChangedRender(nameof(OnManaChanged))]
    public int Mana { get; set; } = 100;

    [SerializeField] private int maxHp = 100;
    [SerializeField] private int maxMana = 100;

    public override void Spawned()
    {
        // Khi vừa spawn, cập nhật UI nếu là local player
        if (Object.HasInputAuthority)
        {
            UIManager.Instance.UpdateHUD(HP, maxHp, Mana, maxMana);
        }
    }

    public void OnHPChanged()
    {
        if (Object.HasInputAuthority)
            UIManager.Instance.UpdateHUD(HP, maxHp, Mana, maxMana);
    }

    public void OnManaChanged()
    {
        if (Object.HasInputAuthority)
            UIManager.Instance.UpdateHUD(HP, maxHp, Mana, maxMana);
    }

    public void TakeDamage(int damage)
    {
        if (Object.HasStateAuthority)
        {
            HP = Mathf.Max(HP - damage, 0);
        }
    }

    public void UseMana(int amount)
    {
        if (Object.HasStateAuthority)
        {
            Mana = Mathf.Max(Mana - amount, 0);
        }
    }

    public void RecoverMana(int amount)
    {
        if (Object.HasStateAuthority)
        {
            Mana = Mathf.Min(Mana + amount, maxMana);
        }
    }
}
