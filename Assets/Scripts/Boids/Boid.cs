﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Boid : MonoBehaviour
{
    public Vector3 Position;
    public Vector3 Velocity;
    public List<Transform> Neighbors;

    public Boid(Vector3 position, Vector3 velocity)
    {
        Position = position;
        Velocity = velocity;
        gameObject.transform.position = position;
        gameObject.transform.rotation = Quaternion.LookRotation(Vector3.Normalize(velocity));
    }

    public void UpdateBoid(Vector3 position, Vector3 velocity)
    {
        Position = position;
        Velocity = velocity;
        gameObject.transform.position = position;
        gameObject.transform.rotation = Quaternion.LookRotation(Vector3.Normalize(velocity));
    }

    public void UpdateNeighbors(List<Transform> boids, float distance)
    {
        var neighbors = new List<Transform>();

        for (var i = 0; i < boids.Count; ++i)
        {
            if (Position != boids[i].position)
            {
                if (Vector3.Distance(boids[i].position, Position) < distance)
                {
                    neighbors.Add(boids[i]);
                }
            }
        }
        Neighbors = neighbors;
    }

    public Vector3 Cohesion(float steps, float weight)
    {
        var nCenter = Vector3.zero;

        if (Neighbors.Count == 0 || steps < 1) return nCenter;

        for (var i = 0; i < Neighbors.Count; ++i)
        {
            var neighbor = Neighbors[i].GetComponent<Boid>();
            if (nCenter == Vector3.zero)
            {
                nCenter = neighbor.Position;
            }
            else
            {
                nCenter = nCenter + neighbor.Position;
            }
        }
        nCenter = nCenter / Neighbors.Count;
        return (nCenter - Position) / steps * weight;
    }

    public Vector3 Separation(float weight)
    {
        var c = Vector3.zero;

        for (var i = 0; i < Neighbors.Count; ++i)
        {
            var neighbor = Neighbors[i].GetComponent<Boid>();
            var distance = Vector3.Distance(Position, neighbor.Position);

            c = c + Vector3.Normalize(Position - neighbor.Position) / Mathf.Pow(distance, 2);
        }
        return c * weight;
    }

    public Vector3 Alignment(float weight)
    {
        Vector3 nVelocity = Vector3.zero;

        if (Neighbors.Count == 0) return nVelocity;

        for (var i = 0; i < Neighbors.Count; ++i)
        {
            var neighbor = Neighbors[i].GetComponent<Boid>();
            nVelocity = nVelocity + neighbor.Velocity;
        }
        if (Neighbors.Count > 1)
        {
            nVelocity = nVelocity / (Neighbors.Count);
        }
        return (nVelocity - Velocity) * weight;
    }

    public Vector3 Seek(Transform target, float weight)
    {
        if (target == null)
            return Velocity;
        if (weight < 0.0001f) return Vector3.zero;

        var desiredVelocity = (target.position - Position) * weight;
        return desiredVelocity - Velocity;
    }

    public Vector3 Socialize(List<Transform> boids, float weight)
    {
        var pc = Vector3.zero;

        if (Neighbors.Count != 0) return pc;

        for (var i = 0; i < boids.Count; ++i)
        {
            var boid = boids[i].GetComponent<Boid>();
            if (Position != boid.Position)
            {
                if (pc == Vector3.zero)
                {
                    pc = boid.Position;
                }
                else
                {
                    pc = pc + boid.Position;
                }
            }
        }
        if (boids.Count > 1)
        {
            pc = pc / (boids.Count - 1);
        }
        return Vector3.Normalize(pc - Position) * weight;
    }

    public Vector3 Arrival(Transform target, float slowingDistance, float maxSpeed)
    {
        if (target == null)
            return Velocity;

        var desiredVelocity = Vector3.zero;
        if (slowingDistance < 0.0001f) return desiredVelocity;

        var targetOffset = target.position - Position;
        var distance = Vector3.Distance(target.position, Position);
        var rampedSpeed = maxSpeed * (distance / slowingDistance);
        var clippedSpeed = Mathf.Min(rampedSpeed, maxSpeed);
        if (distance > 0)
        {
            desiredVelocity = (clippedSpeed / distance) * targetOffset;
        }
        return desiredVelocity - Velocity;
    }


    public Vector3 LimitVelocity(Vector3 v, float limit)
    {
        if (v.magnitude > limit)
        {
            v = v / v.magnitude * limit;
        }
        return v;
    }

    public Vector3 LimitRotation(Vector3 v, float maxAngle, float maxSpeed)
    {
        return Vector3.RotateTowards(Velocity, v, maxAngle * Mathf.Deg2Rad, maxSpeed);
    }

    /////////////Collision detection

    public Vector3 CalculateAvoidance(float boundsRadius, float collisionAvoidDst, LayerMask obstacleMask)
    {
        Vector3 avoidanceMove = Vector3.zero;
        RaycastHit hit;
        if (Physics.SphereCast(transform.position, boundsRadius, transform.forward, out hit, collisionAvoidDst, obstacleMask))
        {
            avoidanceMove = hit.normal;

            return avoidanceMove;
        }
        return avoidanceMove;
    }

}