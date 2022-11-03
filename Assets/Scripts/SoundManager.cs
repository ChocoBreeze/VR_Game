using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField]
    private AudioSource _audioSource;
    // Start is called before the first frame update
    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }
    public void AudioPlay()
    {
        if (_audioSource != null)
        {
            UnityEngine.Debug.Log("played!");
            _audioSource.Play();
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
