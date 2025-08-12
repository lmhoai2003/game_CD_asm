using UnityEngine;
using Fusion;
using System;
using TMPro;

public class EnemyProperties : NetworkBehaviour
{
    [Networked, OnChangedRender(nameof(OnHPChangedEnemy))]
    private int _hpEnemy { get; set; } = 100;

    public TextMeshPro hpText;
    public override void Spawned()
    {
        hpText.text = _hpEnemy.ToString();
    }

    public void OnHPChangedEnemy()
    {
        hpText.text = _hpEnemy.ToString();
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("viendan"))
        {
            _hpEnemy -= 10;
            if (_hpEnemy <= 0)
            {
                _hpEnemy = 0;
                // thực hiện xóa enemy 
                Runner.Despawn(Object);
            }
            Debug.Log("Enemy va cham: " + _hpEnemy);
        }
    }


    public override void FixedUpdateNetwork()
    {
        if (hpText != null)
        {
            hpText.transform.LookAt(Camera.main.transform);
            hpText.transform.Rotate(0, 180, 0);
        }

    }

}


