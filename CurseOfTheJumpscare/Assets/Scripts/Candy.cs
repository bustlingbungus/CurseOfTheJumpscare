#if UNITY_EDITOR

using System;
using System.Threading;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngineInternal;

public class Candy : MonoBehaviour
{
    /* How long it takes to pick up the candy (seconds) */
    [SerializeField] private float pickupTime = 3.0f;
    /* The speed the candy rotates */
    [SerializeField] private float rotationSpeed = 10.0f;
    /* The amount of time the candy has been interacted with */
    private float pickup_time = 0.0f;
    /* Whether or not the candy is being interacted with */
    private bool interaction = false;
    /* Whether or not the candy has been eaten with */
    private bool is_eaten = false;

    // object components
    AudioSource eat_sound;
    MeshRenderer rendering;


    /* ==========  RENDERING PARAMETERS  ========== */
 
    // candy timer rendering options
    private Rect timerRect;
    [SerializeField] private int timerFontSize = 50;
    [SerializeField] private Color timerColour = Color.white;
    // styles used for text rendering
    private GUIStyle timer_style = new GUIStyle(); 

    private Light spotlight;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        timer_style.fontSize = timerFontSize;
        timer_style.normal.textColor = timerColour;
        timerRect = new Rect((Screen.width/4)-(2*timerFontSize),(Screen.height-timerFontSize)/2,200,200);

        eat_sound = GetComponent<AudioSource>();
        rendering = GetComponent<MeshRenderer>();

        spotlight = gameObject.GetComponentInChildren<Light>();
    }

    // Update is called once per frame
    void Update()
    {
        // if the candy is being interacted with, increase the timer
        if (interaction) {
            pickup_time += Time.deltaTime;
            // set interaction to false. If player is still picking it up, it will get set back to true
            interaction = false;
        // else, decrease the candy timer (keep above zero)
        } else if (pickup_time > 0.0f) pickup_time = MathF.Max(0.0f, pickup_time - Time.deltaTime);

        // rotate continuously
        transform.Rotate(new Vector3(0f, Time.deltaTime*rotationSpeed, 0f));
    }

    // GUI rendering 
    void OnGUI()
    {
        if (pickup_time > 0f && !is_eaten) {
            // render total time - time spent
            float time = pickupTime - pickup_time;
            GUI.Label(timerRect, time.ToString("0.00")+'s', timer_style);
        }
    }


    /* ==========  HELPER FUNCTIONS  ==========*/

    // allows the candy's pickup timer to be incremented for one frame
    // if the pickup timer is greater than or equal to the total pickup time, returns true
    public bool interact() {
        interaction = true;
        return pickup_time >= pickupTime;
    }

    // Destroys the candy gameobject
    public void Eat()
    {
        // play sound effect
        eat_sound.Play(0);
        // disable rendering
        rendering.enabled = false;
        spotlight.enabled = false;
        is_eaten = true;
    }

    // un eats the candy
    public void Vomit()
    {
        // enable rendering
        rendering.enabled = true;
        spotlight.enabled = true;
        is_eaten = false;
        
    }
}

#endif