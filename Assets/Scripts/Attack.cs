using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Attack : MonoBehaviour
{
    [SerializeField] protected int _baseDamage;
    protected int _totalDamage; public int TotalDamage => _totalDamage;
    [SerializeField] protected string _name;
    [SerializeField] protected string _description;
    [SerializeField] protected string _warning;
    public virtual void ExecuteOtherEffect(){}

    public virtual string PrepareAttack()
    {
        _totalDamage = _baseDamage;
        return _warning;
    }
}
