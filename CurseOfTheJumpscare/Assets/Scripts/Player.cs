using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TextCore;

// baseplate script for player objects
public class Player : MonoBehaviour
{
    /* ==========  CONSTANTS  ========== */

    // Position of the camera relative to the player's centre
    public Vector2 CAMERA_DISPLACEMENT = new Vector2(0.5f, 0.75f);
    // The amount of speed added from user input
    public float MOVE_SPEED = 20.0f;
    // How much vertical velocity is gained when jumping
    public float JUMP_HEIGHT = 10.0f;


    /* ==========  OTHER OBJECTS  ========== */

    // The camera the player sees with
    public Camera view;
    // global object manager
    public ObjectManager manager;

    /* ==========  Player OBJECT VARIABLES  ========== */

    // Rate of change of position
    public Vector3 velocity = Vector3.zero;

    // Unit vector representing where player is looking
    private Vector3 facing = Vector3.forward;

    // Whether or not the player is on the ground
    private bool airborne = true;


    /* ==========  FUNCTIONS  ========== */

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    public virtual void Update()
    {
        // add velocity to posiiton
        transform.position += velocity * Time.deltaTime;
        // fix rotation
        transform.eulerAngles = new Vector3(0f, transform.eulerAngles.y, 0f);
        
        // set camera position and rotation based on the player's position/rotation
        set_camera();

        // if the y velocity is significant, the player must be falling or flying
        if (MathF.Abs(velocity.y) > 1f) airborne = true;
    }

    /* sets velocity based on user input */
    public void get_input(float vertMovement, float horzMovement, bool jump)
    {
        // update facing direction based on object rotation
        float rad = transform.eulerAngles.y * Mathf.Deg2Rad;
        facing = new Vector3(MathF.Sin(rad), 0f, MathF.Cos(rad));

        // find directions for movement
        Vector3 forward = facing * vertMovement;
        // vector orthogonal to the direction the player is facing
        Vector3 orth = Vector3.Cross(facing, Vector3.up);
        Vector3 sideways = orth * -horzMovement;

        // get input velocity vector
        float y = velocity.y;
        velocity = forward + sideways;
        // make the vector's magnitude the player's move speed
        velocity.Normalize(); velocity *= MOVE_SPEED;
        velocity.y += y;

        // jump when space is pressed
        if (jump && !airborne) velocity.y += JUMP_HEIGHT;
    }

    /* Sets the camera position relative to the objec't centre, based on the facing vector */
    void set_camera()
    {
        Vector3 disp = new Vector3(facing.x*CAMERA_DISPLACEMENT.x, CAMERA_DISPLACEMENT.y, facing.z*CAMERA_DISPLACEMENT.x);
        view.transform.position = transform.position + disp;
        view.transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.x);
    }

    /* Stop the guy from moving up/down when they hit the ground */    
    void OnCollisionStay(Collision collision)
    {
        // for (int i=0, n=collision.contactCount; i<n; i++)
        // {
        //     ContactPoint contact = collision.GetContact(i);
        // }
        velocity.y = 0f;
        airborne = false;
    }

    /* Stop the guy from moving up/down when they hit the ground */
    void OnCollisionEnter(Collision collision)
    {
        // for (int i=0, n=collision.contactCount; i<n; i++)
        // {
        //     ContactPoint contact = collision.GetContact(i);
        // }
        velocity.y = 0f;
        airborne = false;
    }
}
