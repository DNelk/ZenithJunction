using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
using Random = UnityEngine.Random;

public class ShakeCamera : MonoBehaviour
{
    private bool _shaking = false;
    private void Awake()
    {
    }

    public void CameraShake(float dur, float mag)
    {
        if(_shaking)
            return;
        StartCoroutine(Shake(dur, mag));
    }
    
    private IEnumerator Shake(float dur, float mag)
    {
        BattleStates temp = BattleManager.Instance.BattleState;
        BattleManager.Instance.BattleState = BattleStates.Moving;
        _shaking = true;
        
        Vector3 initPos = transform.position;
        
        float elapsed = 0f;

        while (elapsed < dur)
        {
            float x = Random.Range(-1f, 1f) * mag;
            float y = Random.Range(-1f, 1f) * mag;

            transform.position = new Vector3(initPos.x + x, initPos.y + y, initPos.z);
            elapsed += Time.deltaTime;
            yield return null;
        }

        _shaking = false;
        transform.position = initPos;
        BattleManager.Instance.BattleState = temp;
    }
}
