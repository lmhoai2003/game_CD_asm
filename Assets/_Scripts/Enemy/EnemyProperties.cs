using UnityEngine;
using Fusion;
using System;
using TMPro;

public class EnemyProperties : NetworkBehaviour
{
    [Networked, OnChangedRender(nameof(OnHPChangedEnemy))]
    public int _hpPlayer { get; set; } = 100;

    public TextMeshPro hpText;


    public override void Spawned()
    {
        hpText.text = _hpPlayer.ToString();
    }

    public void OnHPChangedEnemy()
    {
        hpText.text = _hpPlayer.ToString();
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("viendan"))
        {
            _hpPlayer -= 10;
            if (_hpPlayer <= 0)
            {
                _hpPlayer = 0;

            }
            Debug.Log("Enemy va cham: " + _hpPlayer);

        }
    }

    private void Update()
    {
        if (hpText != null)
        {
            hpText.transform.LookAt(Camera.main.transform);
            hpText.transform.Rotate(0, 180, 0);
        }

    }
}


