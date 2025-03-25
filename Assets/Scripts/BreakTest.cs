using System;
using System.Collections;
using System.Collections.Generic;
using Oculus.Interaction;
using Unity.Labs.SuperScience;
using UnityEngine;

public class BreakTest : MonoBehaviour
{
    [Header("References")]
    [SerializeField] GameObject brokenItem;
    [SerializeField] private Transform trackObject;
    private Rigidbody rb;

    [Header("Stats")]
    [SerializeField] private float upForceOnBreak = 2f;
    [SerializeField] private float randomForceVariation = 1f;
    [SerializeField] float speedBreakThreshold;
    [SerializeField] bool isIndestructuble;
    [SerializeField] float neededBreakForce;
    [SerializeField] float density;

    Vector3 lastVelocity;

    private PhysicsTracker physicsTracker = new PhysicsTracker();
    public float newtons;
    Vector3 lastPosition;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        var position = trackObject.position;
        physicsTracker.Reset(position, trackObject.rotation, Vector3.zero, Vector3.zero);
        lastPosition = position;
    }

    void Update()
    {
        var position = trackObject.position;
        physicsTracker.Update(position, trackObject.rotation, Time.smoothDeltaTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        //if (!collision.gameObject.CompareTag("Respawn")) return;
        Break(collision.gameObject);
    }

    public float GetNewtons()
    {
        return physicsTracker.Speed * rb.mass;
    }

    public float GetDensity()
    {
        return density;
    }

    public void Break(GameObject collision)
    {
        var newtons = GetNewtons();
        var colNewtons = collision.GetComponent<BreakTest>().GetNewtons();

        if (isIndestructuble) return;
        //if (physicsTracker.Speed < speedBreakThreshold) return;
        if (newtons > colNewtons) return;

        // Position the broken object correctly
        brokenItem.transform.position = transform.position;
        brokenItem.transform.rotation = transform.rotation;
        brokenItem.SetActive(true);


        Rigidbody[] rigidbodies = brokenItem.GetComponentsInChildren<Rigidbody>();

        foreach (var piece in rigidbodies)
        {
            // Base force using the last velocity
            Vector3 force = physicsTracker.Velocity;

            // Add some random variation to the force
            force += new Vector3(
                UnityEngine.Random.Range(-randomForceVariation, randomForceVariation),
                UnityEngine.Random.Range(0, upForceOnBreak), // Ensure force goes upwards a bit
                UnityEngine.Random.Range(-randomForceVariation, randomForceVariation)
            );

            piece.velocity = force;  // Directly set velocity for instant effect
            piece.angularVelocity = rb.angularVelocity; // Transfer rotation momentum
        }

        // Disable the original object
        gameObject.SetActive(false);
    }



}
