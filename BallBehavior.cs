using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Events;

public class BallBehavior : MonoBehaviour
{
    public float MassOfBall { get; set; }
    public float RadiusOfBall { get; set; }

    private Rigidbody rigidBody;
    private float _startMass = 1f;
    private float _startRadius = 1f;
    private float _maxMass;
    private bool _isJoiningPossible = true;

    private float _time;
    private float _fluidUpdateSpeed = 10f;

    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();
        MassOfBall = _startMass;
        RadiusOfBall = _startRadius;
    }

    private void OnEnable()
    {
        rigidBody.isKinematic = false;
        UpdateBall(_startMass, _startRadius, false);
    }

    void Start()
    {
        _maxMass = Manager.Instance.maxBallsJoinedThreshold * _startMass;

        Manager.Instance.OnDoneSpawning += ChangeCollisions;
    }

    private void OnDestroy()
    {
        Manager.Instance.OnDoneSpawning -= ChangeCollisions;
    }

    //smoothly updating scale
    private void Update()
    {
        if (_time > 0)
        {
            _time -= Time.deltaTime;
            transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one * RadiusOfBall, Time.deltaTime * _fluidUpdateSpeed);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball") && _isJoiningPossible)
        {
            BallBehavior otherBall = other.GetComponent<BallBehavior>();

            if (this.MassOfBall > otherBall.MassOfBall)
            {
                JoinBalls(otherBall);
            }
            else if (this.MassOfBall < otherBall.MassOfBall)
            {
                DeactivateBall(this);
            }
            else
            {
                if (this.GetInstanceID() > otherBall.GetInstanceID())
                {
                    JoinBalls(otherBall);
                }
                else
                {
                    DeactivateBall(this);
                }
            }
        }
    }

    private void JoinBalls(BallBehavior otherBall)
    {
        if (MassOfBall > _maxMass)
        {
            int numberOfBalls = (int)(MassOfBall / _startMass);
            Manager.Instance.SplitBall(this.transform.position, numberOfBalls);
            DeactivateBall(this);
        }
        else
        {
            float newMass = this.MassOfBall + otherBall.MassOfBall;
            float newRadius = Mathf.Sqrt(Mathf.Pow(this.RadiusOfBall, 2) + Mathf.Pow(otherBall.RadiusOfBall, 2));
            bool isMassEqual = this.MassOfBall == otherBall.MassOfBall;
            UpdateBall(newMass, newRadius, true, isMassEqual);
        }
    }

    private void UpdateBall(float newMass, float newRadius, bool isUpdateFluid, bool isMassEqual = true)
    {
        MassOfBall = newMass;
        rigidBody.mass = newMass;
        //simulating conservation of momentum
        if (isMassEqual)
        {
            rigidBody.velocity = Vector3.zero;
        }
        else
        {
            rigidBody.velocity /= MassOfBall;
        }

        RadiusOfBall = newRadius;
        //smoothly updating scale
        if (isUpdateFluid)
        {
            _time = 2;
        }
        else
        {
            transform.localScale = Vector3.one * newRadius;
        }
    }

    private void DeactivateBall(BallBehavior ballToDeactivate)
    {
        ballToDeactivate.GetComponent<Rigidbody>().isKinematic = true;
        ballToDeactivate.gameObject.SetActive(false);
    }

    public void ChangeCollisions()
    {
        GetComponent<SphereCollider>().isTrigger = false;
        _isJoiningPossible = false;
    }

}
