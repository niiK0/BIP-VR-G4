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
    private bool canPlaySound = true;
    private float internalCw = 0f;
    private float internalCwValue = .2f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        physicsTracker.Reset(trackObject.position, trackObject.rotation, Vector3.zero, Vector3.zero);
    }

    void Update()
    {
        physicsTracker.Update(trackObject.position, trackObject.rotation, Time.smoothDeltaTime);
        if (internalCw > 0)
        {
            internalCw -= Time.deltaTime;
        }
        if (internalCw <= 0f)
        {
            internalCw = internalCwValue;
            canPlaySound = true;
        }
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
        return (speed == 0) ? density : 0.5f * rb.mass * speed * speed;
    }

    public int GetMaterialStrength => materialStrength;

    private void TryBreak(BreakTest other)
    {
        Debug.Log(gameObject.name + " is indestructible=" + isIndestructible);
        if (isIndestructible) return;

        float myKE = physicsTracker.Speed;
        //float myKE = GetKineticEnergy();
        float otherKE = other.physicsTracker.Speed;
        bool isWeapon = other.CompareTag("Weapon");
        float myBreakThreshold = neededBreakForce * density;

        if (isWeapon && materialStrength != 3) myBreakThreshold = neededBreakForce * density * 5f;

        //Debug.Log(gameObject.name + " is trying to break itself with KE=" + myKE + " and Threshold:" + myBreakThreshold);
        Debug.Log(gameObject.name + " KE=" + myKE + " and Threshold:" + myBreakThreshold + " other KE: " + otherKE);

        int otherMS = other.GetMaterialStrength;

        bool iBreak = false;
        
        if (materialStrength <= otherMS && myKE >= myBreakThreshold || materialStrength <= otherMS && otherKE >= myBreakThreshold)
        {
            iBreak = true;
        }

        if (!iBreak && GetComponent<AudioSource>() && isWeapon && canPlaySound)
        {
            GetComponent<AudioSource>().Play();
            canPlaySound = false;
        }

        if (iBreak) BreakObject();
    }

    private void BreakObject()
    {
        brokenItem.transform.SetPositionAndRotation(transform.position, transform.rotation);
        brokenItem.SetActive(true);

        if(brokenItem.GetComponent<AudioSource>()) brokenItem.GetComponent<AudioSource>().Play();

        foreach (var piece in brokenItem.GetComponentsInChildren<Rigidbody>())
        {
            Vector3 force = physicsTracker.Velocity + new Vector3(
                UnityEngine.Random.Range(-randomForceVariation, randomForceVariation),
                UnityEngine.Random.Range(0, upForceOnBreak),
                UnityEngine.Random.Range(-randomForceVariation, randomForceVariation)
            );

            piece.velocity = new Vector3(Mathf.Clamp(force.x, 0f, 20f), Mathf.Clamp(force.y, 0f, 20f), Mathf.Clamp(force.z, 0f, 20f));
            piece.angularVelocity = new Vector3(Mathf.Clamp(rb.angularVelocity.x, 0f, 20f), Mathf.Clamp(rb.angularVelocity.y, 0f, 20f), Mathf.Clamp(rb.angularVelocity.z, 0f, 20f));
            //Destroy( piece, 3f);
        }

        gameObject.SetActive(false);
    }
}
