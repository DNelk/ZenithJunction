using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MovePlayerDialog : MonoBehaviour
{
    private Button _forwardButton;
    private Button _backwardButton;
    private Text _moveText;
    private Button _confirmButton;
    
    private int _currentMoves = 0;
    public bool Confirmed = false;
    public int MoveTotal = 0;
    private int _lastDirection = 0;
    
    private void Start()
    {
        _forwardButton = transform.Find("Forward").GetComponent<Button>();
        _backwardButton = transform.Find("Backward").GetComponent<Button>();
        _moveText = transform.Find("Moves").GetComponent<Text>();
        _moveText.text = MoveTotal + "";
        _confirmButton = transform.Find("Confirm").GetComponent<Button>();
        
        AssignListeners();
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
                yield return new WaitForSeconds(player.ChangePosition(player.Position - 1));
                _currentMoves--;
                _moveText.text = MoveTotal - _currentMoves + "";
                _lastDirection = 0;
            }
            // Out of moves?
            else if (_currentMoves != MoveTotal)
            {
                yield return new WaitForSeconds(player.ChangePosition(player.Position - 1));
                _currentMoves++;
                _moveText.text = MoveTotal - _currentMoves + "";
                _lastDirection = -1;
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
                yield return new WaitForSeconds(player.ChangePosition(player.Position + 1));
                _currentMoves--;
                _moveText.text = MoveTotal - _currentMoves + "";
                _lastDirection = 0;
            }
            // Out of moves?
            else if (_currentMoves != MoveTotal)
            {
                yield return new WaitForSeconds(player.ChangePosition(player.Position + 1));
                _currentMoves++;
                _moveText.text = MoveTotal - _currentMoves + "";
                _lastDirection = 1;
            }
        }
        
        AssignListeners();
    }

    private void Confirm()
    {
        Confirmed = true;
    }

    private void AssignListeners()
    {
        _forwardButton.onClick.AddListener(() => StartCoroutine(Move(-1)));
        _backwardButton.onClick.AddListener(() => StartCoroutine(Move(1)));
        _confirmButton.onClick.AddListener(Confirm);
    }
}
