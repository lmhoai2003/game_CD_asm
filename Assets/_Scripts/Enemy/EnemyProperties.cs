using UnityEngine;
using Fusion;
using System;
using TMPro;
using System.Collections;

public class EnemyProperties : NetworkBehaviour
{
    [Networked, OnChangedRender(nameof(OnHPChangedEnemy))]
    public int _hpEnemy { get; set; } = 10;

    public TextMeshPro hpText;
    public TextMeshProUGUI textThongBaoHaGuc;
    public GameObject effectHit;
    public override void Spawned()
    {
        hpText.text = _hpEnemy.ToString();
        textThongBaoHaGuc = GameObject.Find("tmp_haguc").GetComponent<TextMeshProUGUI>();
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
            effectHit.gameObject.SetActive(true);
            StartCoroutine(tathieuungbantrungquai(0.3f));
            if (_hpEnemy <= 0)
            {
                _hpEnemy = 0;
                // thực hiện xóa enemy 
                Runner.Despawn(Object);
                
            }
            Debug.Log("Enemy va cham: " + _hpEnemy);
        }
    }

    private IEnumerator tathieuungbantrungquai(float time)
    {
        yield return new WaitForSeconds(time);
        effectHit.gameObject.SetActive(false);
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


