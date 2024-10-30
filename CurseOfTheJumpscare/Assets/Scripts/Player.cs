using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TextCore;

// baseplate script for player objects
public class Player : MonoBehaviour
{
    /* ==========  INPUT KEYBINDS  ========== */

    public KeyCode FORWARDS = KeyCode.W, BACKWARDS = KeyCode.S,
                   LEFT = KeyCode.A, RIGHT = KeyCode.D, JUMP = KeyCode.Space;

    public float LOOK_SENSITIVITY = 1.0f;


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

    /* ==========  PLAYER OBJECT VARIABLES  ========== */

    // Rate of change of position
    public Vector3 velocity = Vector3.zero;

    // Unit vector representing where player is looking
    private Vector3 facing = Vector3.forward;

    // Whether or not the player is on the ground
    private bool airborne = true;
    // Whether or not the player has a jumpscare currently
    private bool is_jumpscared = false;


    /* ==========  ACCESSORS  ========== */

    // unit vector pointing int the direction the player is facing 
    public Vector3 Facing() { return facing; }
    // Whether or not the player has a jumpscare currently
    public bool isJumpscared() { return is_jumpscared; }
    

    /* ==========  MUTATORS  ========== */

    // toggles jumpscared status
    public void makeScared(bool isScared) { is_jumpscared = isScared; }
    

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

    /* sets velocity based on user input */
    public void get_input(float mouseX, float mouseY)
    {
        // rotate object around y axis by mouseX
        transform.Rotate(0,mouseX*LOOK_SENSITIVITY*Time.deltaTime,0);
        // rotate camera around x axis by y movement
        view.transform.Rotate(-mouseY*LOOK_SENSITIVITY*Time.deltaTime,0,0);

        // find input along axes
        float vertMovement = (Input.GetKey(FORWARDS)?1f:0f) - (Input.GetKey(BACKWARDS)?1f:0f), 
              horzMovement = (Input.GetKey(LEFT)?1f:0f) - (Input.GetKey(RIGHT)?1f:0f); 

        // update facing direction based on object rotation
        float rad = transform.eulerAngles.y * Mathf.Deg2Rad;
        facing = new Vector3(MathF.Sin(rad), 0.0f, MathF.Cos(rad));

        // find directions for movement
        Vector3 forward = facing * vertMovement;
        // vector orthogonal to the direction the player is facing
        Vector3 orth = Vector3.Cross(facing, Vector3.up);
        Vector3 sideways = orth * horzMovement;

        // get input velocity vector
        float y = velocity.y;
        velocity = forward + sideways;
        // make the vector's magnitude the player's move speed
        velocity.Normalize(); velocity *= MOVE_SPEED;
        velocity.y += y;

        // jump when space is pressed
        if (Input.GetKey(JUMP) && !airborne) velocity.y += JUMP_HEIGHT;

        facing.y = -MathF.Sin(view.transform.eulerAngles.x * Mathf.Deg2Rad);
        facing.Normalize();
    }

    /* Sets the camera position relative to the object's centre, based on the facing vector */
    void set_camera()
    {
        Vector3 disp = new Vector3(facing.x*CAMERA_DISPLACEMENT.x, CAMERA_DISPLACEMENT.y, facing.z*CAMERA_DISPLACEMENT.x);
        view.transform.position = transform.position + disp;
        view.transform.eulerAngles = new Vector3(view.transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.x);
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
