using UnityEngine;
using Meta.WitAi;
using Meta.XR;

public class VRPlayerWalkController : MonoBehaviour
{
    public float walkSpeed = 2f; // Walking speed
    public float turnSpeed = 10f; // Rotation speed

    private CharacterController characterController;
    private Vector3 moveDirection;
    private float gravity = -9.81f;
    private float ySpeed;

    private Transform headTransform; // Reference to the VR headset

    public OVRInput.Axis2D moveInputAxis = OVRInput.Axis2D.PrimaryThumbstick; // Meta Quest movement input

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        headTransform = Camera.main.transform; // Gets the main camera (VR headset position)
    }

    void Update()
    {
        // Read input from the Oculus controller
        Vector2 inputAxis = OVRInput.Get(moveInputAxis);

        // Get head direction and move accordingly
        Vector3 headForward = headTransform.forward;
        headForward.y = 0; // Keep movement on the horizontal plane
        headForward.Normalize();
        Vector3 headRight = headTransform.right;
        headRight.y = 0;
        headRight.Normalize();

        Vector3 move = headForward * inputAxis.y + headRight * inputAxis.x;
        moveDirection = move * walkSpeed;

        // Apply gravity
        ySpeed += gravity * Time.deltaTime;
        moveDirection.y = ySpeed;

        // Move the player
        characterController.Move(moveDirection * Time.deltaTime);
    }
}