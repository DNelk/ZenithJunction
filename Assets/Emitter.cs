using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Emitter : MonoBehaviour
{
    public ParticleSystem MyParticleSystem;

    public void PlayParticle()
    {
        MyParticleSystem.Stop();
        MyParticleSystem.Play();
    }
}
