using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class CardPreview : MonoBehaviour
{
    private GameObject _myCard;
    private CanvasGroup _cg;

    private void Awake()
    {
        _cg = GetComponent<CanvasGroup>();
        _cg.alpha = 0;
    }

    public void SetCard(Card c)
    {
        _myCard = Instantiate(Resources.Load<GameObject>("Prefabs/Cards/" + c.CardName.Replace(" ", String.Empty)), transform.GetChild(0));
        FadeIn();
    }

    private void FadeIn()
    {
        _cg.DOFade(1, 0.2f);
    }

    public void Destroy()
    {
        _cg.DOFade(0, 0.1f).OnComplete(() =>
        {
            Destroy(gameObject);
            Utils.CurrentPreview = null;
        });
    }
}
