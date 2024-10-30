using System;
using UnityEngine;

public class Jumpscare : MonoBehaviour
{
    // the amount of time (in seconds) the jumpscare will exist
    public float scareTime = 5.0f;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // jumpscare is over
        if (scareTime <= 0.0f) Destroy(gameObject);
        else scareTime -= Time.deltaTime;
    }
}
