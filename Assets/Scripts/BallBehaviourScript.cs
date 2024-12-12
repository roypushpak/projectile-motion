using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BallBehaviourScript : MonoBehaviour
{
    private Rigidbody basketballRb;
    private Transform basketball;
    private Transform goalDetector;
    private Vector3 initialPosition;
    public float horizontalForce;
    public float verticalForce;
    private bool simulationPaused;
    private bool explanationToggled;

    //Audio
    [Header("------Audio Source------")]
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource musicSource2;

    [Header("------Audio Clip------")]
    public AudioClip throwSound;
    public AudioClip Goal;

    private TextMeshProUGUI PhenomenonExplanation;
    private TextMeshProUGUI Instructions; 

    //Sets the initial values for the ball.
    void Start()
    {
        explanationToggled = false;
        horizontalForce = 1.68f;
        verticalForce = 2.5f;

        //Get basketball and goal detector objects by name
        basketball = GameObject.Find("Basketball").transform;
        goalDetector = GameObject.Find("GoalDetector").transform;

        //Finds and sets the Instructions and Explanation TMP texts
        Instructions = GameObject.Find("Canvas/Instructions").GetComponent<TextMeshProUGUI>();
        PhenomenonExplanation = GameObject.Find("Canvas/PhenomenonExplanation").GetComponent<TextMeshProUGUI>();

        basketballRb = basketball.GetComponent<Rigidbody>();
        basketballRb.isKinematic = true;

        //Saves the initial coordinates for the basketball for when the phenomenon is reset.
        initialPosition = basketball.position;

        simulationPaused = true; 
        Instructions.gameObject.SetActive(true); 
    }

    //Handle user input for shooting and resetting
    void Update()
    {
        Instructions.gameObject.SetActive(simulationPaused);

        //Spacebar to shoot
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ShootBall();
        }

        //R key to reset the ball position
        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetBall();
        }
        if (Input.GetKeyDown (KeyCode.H))
        {
            ToggleExplanation();
        }
    }

    //Apply force to shoot the ball in a set direction, with audio feedback
    void ShootBall()
    {
        simulationPaused = false; 

        //Enables physics, and calculates the direction of the goal, then horizontal and vertical forces are applied.
        basketballRb.isKinematic = false;
        Vector3 direction = (goalDetector.position - basketball.position).normalized;
        Vector3 horizontalDirection = new Vector3(direction.x, 0, direction.z);
        Vector3 verticalDirection = Vector3.up;

        basketballRb.AddForce(horizontalDirection * horizontalForce, ForceMode.Impulse);
        basketballRb.AddForce(verticalDirection * verticalForce, ForceMode.Impulse);

        musicSource.clip = throwSound;
        musicSource.Play();
    }

    //If the ball goes through the goal it makes a sound
    private void OnTriggerEnter(Collider other)
    {
        //Play goal sound only if it touches GoalDetector
        if (other.transform == goalDetector)
        {
            musicSource2.clip = Goal;
            musicSource2.Play();
        }
    }

    //Reset the basketball’s position, velocity, and state to its initial values for another shot
    private void ResetBall()
    {
        simulationPaused = true; 

        //Temporarily disable kinematic to clear velocity and spin, reset position, then re-enable
        basketballRb.isKinematic = false;
        basketballRb.velocity = Vector3.zero;
        basketballRb.angularVelocity = Vector3.zero;
        basketball.position = initialPosition;
        basketballRb.isKinematic = true;
    }
    private void ToggleExplanation()
    {
        if (!explanationToggled)
        {
            PhenomenonExplanation.text = "Projectile motion is the motion when a projectile," +
                " in our case, a basketball, " +
                "is thrown into the air. After the initial force of" +
                " throwing the basketball, only the force of gravity affects it. " +
                "Air resistance is a frictional force that slows down the basketball’s " +
                "motion and can significantly affect its path. In our simulation, " +
                "the trajectory of the basketball is moving from the initial position " +
                "on the ground to the basketball hoop’s net. \n\nOur simulation " +
                "only has gravity and colliders with the basketball and the hoop. " +
                "We did not include air resistance as we wanted to focus on the " +
                "scientific phenomenon of projectile motion. When air resistance is " +
                "ignored, horizontal and vertical motions don’t influence one another.\n\n\n" +
                "Press H to show an explanation of the phenomenon";
            Instructions.text = "";
            explanationToggled = true;
        }
        else
        {
            PhenomenonExplanation.text = "";
            Instructions.text = "Press Spacebar to start/shoot ball." +
                "\r\nPress R to reset the simulation.\r\n" +
                "Press G to stop the ball.\r\n\r\nWASD/Arrow Keys to move the ball.\r\n" +
                "Q  to shoot ball up.\r\nE  to shoot ball down.\r\n\r\n" +
                "Click, hold, and move the mouse to rotate the scene\r\nU" +
                "se scroll wheel to zoom in and out.\r\n\r\nPress H to show an explanation of the phenomenon";
            explanationToggled = false;
        }
    }
}

