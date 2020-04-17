using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MovePlayerDialog : MonoBehaviour
{
    private Button _forwardButton;
    private Button _backwardButton;
    private TextMeshProUGUI _moveNumber;
    private Button _confirmButton;
    
    private int _currentMoves = 0;
    public bool Confirmed = false;
    public int MoveTotal = 0;
    private int _lastDirection = 0;

    //player pos
    private Transform _playerPos;
    //car pos
    private Image[] _car;
    private Sprite[] _carSprite = new Sprite[2];
    
    private void Start()
    {
        Button[] moveButton = transform.GetComponentsInChildren<Button>();
        
        //arrow button
        _forwardButton = moveButton[0];
        _backwardButton = moveButton[1];
        
        //move number text
        _moveNumber = transform.Find("NumberOfMove").GetComponent<TextMeshProUGUI>();

        //confirm button
        _confirmButton = moveButton[2];
        
        //get player pos
        Transform positionPanel = transform.parent.transform.parent;
        _playerPos = positionPanel.Find("PlayerPos");
        
        //car
        _car = positionPanel.Find("Car").GetComponentsInChildren<Image>();
        _carSprite[0] = Resources.Load<Sprite>("Sprites/CarUI/TrainCar_Blank"); //blank
        _carSprite[1] = Resources.Load<Sprite>("Sprites/CarUI/TrainCar_Pos"); //pos
        
        AssignListeners();
        AssignMoveNumber();
        //gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        AssignMoveNumber();
    }

    private IEnumerator Move(int n)
    {
        Player player = BattleManager.Instance.Player;
        
        _forwardButton.onClick.RemoveAllListeners();
        _backwardButton.onClick.RemoveAllListeners();
        _confirmButton.onClick.RemoveAllListeners();
        
        //Move forward
        if (n < 0)
        {
            //Already at the front
            if (player.Position == 0)
            {
                AssignListeners();
                yield break;
            }

            //Undoing?
            if (_lastDirection > 0)
            {
                //Undoing
                _currentMoves--;
                _moveNumber.text = MoveTotal - _currentMoves + "";
                _lastDirection = 0;
                UpdateCarPosition(-1);
                yield return new WaitForSeconds(player.ChangePosition(player.Position - 1));
            }
            // Out of moves?
            else if (_currentMoves != MoveTotal)
            {
                _currentMoves++;
                _moveNumber.text = (MoveTotal - _currentMoves + "");
                _lastDirection = -1;
                UpdateCarPosition(-1);
                yield return new WaitForSeconds(player.ChangePosition(player.Position - 1));
            }
        }
        else
        {
            //Already at the back
            if (player.Position == 3)
            {
                AssignListeners();
                yield break;
            }

            //Undoing?
            if (_lastDirection < 0)
            {
                //Undoing
                _currentMoves--;
                _moveNumber.text = MoveTotal - _currentMoves + "";
                _lastDirection = 0;
                UpdateCarPosition(1);
                yield return new WaitForSeconds(player.ChangePosition(player.Position + 1));
            }
            // Out of moves?
            else if (_currentMoves != MoveTotal)
            {
                _currentMoves++;
                _moveNumber.text = MoveTotal - _currentMoves + "";
                _lastDirection = 1;
                UpdateCarPosition(1);
                yield return new WaitForSeconds(player.ChangePosition(player.Position + 1));
            }
        }
        
        AssignListeners();
    }

    private void Confirm()
    {
        Confirmed = true;
        BattleManager.Instance.PlayerAttack.AmtMoved = _currentMoves;
    }

    private void AssignListeners()
    {
        _forwardButton.onClick.AddListener(() => StartCoroutine(Move(-1)));
        _backwardButton.onClick.AddListener(() => StartCoroutine(Move(1)));
        _confirmButton.onClick.AddListener(Confirm);
    }
    
    private void AssignMoveNumber()
    {
        if (_moveNumber != null)
            _moveNumber.text = MoveTotal - _currentMoves + "";
    }

    private void UpdateCarPosition(int posChange)
    {
        Player player = BattleManager.Instance.Player;
        int pos =  player.Position + posChange;
        float y_Pos = _playerPos.localPosition.y;
        
        _playerPos.localPosition = new Vector3(_car[pos].transform.localPosition.x, y_Pos, 0);
        transform.position = new Vector3(_playerPos.position.x, transform.position.y, 0);

        for (int i = 0; i < _car.Length; i++)
        {
            if (i == pos)
            {
                _car[i].sprite = _carSprite[1];
                _car[i].SetNativeSize();
            }
            else
            {
                _car[i].sprite = _carSprite[0];
                _car[i].SetNativeSize();
            }
        }
    }
}
