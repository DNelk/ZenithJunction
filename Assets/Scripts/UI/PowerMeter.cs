using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerMeter : MonoBehaviour
{
    public ParticleSystem[] Sparks;

    public void PlaySpark(int index)
    {
        var sparkToPlay = Sparks[index];

        if (sparkToPlay.isPlaying)
        {
            sparkToPlay.Stop();
        }

        sparkToPlay.Play();
    }
}
