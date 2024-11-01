using System;
using System.Threading;
using UnityEngine;
using UnityEngine.UIElements;

public class Candy : MonoBehaviour
{
    /* How long it takes to pick up the candy (seconds) */
    [SerializeField] private float pickupTime = 3.0f;
    /* The speed the candy rotates */
    [SerializeField] private float rotationSpeed = 10.0f;
    /* The amount of time the candy has been interacted with */
    private float pickupTimer = 0.0f;
    /* Whether or not the candy is being interacted with */
    private bool interaction = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // if the candy is being interacted with, increase the timer
        if (interaction) {
            pickupTimer += Time.deltaTime;
            // set interaction to false. If player is still picking it up, it will get set back to true
            interaction = false;
        // else, decrease the candy timer (keep above zero)
        } else if (pickupTimer > 0.0f) pickupTimer = MathF.Max(0.0f, pickupTimer - Time.deltaTime);

        // rotate continuously
        transform.Rotate(new Vector3(0f, Time.deltaTime*rotationSpeed, 0f));
    }

    /* ==========  HELPER FUNCTIONS  ==========*/

    // allows the candy's pickup timer to be incremented for one frame
    // if the pickup timer is greater than or equal to the total pickup time, returns true
    public bool interact() {
        interaction = true;
        return pickupTimer >= pickupTime;
    }

    // Destroys the candy gameobject
    public void Eat()
    {
        // play sound effect
        AudioSource gulp = GetComponent<AudioSource>();
        gulp.Play(0);
        // disable rendering
        GetComponent<MeshRenderer>().enabled = false;
    }
}
