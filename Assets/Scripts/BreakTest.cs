using System;
using UnityEngine;
using Oculus.Interaction;
using Unity.Labs.SuperScience;

public class BreakTest : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject brokenItem;
    [SerializeField] private Transform trackObject;
    private Rigidbody rb;

    [Header("Stats")]
    [SerializeField] private float upForceOnBreak = 2f;
    [SerializeField] private float randomForceVariation = 1f;

    [Header("Physics")]
    [SerializeField] private bool isIndestructible;
    [SerializeField] private float neededBreakForce;
    [SerializeField] private float density;
    [SerializeField] private int materialStrength;

    private PhysicsTracker physicsTracker = new PhysicsTracker();

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        physicsTracker.Reset(trackObject.position, trackObject.rotation, Vector3.zero, Vector3.zero);
    }

    void Update()
    {
        physicsTracker.Update(trackObject.position, trackObject.rotation, Time.smoothDeltaTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent(out BreakTest otherBreakTest))
        {
            TryBreak(otherBreakTest);
        }
    }

    private float GetKineticEnergy()
    {
        float speed = physicsTracker.Speed;
        return (speed == 0) ? density : 0.5f * rb.mass * speed * speed; // Fórmula correta de energia cinética
    }

    public int GetMaterialStrength => materialStrength;

    private void TryBreak(BreakTest other)
    {
        Debug.Log(gameObject.name + " is indestructible=" + isIndestructible);
        if (isIndestructible) return;

        float myKE = physicsTracker.Speed;
        //float myKE = GetKineticEnergy();
        float myBreakThreshold = neededBreakForce * density;

        Debug.Log(gameObject.name + " is trying to break itself with KE=" + myKE + " and Threshold:" + myBreakThreshold);

        int otherMS = other.GetMaterialStrength;

        bool iBreak = false;

        if (materialStrength <= otherMS && myKE >= myBreakThreshold)
        {
            iBreak = true;
        }

        if (iBreak) BreakObject();
    }

    private void BreakObject()
    {
        brokenItem.transform.SetPositionAndRotation(transform.position, transform.rotation);
        brokenItem.SetActive(true);

        foreach (var piece in brokenItem.GetComponentsInChildren<Rigidbody>())
        {
            Vector3 force = physicsTracker.Velocity + new Vector3(
                UnityEngine.Random.Range(-randomForceVariation, randomForceVariation),
                UnityEngine.Random.Range(0, upForceOnBreak),
                UnityEngine.Random.Range(-randomForceVariation, randomForceVariation)
            );

            piece.velocity = force;
            piece.angularVelocity = rb.angularVelocity;
        }

        gameObject.SetActive(false);
    }
}
