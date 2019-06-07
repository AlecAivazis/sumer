using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpawnWorld : MonoBehaviour
{

    // the only thing we need to do is create the scene when we spawn
    void Start()
    {
        // grab the play boundary defined by the oculus system
        OVRBoundary boundary = OVRManager.boundary;

        // make sure the boundary is always visible
        boundary.SetVisible(true);

        // we want to create a room that encompasses the user's defined boundary area

        // compute the 4 points that make the bounding box of the play area
        Vector3[] r = this.boundingBox();

        // create the ground plane
        Vector3 boxDims = boundary.GetDimensions(OVRBoundary.BoundaryType.OuterBoundary);
        GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
        ground.transform.position = r[0]; // make sure the plan is in line with the ground
        ground.transform.localScale = new Vector3(boxDims.x, 0, boxDims.z);

        // some spheres for good measure
        createSphere(r[0] + new Vector3(0, 0, 0), Color.red);
        createSphere(r[1] + new Vector3(0, 1, 0), Color.blue);
        createSphere(r[2] + new Vector3(0, 1, 0), Color.yellow);
        createSphere(r[3] + new Vector3(0, 1, 0), Color.green);
    }

    Vector3[] boundingBox()
    {
        // grab the play boundary defined by the oculus system
        OVRBoundary boundary = OVRManager.boundary;

        if (!boundary.GetConfigured())
        {
            throw new System.InvalidOperationException("Boundary is not configured");
        }

        // grab the actual points that make up the boundary
        Vector3[] points = boundary.GetGeometry(OVRBoundary.BoundaryType.OuterBoundary);

        // compute the convex hull of the points
        Vector3[] convexHull = ConvexHull.JarvisMarch(points);

        // we now have to do the rotating callipers algorithm. Since the minimum area rectangle
        // must be along one of the edges of the convex hull, we just need to go over every one
        // compute the minimum area, and then find the lowest one.


        // we're done here
        return points;
    }

    void createSphere(Vector3 location, Color color)
    {
        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.transform.position = location;
        sphere.GetComponent<Renderer>().material.SetColor("_Color", color);
    }
}
