using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakTest : MonoBehaviour
{
    public GameObject brokenItem;
    private Rigidbody rb;
    private Vector3 lastVelocity;
    [SerializeField] private float upForceOnBreak = 2f;
    [SerializeField] private float randomForceVariation = 1f; // Randomness factor for explosion

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        lastVelocity = rb.velocity;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Respawn")) return;

        Break();
    }

    private void Break()
    {
        // Position the broken object correctly
        brokenItem.transform.position = transform.position;
        brokenItem.transform.rotation = transform.rotation;
        brokenItem.SetActive(true);

        Rigidbody[] rigidbodies = brokenItem.GetComponentsInChildren<Rigidbody>();

        foreach (var piece in rigidbodies)
        {
            // Base force using the last velocity
            Vector3 force = lastVelocity;

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

