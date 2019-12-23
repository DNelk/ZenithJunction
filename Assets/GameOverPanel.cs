using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverPanel : MonoBehaviour
{
    private TMP_Text _result;

    private CanvasGroup _cg;
    // Start is called before the first frame update
    void Awake()
    {
        _result = transform.Find("ResultText").GetComponent<TMP_Text>();
        _cg = GetComponent<CanvasGroup>();
        _cg.alpha = 0;
    }

    public void ToMainMenu()
    {
        SceneManager.LoadScene("StartScreen");
    }

    public string Result
    {
        get => _result.text;
        set => _result.text = value;
    }

    public void Load(string msg)
    {
        Result = msg;
        _cg.DOFade(1, 0.2f);
    }
}  
