using System;
using Unity.VisualScripting;
using UnityEditor.Rendering.LookDev;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.TextCore;

// baseplate script for player objects
public class Player : MonoBehaviour
{
    /* ==========  INPUT KEYBINDS  ========== */
    [SerializeField] private float lookSensitivity = 1.0f;

    // strings used for getting axis input
    private string x_movement_axis, y_movement_axis, x_look_axis, y_look_axis, jump;


    /* ==========  CONSTANTS  ========== */

    // Position of the camera relative to the player's centre
    [SerializeField] private Vector2 cameraDisplacement = new Vector2(0.5f, 0.75f);
    // The amount of speed added from user input
    [SerializeField] private float moveSpeed = 20.0f;
    // How much vertical velocity is gained when jumping
    [SerializeField] private float jumpHeight = 10.0f;


    /* ==========  OTHER OBJECTS  ========== */

    // The camera the player sees with
    [SerializeField] private Camera view;

    /* ==========  PLAYER OBJECT VARIABLES  ========== */

    // Rate of change of position
    [SerializeField] private Vector3 velocity = Vector3.zero;
    // input velocity 
    private Vector3 input_vel = Vector3.zero;

    // Unit vector representing where player is looking
    private Vector3 facing = Vector3.forward;

    // Whether or not the player is on the ground
    private bool airborne = true;


    /* ==========  ACCESSORS  ========== */

    // unit vector pointing int the direction the player is facing 
    public Vector3 Facing() { return facing; }


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
    
    /* ==========  HELPER FUNCTIONS  ========== */

    /* Sets controller axis strings based on controller number */
    public void set_axes(int controllerNumber)
    {
        x_movement_axis = "Joy" + controllerNumber + "Horizontal";
        y_movement_axis = "Joy" + controllerNumber + "Vertical";
        x_look_axis = "Look" + controllerNumber + "Horizontal";
        y_look_axis = "Look" + controllerNumber + "Vertical";
        jump = "J" + controllerNumber + "A";
    }

    /* sets velocity based on user input */
    public void get_input()
    {
        float lookXamt = Input.GetAxis(x_look_axis);
        float lookYamt = -Input.GetAxis(y_look_axis);
        float xMove    = Input.GetAxis(x_movement_axis);
        float yMove    = -Input.GetAxis(y_movement_axis);
        
        
        // rotate object around y axis by mouseX
        transform.Rotate(0,lookXamt*lookSensitivity*Time.deltaTime,0);
        // rotate camera around x axis by y movement
        view.transform.Rotate(-lookYamt*lookSensitivity*Time.deltaTime,0,0);

        // find input along axes
        input_vel = new Vector3(xMove, 0f, yMove);

        // update facing direction based on object rotation
        float rad = transform.eulerAngles.y * Mathf.Deg2Rad;
        facing = new Vector3(MathF.Sin(rad), 0.0f, MathF.Cos(rad));

        // find directions for movement
        Vector3 forward = facing * input_vel.z;
        // vector orthogonal to the direction the player is facing
        Vector3 orth = Vector3.Cross(facing, Vector3.up);
        Vector3 sideways = orth * -input_vel.x;

        // get input velocity vector
        float y = velocity.y;
        velocity = forward + sideways;
        // make the vector's magnitude the player's move speed
        velocity.Normalize(); velocity *= moveSpeed;
        velocity.y += y;

        // attempt to jump
        if (Input.GetAxis(jump)==1f && !airborne) velocity.y += jumpHeight; 

        facing.y = -MathF.Sin(view.transform.eulerAngles.x * Mathf.Deg2Rad);
        facing.Normalize();
    }

    /* Sets the camera position relative to the object's centre, based on the facing vector */
    private void set_camera()
    {
        Vector3 disp = new Vector3(facing.x*cameraDisplacement.x, cameraDisplacement.y, facing.z*cameraDisplacement.x);
        view.transform.position = transform.position + disp;
        view.transform.eulerAngles = new Vector3(view.transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.x);
    }

    /* makes the player face in the given direction */
    public void set_facing(Vector3 dir)
    {
        facing = dir;
        transform.eulerAngles = new Vector3(0f,dir.y,dir.z);
        view.transform.eulerAngles = new Vector3(dir.x, transform.eulerAngles.y, transform.eulerAngles.x);
    }

    /* Stop the guy from moving up/down when they hit the ground */    
    void OnCollisionStay(Collision collision)
    {
        // usually you wouldn't do something like this, since this marks the player as 
        // airborne and cancels y velocity when they collide with ANYTHING (not just the ground)
        // I'll fix this if I have time, if not I don't think it's a big deal in this context
        velocity.y = 0f;
        airborne = false;
    }

    /* Stop the guy from moving up/down when they hit the ground */
    void OnCollisionEnter(Collision collision)
    {
        // usually you wouldn't do something like this, since this marks the player as 
        // airborne and cancels y velocity when they collide with ANYTHING (not just the ground)
        // I'll fix this if I have time, if not I don't think it's a big deal in this context
        velocity.y = 0f;
        airborne = false;
    }
}
