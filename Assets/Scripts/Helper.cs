using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public static class Helper
{
    public static ParticleSystem SpawnParticlesSystemOnTop(ParticleSystem particleSystem, GameObject gameObject)
    {
        ParticleSystem instantiatedParticleSystem = GameObject.Instantiate(particleSystem, gameObject.transform);
        instantiatedParticleSystem.transform.position = Helper.GetTop(gameObject) + new Vector3(0.0f, 0.5f, 0.0f);
        return (instantiatedParticleSystem);
    }

    public static Vector3 GetTop(GameObject gameObject)
    {
        Bounds bounds = gameObject.GetComponentInChildren<Collider>().bounds;
        return (bounds.center + new Vector3(0.0f, bounds.extents.y, 0.0f));
    }

    public static Vector3 GetCenter(GameObject gameObject)
    {
        Bounds bounds = gameObject.GetComponentInChildren<Collider>().bounds;
        return (bounds.center);
    }

    public static Vector3 GetBottom(GameObject gameObject)
    {
        Bounds bounds = gameObject.GetComponentInChildren<Collider>().bounds;
        return (bounds.center - new Vector3(0.0f, bounds.extents.y, 0.0f));
    }

    public static bool SpaceUp(Vector3 position, float aboveDistance)
    {
        RaycastHit hitInfo;

        int layerMask = Physics.DefaultRaycastLayers & ~(1 << LayerMask.NameToLayer("Player"));

        Vector3 abovePosition = position;
        abovePosition.y += aboveDistance;

        Debug.DrawLine(position, abovePosition, Color.blue, 0.3f);

        if (Physics.Linecast(position, abovePosition, out hitInfo, layerMask))
            return (false);
        return (true);
    }

    public static bool SpaceUpSphere(GameObject gameObject, ColliderProperties properties)
    {
        Vector3 start = Helper.GetCenter(gameObject);
        Vector3 end = start + new Vector3(0, properties.Height - start.y, 0);

        int layerMask = Physics.AllLayers & ~(1 << LayerMask.NameToLayer("Player"));
        layerMask &= ~(Physics.IgnoreRaycastLayer);

        DebugExtension.DebugCapsule(start, end, Color.red, 0.3f);

        if (Physics.CheckCapsule(start, end, 0.3f, layerMask))
            return (false);
        return (true);
    }

    public static Vector3 RandomNavSphere(Vector3 origin, float radius)
    {
        Vector3 randomPoint = Random.insideUnitSphere * radius;

        randomPoint += origin;

        NavMeshHit navMeshHit;

        if (NavMesh.SamplePosition(randomPoint, out navMeshHit, radius, NavMesh.AllAreas))
            return (navMeshHit.position);
        return (origin);
    }

    public static GameObject FindNearestInLayer(Vector3 origin, float radius, int layerMask, bool lineOfSight = true)
    {
        Collider[] hits = Physics.OverlapSphere(origin, radius, layerMask);

        Collider closestHit = null;
        float smallestRange = float.MaxValue;
        RaycastHit hitInfo;

        foreach (Collider hit in hits)
        {
            if (!lineOfSight)
            {
                if (Vector3.Distance(origin, hit.transform.position) < smallestRange)
                {
                    closestHit = hit;
                    smallestRange = Vector3.Distance(origin, hit.transform.position);
                }
            }
            else
            {
                if (!Physics.Linecast(origin, hit.transform.position, out hitInfo, Physics.DefaultRaycastLayers & ~(1 << LayerMask.NameToLayer("Player"))))
                    continue;

                if (hitInfo.collider == hit && hitInfo.distance < smallestRange)
                {
                    closestHit = hit;
                    smallestRange = hitInfo.distance;
                }
            }
        }
        if (closestHit != null)
            return (closestHit.gameObject);
        return (null);
    }
}
