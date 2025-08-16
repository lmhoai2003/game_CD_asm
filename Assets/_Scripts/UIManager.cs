using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("HUD UI")]
    [SerializeField] private Slider hpSlider;
    [SerializeField] private Slider manaSlider;

    private void Awake()
    {
        Instance = this;
    }

    public void UpdateHUD(int hp, int maxHp, int mana, int maxMana)
    {
        if (hpSlider != null)
        {
            hpSlider.maxValue = maxHp;
            hpSlider.value = hp;
        }

        if (manaSlider != null)
        {
            manaSlider.maxValue = maxMana;
            manaSlider.value = mana;
        }
    }
}
