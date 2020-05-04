using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map_nonUIHovering : MonoBehaviour
{
    private SpriteRenderer _mySpriteRenderer;
    public Animator[] _gear;

    public Sprite[] _mySpr;
    
    // Start is called before the first frame update
    void Start()
    {
        _mySpriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnMouseOver()
    {
        if (_mySpr.Length != 0) _mySpriteRenderer.sprite = _mySpr[1];

        if (_gear != null)
        {
            for (int i = 0; i < _gear.Length; i++)
            {
                if (!_gear[i].GetBool("TurnOn"))
                    _gear[i].SetBool("TurnOn", true);
            }
        }
    }

    private void OnMouseExit()
    {
        if (_mySpr.Length != 0)  _mySpriteRenderer.sprite = _mySpr[0];

        if (_gear != null)
        {
            for (int i = 0; i < _gear.Length; i++)
            {
                if (_gear[i].GetBool("TurnOn"))
                    _gear[i].SetBool("TurnOn", false);
            }
        }
    }
}
