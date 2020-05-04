using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TrainController : MonoBehaviour
{
    public Waypoint CurrentWP;

    private float _dist = 0;
    private float _per = 0;
    private Vector3 _lastPos;
    public float Speed = 1f;
    private float _t = 0;
    
    private void Start()
    {
        if(!WaypointManager.Instance.NeighborsAssigned)
            WaypointManager.Instance.AssignNeighbors();
        WaypointManager.Instance.RecalculateLinearDist();
        _t = CurrentWP.GetPercentageByDistance(Vector3.Distance(transform.position, CurrentWP.startPoint));
    }

    private void Update()
    {
        if(BattleManager.Instance.BattleState == BattleStates.Moving)
            return;
        _dist = Time.deltaTime * Speed;
        _per = CurrentWP.GetPercentageByDistance(_dist);
        
        //Overtravel check
        if (_t + _per > 1)
        {
            float percentageLeft = 1 - _t;
            float distanceLeft = CurrentWP.GetDistanceByPercentage(percentageLeft);
            float distanceCarryover = _dist - distanceLeft;

            CurrentWP = CurrentWP.Next;
            
            _t = CurrentWP.GetPercentageByDistance(distanceCarryover);
        }
        else
            _t += _per;

        Vector3 placeOnTrack = CurrentWP.GetPositionOnPath(_t); //Where are we on the curve
        _lastPos = transform.position; //Save position
        transform.position = placeOnTrack; //Move cam
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation((placeOnTrack - _lastPos), Vector3.up), 0.05f );
    }
}
