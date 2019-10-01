using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EngineDelegateHandler : MonoBehaviour
{
    public delegate void NextEngineDelegate();

    public static NextEngineDelegate NextEngineEffect;

    public static void ApplyEngineEffects()
    {
        if(NextEngineEffect == null)
            return;
        NextEngineEffect();
        foreach (var d in NextEngineEffect.GetInvocationList())
        {
            NextEngineEffect -= (d as NextEngineDelegate);
        }
    }
}
