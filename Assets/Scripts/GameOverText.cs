using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverText : MonoBehaviour
{
    private Text _myText;
    // Start is called before the first frame update
    void Start()
    {
        _myText = GetComponent<Text>();
        _myText.text = PlayerPrefs.GetString("GameOverMessage");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
