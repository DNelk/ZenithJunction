using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EngineMagicCircle : MonoBehaviour
{
    private GameObject _magicCircle_front, _magicCircle_Back;
    private ParticleSystem _magiceParticle;
    
    void Awake()
    {
        _magicCircle_front = transform.Find("MagicCircle_Effect").transform.Find("MagicCircle_Front").gameObject;
        _magicCircle_Back = transform.Find("MagicCircle_Effect").transform.Find("MagicCircle_Back").gameObject;
        _magiceParticle = transform.Find("MagicCircle_Effect").transform.Find("CircularGlow").GetComponent<ParticleSystem>();
    }

    public void turnOnParticle()
    {
        _magiceParticle.Play();
    }

    public void turnOffParticle()
    {
        _magiceParticle.Stop();
    }
    
    public void turnONCircle()
    {
        _magicCircle_front.SetActive(true);
        _magicCircle_Back.SetActive(true);
    }
    
    public void turnOffCircle()
    {
        _magicCircle_front.SetActive(false);
        _magicCircle_Back.SetActive(false);
    }
}
