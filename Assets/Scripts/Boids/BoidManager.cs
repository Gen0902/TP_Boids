using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class BoidManager : MonoBehaviour
{
    private List<Transform> _boids = new List<Transform>();

    [Header("Prefab")]
    public Transform Prefab;

    [Header("Boids")]
    public Transform boidParent;
    public int NumberOfBoids = 10;

    private Transform target;
    private Transform dangerSource;
    private EBoidState state = EBoidState.Idle;

    #region settings
    [Range(0.1f, 2)]
    public float NeighborDistance = 1;

    [Range(0.01f, 0.1f)]
    public float MaxVelocty = 1;

    [Range(0.1f, 5)]
    public float MaxRotationAngle = 3;

    [Header("Cohesion")]
    [Range(0.1f, 100)]
    public float CohesionWeight = 2;

    [Header("Separation")]
    [Range(0.1f, 10)]
    public float SeparationWeight = 0.1f;

    [Header("Alignment")]
    [Range(0.1f, 100)]
    public float AlignmentWeight = 0.1f;

    [Header("Seek")]
    [Range(0f, 10)]
    public float SeekWeight = 0.1f;

    [Header("Flee")]
    [Range(0f, 2)]
    public float FleeWeight = 0.1f;

    [Header("Socialize")]
    [Range(0f, 10)]
    public float SocializeWeight = 0;

    [Header("Arrival")]
    [Range(0.1f, 5)]
    public float ArrivalSlowingDistance = 2;
    public float ArrivalDistance = 0.1f;

    [Range(0.1f, 5)]
    public float ArrivalMaxSpeed = 0.1f;

    [Header("Bounds")]
    public BoxCollider bounds;

    #endregion

    private void Start()
    {
        for (var i = 0; i < NumberOfBoids; ++i)
        {
            Vector3 position = new Vector3(Random.Range(-0.2f, 0.2f), Random.Range(-0.2f, 0.2f), Random.Range(-0.2f, 0.2f));
            Transform transform = Instantiate(Prefab, boidParent.position + position, Quaternion.identity, boidParent);

            Vector3 velocity = new Vector3(Random.Range(-MaxVelocty, MaxVelocty), Random.Range(-MaxVelocty, MaxVelocty), Random.Range(-MaxVelocty, MaxVelocty));

            transform.GetComponent<Boid>().UpdateBoid(transform.position, velocity);
            _boids.Add(transform);
        }

        for (var i = 0; i < _boids.Count; ++i)
        {
            var boid = _boids[i].GetComponent<Boid>();
            boid.UpdateNeighbors(_boids, NeighborDistance);
        }
    }

    private void FixedUpdate()
    {
        UpdateBoids();

    }

    private void UpdateBoids()
    {
        float separation = 0, alignement = 0, seek = 0, flee = 0, maxVelocity = 0;
        switch (state)
        {
            case EBoidState.Idle:
                separation = this.SeparationWeight;
                alignement = this.AlignmentWeight;
                seek = this.SeekWeight;
                flee = 0;
                maxVelocity = this.MaxVelocty;
                break;
            case EBoidState.Feed:
                separation = 0.1f;
                alignement = 0.1f;
                seek = 3;
                flee = 0;
                maxVelocity = this.MaxVelocty * 1.2f;
                break;
            case EBoidState.Flee:
                separation = this.SeparationWeight;
                alignement = this.AlignmentWeight;
                seek = this.SeekWeight;
                flee = this.FleeWeight;
                maxVelocity = this.MaxVelocty * 1.4f;
                break;
            default:

                break;
        }

        for (var i = 0; i < _boids.Count; ++i)
        {
            var boid = _boids[i].GetComponent<Boid>();
            boid.UpdateNeighbors(_boids, NeighborDistance);
            if (!CheckPosition(boid))
                boid.targetPos = CreateRandomPosition();

            var cohesionVector = boid.Cohesion(CohesionWeight);
            var separationVector = boid.Separation(separation);
            var alignmentVector = boid.Alignment(alignement);
            var fleeVector = boid.Flee(dangerSource, flee);

            var seekVector = boid.Seek(target, seek);
            var socializeVector = boid.Socialize(_boids, SocializeWeight);
            var arrivalVector = boid.Arrival(ArrivalDistance, ArrivalSlowingDistance, ArrivalMaxSpeed);
            var velocity = boid.Velocity + cohesionVector + separationVector + alignmentVector + seekVector + socializeVector + arrivalVector + fleeVector;

            velocity = boid.LimitVelocity(velocity, maxVelocity);
            velocity = boid.LimitRotation(velocity, MaxRotationAngle, maxVelocity);
            var position = boid.Position + velocity;
            boid.UpdateBoid(position, velocity);
        }
    }

    public void UpdateInput(Transform foodTarget, Transform dangerSource)
    {
        if (dangerSource != null)
        {
            this.dangerSource = dangerSource;
            SetState(EBoidState.Flee);
        }
        else if (foodTarget != null)
        {
            this.target = foodTarget;
            SetState(EBoidState.Feed);
        }
        else
        {
            this.dangerSource = null;
            target = null;
            SetState(EBoidState.Idle);
        }
    }

    private void SetState(EBoidState newState)
    {
        state = newState;
    }

    private bool CheckPosition(Boid boid)
    {
        return bounds.bounds.Contains(boid.Position);
    }

    private Vector3 CreateRandomPosition()
    {
        return transform.position + new Vector3(Random.Range(-bounds.size.x / 2, bounds.size.x / 2),
                                        Random.Range(-bounds.size.y / 2, bounds.size.y / 2),
                                        Random.Range(-bounds.size.z / 2, bounds.size.z / 2));
    }

}