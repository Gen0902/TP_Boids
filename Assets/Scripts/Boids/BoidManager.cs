using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class BoidManager : MonoBehaviour
{
    private List<Transform> _boids = new List<Transform>();

    [Header("Geometry")]
    public Transform Prefab;
    public Transform Target;

    [Header("Boids")]
    public Transform boidParent;
    public int NumberOfBoids = 10;

    #region settings
    [Range(0.1f, 2)]
    public float NeighborDistance = 1;

    [Range(0.01f, 0.1f)]
    public float MaxVelocty = 1;

    [Range(0.1f, 5)]
    public float MaxRotationAngle = 3;

    [Header("Cohesion")]
    [Tooltip("Arbitary text message")]
    [Range(0.1f, 100)]
    public float CohesionStep = 100;

    [Range(0.1f, 100)] public float CohesionWeight = 2;
    [Header("Separation")]
    [Range(0.1f, 10)]

    public float SeparationWeight = 0.1f;
    [Header("Alignment")]
    [Range(0.1f, 1)]

    public float AlignmentWeight = 0.1f;
    [Header("Seek")]
    [Range(0f, 1)]

    public float SeekWeight = 0.1f;
    [Header("Socialize")]
    [Range(0f, 1)]

    public float SocializeWeight = 0;
    [Header("Arrival")]
    [Range(0.1f, 5)]

    public float ArrivalSlowingDistance = 2;

    [Range(0.1f, 5)]
    public float ArrivalMaxSpeed = 0.1f;

    [Header("Avoidance")]
    public LayerMask obstacleMask;
    public float boundsRadius = 0.1f;
    public float collisionAvoidDistance = 0.1f;
    public float avoidCollisionWeight = 0.1f;

    #endregion

    private void Start()
    {
        for (var i = 0; i < NumberOfBoids; ++i)
        {
            Vector3 position = new Vector3(Random.Range(-0.2f, 0.2f), Random.Range(-0.2f, 0.2f), Random.Range(-0.2f, 0.2f));
            Transform transform = Instantiate(Prefab, position, Quaternion.identity, boidParent);

            Vector3 velocity = new Vector3(Random.Range(-MaxVelocty, MaxVelocty), Random.Range(-MaxVelocty, MaxVelocty), Random.Range(-MaxVelocty, MaxVelocty));

            transform.GetComponent<Boid>().UpdateBoid(position, velocity);
            _boids.Add(transform);
        }

        for (var i = 0; i < _boids.Count; ++i)
        {
            var boid = _boids[i].GetComponent<Boid>();
            boid.UpdateNeighbors(_boids, NeighborDistance);
        }
    }

    private void Update()
    {
        UpdateBoids();

    }

    private void UpdateBoids()
    {
        for (var i = 0; i < _boids.Count; ++i)
        {
            var boid = _boids[i].GetComponent<Boid>();
            boid.UpdateNeighbors(_boids, NeighborDistance);
            var cohesionVector = boid.Cohesion(CohesionStep, CohesionWeight);
            var separationVector = boid.Separation(SeparationWeight);
            var alignmentVector = boid.Alignment(AlignmentWeight);
            var seekVector = boid.Seek(Target, SeekWeight);
            var socializeVector = boid.Socialize(_boids, SocializeWeight);
            var arrivalVector = boid.Arrival(Target, ArrivalSlowingDistance, ArrivalMaxSpeed);
            var avoidanceVector = boid.CalculateAvoidance(boundsRadius, collisionAvoidDistance, obstacleMask) * avoidCollisionWeight;
            var velocity = boid.Velocity + cohesionVector + separationVector + alignmentVector + seekVector + socializeVector + arrivalVector + avoidanceVector;

            velocity = boid.LimitVelocity(velocity, MaxVelocty);
            velocity = boid.LimitRotation(velocity, MaxRotationAngle, MaxVelocty);
            var position = boid.Position + velocity;
            boid.UpdateBoid(position, velocity);
        }
    }


}