using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleDelegateHandler : MonoBehaviour
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

    public delegate void EnemyEffectDelegate();

    public static EnemyEffectDelegate EnemyEffect;

    public static void ApplyNegativeEnemyEffects()
    {
        if(EnemyEffect == null)
            return;
        EnemyEffect();
        foreach (var d in EnemyEffect.GetInvocationList())
        {
            EnemyEffect -= (d as EnemyEffectDelegate);
        }
    }
}
