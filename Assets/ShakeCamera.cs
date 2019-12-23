using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
using Random = UnityEngine.Random;

public class ShakeCamera : MonoBehaviour
{
    private bool _shaking = false;
    private Transform _uiFrame;

    private void Awake()
    {
        _uiFrame = GameObject.FindWithTag("UIFrame").transform;
    }

    public void CameraShake(float dur, float mag)
    {
        if(_shaking)
            return;
        StartCoroutine(Shake(dur, mag));
    }
    
    private IEnumerator Shake(float dur, float mag)
    {
        _shaking = true;
        
        Vector3 initPos = transform.position;
        Vector3 frameInitPos = _uiFrame.position;
        
        float elapsed = 0f;

        while (elapsed < dur)
        {
            float x = Random.Range(-1f, 1f) * mag;
            float y = Random.Range(-1f, 1f) * mag;

            transform.position = new Vector3(initPos.x + x, initPos.y + y, initPos.z);
            _uiFrame.position = new Vector3(frameInitPos.x + x, frameInitPos.y + y, frameInitPos.z);
            elapsed += Time.deltaTime;
            yield return null;
        }

        _shaking = false;
        transform.position = initPos;
        _uiFrame.position = frameInitPos;
    }
}
