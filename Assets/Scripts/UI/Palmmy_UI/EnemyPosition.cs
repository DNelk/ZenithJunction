using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class EnemyPosition : MonoBehaviour
{
    //car pos
    private Image[] _car;
    private Enemy _enemy;

    // Start is called before the first frame update
    void Start()
    {
        _enemy = BattleManager.Instance.CurrentEnemy;
        
        //get player pos
        Transform positionPanel = transform.parent;
        _car = positionPanel.Find("Car").GetComponentsInChildren<Image>();
    }
    
    public void UpdatePosition()
    {
        int pos = _enemy.Position;
        
        transform.DOLocalMove(new Vector3(_car[pos].transform.localPosition.x, transform.localPosition.y, 0), 0.3f);
    }
}
