using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EquippedCardIcon : MonoBehaviour
{
    private TMP_Text _cardName;
    public GameObject CardObj;
    
    public String CardName
    {
        get => _cardName.text;
        set => _cardName.text = value;
    }
    void Awake()
    {
        _cardName = GetComponentInChildren<TMP_Text>();
    }

    public void Unequip()
    {
        CustomizeManager.Instance.DeselectEquipped(this);
    }
}
