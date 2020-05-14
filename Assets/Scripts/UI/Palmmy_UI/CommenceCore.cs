using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CommenceCore : MonoBehaviour
{
    private Image u_circular;
    private int _coreTransID;
    private bool turnOn;
    private float _localTrans = 0;

    private Animator _myAnim;
    private ParticleSystem _circularGlow;
    
    // Start is called before the first frame update
    void Start()
    {
        u_circular = transform.Find("Effect").transform.Find("Circular_Aura").GetComponent<Image>();
        u_circular.material = new Material(u_circular.material);
        _coreTransID = Shader.PropertyToID("Core_Trans");

        _myAnim = GetComponent<Animator>();
        _circularGlow = transform.Find("Effect").transform.Find("CircularGlow").GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        if (turnOn)
        {
            if (_localTrans < 1)
            {
                _localTrans += (1f * Time.deltaTime);
                u_circular.material.SetColor(_coreTransID, new Color(1, 1, 1, _localTrans));
            }
            else _localTrans = 1;
        }
        else if (!turnOn)
        {
            if (_localTrans > 0)
            {
                _localTrans -= (1f * Time.deltaTime);
                u_circular.material.SetColor(_coreTransID, new Color(1, 1, 1, _localTrans));
            }
            else _localTrans = 0;
        }

        //for Test
        /*if (Input.GetKeyDown(KeyCode.A))
        {
            _myAnim.SetBool("TurnOn", true);
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            _myAnim.SetBool("TurnOn", false);
        }
        
        if (Input.GetKeyDown(KeyCode.Z))
        {
            _myAnim.SetTrigger("CoreTrigger");
        }*/
    }

    private void turnCircularOn()
    {
        turnOn = true;
    }
    
    private void turnCircularOff()
    {
        turnOn = false;
    }

    private void turnParticleOn()
    {
        _circularGlow.Play();
    }
    
    private void turnParticleOff()
    {
        _circularGlow.Stop();
    }
}
