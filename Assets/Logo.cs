using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Logo : MonoBehaviour
{
    public GameObject LoadButton;
    public void EnableButton()
    {
        LoadButton.gameObject.SetActive(true);
    }
}
