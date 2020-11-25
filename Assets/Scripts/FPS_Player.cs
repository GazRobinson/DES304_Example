using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Stats
{
    public float    MoveSpeed = 5.0f;
    public float    JumpSpeed = 5.0f;
    public int      MaxHealth = 100;
}

// A simple First Person Character controller that works /mostly/ outside the physics system
// Builds upon the built in CharacterController class so that it will interact nicely with collisions
public class FPS_Player : MonoBehaviour
{
    [SerializeField] private Stats m_Stats;

    //A reference to the Player's CharacterController, so that we can control it and call functions later on
    CharacterController m_CharacterController;
    CapsuleCollider m_Collider;
    //What should the current velocity of our Player be?
    Vector3 m_Velocity;

    //[SerializeField] allows us to modify the following variable in the Unity Inspector
    //What is our desired maximum lateral movement speed of the player?    
    [SerializeField] float m_DashSpeedMultiplier = 3.0f;
    [SerializeField] float m_DecayTime = 0.5f;
    float m_SpeedMultiplier = 1.0f;

    //What is our desired maximum rotation speed of the player's camera (in degrees per second)?
    [SerializeField] float m_RotateSpeed = 360.0f;

    bool m_IsGrounded = false;

    //Yaw is our Horizontal rotation
    //Pitch is our vertical rotation
    private float yaw = 0.0f;
    private float pitch = 0.0f;


    [SerializeField] private Gun m_Gun;

    // Start is called before the first frame update
    void Start()
    {
        //Find the reference to our Player's "CharacterController" component
        m_CharacterController = GetComponent<CharacterController>();
        m_Collider = GetComponent<CapsuleCollider>();
        m_Gun = GetComponentInChildren<Gun>();

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        //Accellerate our Player due to the effects of gravity (note: this will apply even when on the ground!)
        //but only if we're not on the ground
        if (m_IsGrounded == false)
        {
            m_Velocity += Physics.gravity * Time.deltaTime;
        }

        //Get the values from the keyboard/controller. A separate Vector2 for the left stick and right stick
        //Store the horizontal (left/right value) in the 'x' and the vertical (up/down value) in the 'y'
        Vector2 leftStickInputDirection  = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        Vector2 rightStickInputDirection = new Vector2(Input.GetAxis("R_Horizontal"), Input.GetAxis("R_Vertical"));

        //We're now seperately keeping track of our yaw and pitch, and then rotating our body and head to match the values
        yaw += rightStickInputDirection.x * Time.deltaTime * m_RotateSpeed;
        pitch -= rightStickInputDirection.y * Time.deltaTime * m_RotateSpeed;

        //Clamp the pitch value between -89 and 89 to avoid flipping the camera
        pitch = Mathf.Clamp(pitch, -89.0f, 89.0f);

        //Set the body's rotation
        //Quaternion is just a mechanism for managing rotations. You don't need to directly work with it.
        //You can just ask it to give you a rotation, which is what we're doing here
        //Give me a rotation of 'yaw' degress around the Y-axis, then apply it to the body
        transform.localRotation = Quaternion.AngleAxis(yaw, Vector3.up);
        //Then we do the same for the head pitch
        Camera.main.transform.localRotation = Quaternion.AngleAxis(pitch, Vector3.right);


        // Get some information about the camera.
        // Which way is it looking now? And which way does it consider "to the right"?
        Vector3 cameraForward = Camera.main.transform.forward;
        Vector3 cameraRight = Camera.main.transform.right;

        // Remove the 'y' component from our "look at" direction.
        // We don't want the player to fly when they move forward!
        cameraForward.y = 0;
        //Normalising the vector turns it into a 'unit vector'; This makes the 'length' of it equal '1'
        //This is a lesson to have outside of comments if you're curious :)
        cameraForward.Normalize();

        //Here we are modifying our "look at" directions to give them
        // a speed based on the input of the left analogue stick
        Vector3 playerForwardMovement   = cameraForward * leftStickInputDirection.y * m_Stats.MoveSpeed * m_SpeedMultiplier;
        Vector3 playerSideMovement      = cameraRight * leftStickInputDirection.x * m_Stats.MoveSpeed * m_SpeedMultiplier;

        //Now we take these values and apply them to our current movement speed
        // We're now 'moving' along the Camera's forward and right directions
        m_Velocity.x = playerForwardMovement.x + playerSideMovement.x;
        m_Velocity.z = playerForwardMovement.z + playerSideMovement.z;

        //Finally tell the CharacterController to 'move' our body based on our velocity!
        // We multiply the value by "Delta time" to make the movement happen over time, rather than a huge speed per frame
        //This makes our movement framerate independant!
        m_CharacterController.Move(m_Velocity * Time.deltaTime);

        if (Input.GetButtonDown("Cross") && m_IsGrounded)
        {
            //Jump
            Debug.Log("Jump!"); //Print a message to the console! This is really helpful for debugging errors!
            m_Velocity.y = 5.0f; //To jump, we set our vertical velocity to a positive value, meaning we'll move up the way
            m_IsGrounded = false;
        }
        //Shoot the gun if the player presses R1
        if (Input.GetButtonDown("R1")||Input.GetMouseButtonDown(0))
        {
            m_Gun.Shoot();
        }
        //Reload the gun if the player presses Square
        if (Input.GetButtonDown("Square")||Input.GetKeyDown(KeyCode.R))
        {
            m_Gun.Reload();
        }
        //Reload the gun if the player presses Square
        if (Input.GetButtonDown("Circle") || Input.GetKeyDown(KeyCode.LeftShift))
        {
            Dash();
        }
        m_IsGrounded = DetectGround();

        m_SpeedMultiplier = Mathf.MoveTowards(m_SpeedMultiplier, 1.0f, (m_DashSpeedMultiplier/m_DecayTime) * Time.deltaTime);
        Camera.main.fieldOfView = Mathf.Lerp(90.0f, 105.0f, Mathf.InverseLerp(1.0f, m_DashSpeedMultiplier, m_SpeedMultiplier));
    }

    bool DetectGround()
    {
        float tolerance = 0.1f;
        if(Physics.Raycast(transform.position, Vector3.down, (m_Collider.height/2.0f) + tolerance))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    void Dash()
    {
        m_SpeedMultiplier = m_DashSpeedMultiplier;
    }

    //Here's a bonus function!
    // We can put code here that we want to trigger when our character touches something (anything right now!)
    private void OnCollisionEnter(Collision collision)
    {
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Debug.Log("Collide!");
        // MATHS EXPLANATION
        
        // 'hit' gives us information about the thing our Collider is touching right now
        // The normal tells us which direction we're touching it on
        // The dot product tells us "How much does this 'normal' direction match the 'up' direction"
        // ... i.e. Is the 'normal' pointing 'up'?

        if (Vector3.Dot(hit.normal, Vector3.up) > 0.5f)
        {
            // If the above conditions are true, then we've landed on something.
            // Set 'Grounded' to true and set our falling speed to 0
            m_IsGrounded = true;
            m_Velocity.y = 0.0f;
        }
    }
}
