using System;
using UnityEngine;

public class Jumpscare : MonoBehaviour
{
    // the amount of time (in seconds) the jumpscare will exist
    public float scareTime = 5.0f;

    // audio that plays on jumpscare
    private AudioSource dracula, vine_boom;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // get sounds effects
        var audio = GetComponents<AudioSource>();
        // ensure all effects were found before accessing them 
        if (audio.Length>0) {
            dracula = audio[0]; dracula.Play(0);
        }
        if (audio.Length>1) {
            vine_boom = audio[1]; vine_boom.Play(0);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // jumpscare is over
        if (scareTime <= 0.0f) Destroy(gameObject);
        else scareTime -= Time.deltaTime;
    }
}
