using System.Collections;
using System.Collections.Generic;
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
    }
    
    public void UpdatePosition()
    {
        int pos = _enemy.Position;
        
        transform.localPosition = new Vector3(_car[pos].transform.localPosition.x, transform.localPosition.y, 0);
    }
}
