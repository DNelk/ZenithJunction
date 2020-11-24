using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetActiveFunction : MonoBehaviour
{
    public void TurnGameObjectOff()
    {
        gameObject.SetActive(false);
    }

    public void DestroyGameObject()
    {
        Destroy(gameObject);
    }
}
