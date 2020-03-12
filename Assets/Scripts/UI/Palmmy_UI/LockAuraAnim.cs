using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class LockAuraAnim : MonoBehaviour
{
    private Image[] _slotAura;
    private Vector3 _oriScale;
    
    private void Start()
    {
        _slotAura = GetComponentsInChildren<Image>();

        _oriScale = _slotAura[1].transform.localScale;
    }

    void _randomSize()
    {
        for (int i = 1; i < _slotAura.Length; i++)
        {
            float multiplier = Random.Range(0.6f, 1.3f);
            _slotAura[i].transform.localScale = new Vector3(_oriScale.x, _oriScale.y*multiplier, _oriScale.z);
        }
    }
    
    void _resetSize()
    {
        for (int i = 1; i < _slotAura.Length; i++)
        {
            _slotAura[i].transform.localScale = _oriScale;
        }
    }
}
