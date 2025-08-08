using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class ChonTuong : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private TextMeshProUGUI textNamePicker;
    [SerializeField] private RawImage loadGameImage;
    [SerializeField] private TextMeshProUGUI textThongBao;

    public void chontuong1()
    {
        PlayerPrefs.SetInt("player", 0);
        PlayerPrefs.SetString("name", "Diaochan");
        textNamePicker.text = "Diaochan";
    }

    // Gọi khi bấm chọn nhân vật 2
    public void chontuong2()
    {
        PlayerPrefs.SetInt("player", 1);
        PlayerPrefs.SetString("name", "LiuBei");
        textNamePicker.text = "LiuBei";
    }

    public void playGame()
    {


        if (textNamePicker.text == "LiuBei" || textNamePicker.text == "Diaochan")
        {
            loadGameImage.gameObject.SetActive(true);
            StartCoroutine(LoadScene2s(2f));
        }
        else
        {
            textThongBao.gameObject.SetActive(true);
            StartCoroutine(ShowTextForSeconds(2f));
        }


    }

    private IEnumerator LoadScene2s(float time)
    {
        yield return new WaitForSeconds(time);
        SceneManager.LoadScene("MainScene");
    }

    private IEnumerator ShowTextForSeconds(float time)
    {
        yield return new WaitForSeconds(time);
        textThongBao.gameObject.SetActive(false);
    }


}
