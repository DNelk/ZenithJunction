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
        if (NextEngineEffect != null)
        {
            foreach (var d in NextEngineEffect.GetInvocationList())
            {
                NextEngineEffect -= (d as NextEngineDelegate);
            }
        }
    }
    
    

    public delegate void EnemyEffectDelegate();

    public static EnemyEffectDelegate EnemyEffect;

    public static void ApplyNegativeEnemyEffects()
    {
        if(EnemyEffect == null)
            return;
        EnemyEffect();
        if (EnemyEffect != null)
        {
            foreach (var d in EnemyEffect.GetInvocationList())
            {
                EnemyEffect -= (d as EnemyEffectDelegate);
            }
        }
    }

    public static void ClearAllDelegates()
    {
        if (NextEngineEffect != null)
        {
            foreach (var d in NextEngineEffect.GetInvocationList())
            {
                NextEngineEffect -= (d as NextEngineDelegate);
            }
        }

        if (EnemyEffect != null)
        {
            foreach (var d in EnemyEffect.GetInvocationList())
            {
                EnemyEffect -= (d as EnemyEffectDelegate);
            }
        }
    }
    
    public delegate void EnemyMoveDelegate();

    public static EnemyMoveDelegate EnemyMove;

    public static void MoveEnemy()
    {
        if(EnemyMove == null)
            return;
        EnemyMove();
        if (EnemyMove != null)
        {
            foreach (var d in EnemyMove.GetInvocationList())
            {
                EnemyMove -= (d as EnemyMoveDelegate);
            }
        }
    }
    
    public delegate void AfterDamageDelegate();

    public static AfterDamageDelegate AfterDamageEffect;

    public static void ApplyAfterDamageEffects()
    {
        if(AfterDamageEffect == null)
            return;
        AfterDamageEffect();
        if (AfterDamageEffect != null)
        {
            foreach (var d in AfterDamageEffect.GetInvocationList())
            {
                AfterDamageEffect -= (d as AfterDamageDelegate);
            }
        }
    }
    
}
