using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Microsoft.FSharp.Collections;

using static Sumer.Geometry;

public class SpawnWorld : MonoBehaviour
{

    // the only thing we need to do is create the scene when we spawn
    void Start()
    {
        // grab the play boundary defined by the oculus system
        OVRBoundary boundary = OVRManager.boundary;

        // make sure the boundary is always visible
        boundary.SetVisible(true);

        // compute the points that make the bounding box of the play area
        List<Vector3> r = this.boundingBox();

        // create the ground plane

        // grab the origin of VR space
        Vector3 boxDims = boundary.GetDimensions(OVRBoundary.BoundaryType.OuterBoundary);
        GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
        ground.transform.position = FindObjectsOfType<OVRCameraRig>()[0].trackingSpace.transform.position; // make sure the plan is in line with the ground

        // some spheres for good measure
        createSphere(r[0] + new Vector3(0, 1, 0), Color.red);
        createSphere(r[1] + new Vector3(0, 1, 0), Color.blue);
        createSphere(r[2] + new Vector3(0, 1, 0), Color.yellow);
        createSphere(r[3] + new Vector3(0, 1, 0), Color.green);
    }

    List<Vector3> boundingBox()
    {
        // grab the play boundary defined by the oculus system
        OVRBoundary boundary = OVRManager.boundary;
        // the origin of VR space
        Transform origin = FindObjectsOfType<OVRCameraRig>()[0].trackingSpace;

        if (!boundary.GetConfigured())
        {
            throw new System.InvalidOperationException("Boundary is not configured");
        }

        // grab the actual points that make up the boundary
        Vector3[] points = boundary.GetGeometry(OVRBoundary.BoundaryType.OuterBoundary);

        // compute the bounding box that is oriented with the shape (not axis-aligned)
        Rectangle box = OrientedBoundingBox2D(points.Select(point => new Vector2(point.x, point.z)));

        // convert the 4 points that define the extent of the boudning box
        return (new List<Vector2>{
            box.supports.top,
            box.supports.bottom,
            box.supports.left,
            box.supports.right
        // back into 3D vectors
        }).Select(point => new Vector3(point.x, origin.transform.position.y, point.y)).ToList();
    }

    void createSphere(Vector3 location, Color color)
    {
        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.transform.position = location;
        sphere.GetComponent<Renderer>().material.SetColor("_Color", color);
    }
}
