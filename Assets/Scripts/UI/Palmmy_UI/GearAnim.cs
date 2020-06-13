using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GearAnim : MonoBehaviour
{
    private Engine myEngine;
    
    // Start is called before the first frame update
    void Awake()
    {
        myEngine = transform.parent.GetComponent<Engine>();
    }

    private void blockEngineAuraOff()
    {
        myEngine.blockEngineAuraOff();
    }
}
