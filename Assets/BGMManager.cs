using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMManager : MonoBehaviour
{
    public static BGMManager Instance = null;
    private AudioSource _audio;
    // Start is called before the first frame update
    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if(Instance != this)
            Destroy(gameObject);

        _audio = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.M))
            _audio.Stop();
    }
}
