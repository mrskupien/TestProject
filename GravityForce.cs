using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityForce : MonoBehaviour
{
    const float G = 50f;

    public static List<GravityForce> attractorList;
    public Rigidbody rigidBody;

    private int _forceDirectionChanger = 1;

    private void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        Manager.Instance.OnDoneSpawning += ChangeForceDirection;
    }

    private void OnDestroy()
    {
        Manager.Instance.OnDoneSpawning -= ChangeForceDirection;
    }

    private void OnEnable()
    {
        if (attractorList == null)
        {
            attractorList = new List<GravityForce>();
        }
        attractorList.Add(this);
    }

    private void OnDisable()
    {
        attractorList.Remove(this);
    }

    private void FixedUpdate()
    {
        foreach (var attractor in attractorList)
        {
            if (attractor != this)
            {
                Attract(attractor);
            }
        }
    }

    private void Attract(GravityForce objectToAttract)
    {
        Rigidbody rigidBodyToAttract = objectToAttract.rigidBody;

        Vector3 direction = rigidBody.position - rigidBodyToAttract.position;
        float distance = direction.magnitude;
        float forceMagnitude = G * (rigidBody.mass * rigidBodyToAttract.mass) / Mathf.Pow(distance, 2);
        Vector3 force = direction.normalized * forceMagnitude * _forceDirectionChanger;

        rigidBodyToAttract.AddForce(force);
    }

    private void ChangeForceDirection()
    {
        _forceDirectionChanger = -1;
    }
    
}
