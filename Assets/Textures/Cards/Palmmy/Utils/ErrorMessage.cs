using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ErrorMessage : MonoBehaviour
{
    private Text _message;

    private CanvasGroup _cg;
    
    // Start is called before the first frame update
    private void Start()
    {
        if(_message == null || _cg == null)
            Init();
    }

    private void Init()
    {
        _message = GetComponentInChildren<Text>();
        _cg = GetComponent<CanvasGroup>();
    }

    public IEnumerator StartFade(string msg, float time)
    {
        if(_message == null || _cg == null)
            Init();
        _message.text = msg;
        Tween fadeTween = _cg.DOFade(0.0f, time);
        yield return fadeTween.WaitForCompletion();
        Destroy(gameObject);
    }
}
