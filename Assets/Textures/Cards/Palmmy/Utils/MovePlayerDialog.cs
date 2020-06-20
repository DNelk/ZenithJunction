using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
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

    private PositionAura posAura;
    //car pos
    private Image[] _car;
    private Sprite[] _carSprite = new Sprite[2];
    
    private void Awake()
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
        Transform positionPanel = transform.parent;
        _playerPos = positionPanel.Find("PlayerPos");
        posAura = _playerPos.Find("AuraBack").GetComponent<PositionAura>();

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
        _currentMoves = 0;
        _lastDirection = 0;
        posAura.animate();
    }

    private void OnDisable()
    {
        _currentMoves = 0;
        MoveTotal = 0;
        _lastDirection = 0;
        posAura.stopAnimate();
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
                if (MoveTotal >= 100)
                    _moveNumber.text = "Infinite";
                _lastDirection = 0;
                UpdateCarPosition(-1);
                yield return new WaitForSeconds(player.ChangePosition(player.Position - 1));
            }
            // Out of moves?
            else if (_currentMoves != MoveTotal)
            {
                _currentMoves++;
                _moveNumber.text = (MoveTotal - _currentMoves + "");
                if (MoveTotal >= 100)
                    _moveNumber.text = "Infinite";
                _lastDirection = -1;
                UpdateCarPosition(-1);
                yield return new WaitForSeconds(player.ChangePosition(player.Position - 1));
            }
        }
        else
        {
            //Already at the back
            if (player.Position == 2)
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
                if (MoveTotal >= 100)
                    _moveNumber.text = "Infinite";
                _lastDirection = 0;
                UpdateCarPosition(1);
                yield return new WaitForSeconds(player.ChangePosition(player.Position + 1));
            }
            // Out of moves?
            else if (_currentMoves != MoveTotal)
            {
                _currentMoves++;
                _moveNumber.text = MoveTotal - _currentMoves + "";
                if (MoveTotal >= 100)
                    _moveNumber.text = "Infinite";
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
        {
            _moveNumber.text = MoveTotal - _currentMoves + "";
            if (MoveTotal >= 100)
                _moveNumber.text = "Infinite";
        }
    }

    private void UpdateCarPosition(int posChange)
    {
        Player player = BattleManager.Instance.Player;
        int pos =  player.Position + posChange;
        float y_Pos = _playerPos.localPosition.y;
        float car_Xpos_Local = _car[pos].transform.localPosition.x * _car[pos].transform.parent.localScale.x + _car[pos].transform.parent.localPosition.x;

        //_playerPos.localPosition = new Vector3(_car[pos].transform.localPosition.x, y_Pos, 0);

        _playerPos.DOLocalMove(new Vector3(car_Xpos_Local, y_Pos, 0), 0.3f);
        
        //transform.position = new Vector3(_playerPos.position.x, transform.position.y, 0);
        
        transform.DOLocalMove(new Vector3(car_Xpos_Local, transform.localPosition.y, 0), 0.3f);

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
