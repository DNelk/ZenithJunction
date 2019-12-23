using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class XDialog : MonoBehaviour
{
    private Button _decreaseButton;
    private Button _increaseButton;
    private Text _valueText;
    private Button _confirmButton;
    
    public int XValue = 0;
    public bool XConfirmed = false;

    public int AetherMax = 0;
    private void Start()
    {
        _decreaseButton = transform.Find("Decrease").GetComponent<Button>();
        _increaseButton = transform.Find("Increase").GetComponent<Button>();
        _valueText = transform.Find("Value").GetComponent<Text>();
        _confirmButton = transform.Find("Confirm").GetComponent<Button>();
        
        _decreaseButton.onClick.AddListener(() => ChangeX(-1));
        _increaseButton.onClick.AddListener(() => ChangeX(1));
        _confirmButton.onClick.AddListener(ConfirmX);
    }

    private void ChangeX(int n)
    {
        XValue += n;
        if (XValue > AetherMax)
            XValue -= n;
        if (XValue < 0)
            XValue = 0;
        _valueText.text = XValue.ToString();
    }

    private void ConfirmX()
    {
        if(XValue > AetherMax)
            Utils.DisplayError("Not Enough Focus to pay for X!", 1f);
        else
            XConfirmed = true;
    }
}
