using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_enemyInfo : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    public void Init()
    {
        //ref Battlemanager current enemy
        Enemy _currentEnemy = BattleManager.Instance.CurrentEnemy;

        //name
        TextMeshProUGUI enemyName = transform.Find("NameTag").GetComponentInChildren<TextMeshProUGUI>();
        enemyName.text = _currentEnemy.getName();

        //special ability
        TextMeshProUGUI special = transform.Find("Special").GetComponent<TextMeshProUGUI>();
        special.text = "Special Ability : " + _currentEnemy.getSpecial();

            //description
        TextMeshProUGUI desc = transform.Find("Description").GetComponent<TextMeshProUGUI>();
        desc.text = _currentEnemy.getDescription();

        //possible move
        Transform moves = transform.Find("Move List");
        List<EnemyAttack> moveList = null;
        moveList = _currentEnemy.getAttack();

        foreach (var EnemyAttack in moveList)
        {
            GameObject move = Instantiate(Resources.Load("Prefabs/UI/MoveTag"), moves.transform) as GameObject;
            TextMeshProUGUI[] moveText = move.transform.GetComponentsInChildren<TextMeshProUGUI>();
            moveText[0].text = ": " + EnemyAttack.getName(); //add name
            moveText[1].text = ": " + EnemyAttack.getWarning(); //add warning
        }
    }
}
