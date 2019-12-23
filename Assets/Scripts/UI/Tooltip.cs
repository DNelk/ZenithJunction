using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class Tooltip : MonoBehaviour
{
    private TMP_Text _myText;
    private CanvasGroup _cg;
    private float _timer = 5f;

    public string Text
    {
        get => _myText.text;
        set => _myText.text = Utils.ReplaceWithSymbols(value);
    }

    private void Awake()
    {
        _myText = GetComponentInChildren<TMP_Text>();
        _cg = GetComponent<CanvasGroup>();
    }

    // Start is called before the first frame update
    void Start()
    {
        _cg.DOFade(1, 0.2f);
    }

    public IEnumerator FadeOut()
    {
        _timer = 0;
        Tween ft = _cg.DOFade(0, 0.1f);
        yield return ft.WaitForCompletion();
        Destroy(gameObject);
    }

    private void Update()
    {
        if(_timer > 0)
            _timer -= Time.deltaTime;
        if (_timer < 0)
            StartCoroutine(FadeOut());
    }
}
