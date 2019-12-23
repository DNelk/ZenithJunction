using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PowerBanner : MonoBehaviour
{
    private TMP_Text _myText;
    private Image _image;
    private Color _myColor;
    
    // Start is called before the first frame update
    void Awake()
    {
        _myText = transform.GetComponentInChildren<TMP_Text>();
        _image = GetComponent<Image>();
        _myColor = _image.color;
    }

    public string Text
    {
        get => _myText.text;
        set => _myText.text = Utils.ReplaceWithSymbols(value);
    }

    public Color Color
    {
        get => _image.color;
        set => _image.color = value;
    }

    public void UseDefaultColor()
    {
        _image.color = _myColor;
    }
}
