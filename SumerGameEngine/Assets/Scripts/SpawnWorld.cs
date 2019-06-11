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
        // the origin of VR space
        Transform origin = FindObjectsOfType<OVRCameraRig>()[0].trackingSpace.transform;

        /// compute the location of the play area

        // compute the points that make the bounding box of the play area
        Rectangle r = this.boundingBox();

        // convert supports into 3D space
        List<Vector3> supports = (new List<Vector2>{
            r.supports.top,
            r.supports.right,
            r.supports.bottom,
            r.supports.left,
        // back into 3D vectors
        }).Select(point => new Vector3(point.x, origin.position.y, point.y)).ToList();

        // the rectangle basis vectors in 3D
        Vector3 u1 = new Vector3(r.basisVectors.Item1.x, 0, r.basisVectors.Item1.y);
        Vector3 u2 = new Vector3(r.basisVectors.Item2.x, 0, r.basisVectors.Item2.y);
        // the corners of the room should be slightly out from the boundary
        Vector3 leftAnchor = supports[3] - u1;
        Vector3 rightAnchor = supports[1] + u1;
        Vector3 topAnchor = supports[0] + u2;
        Vector3 bottomAnchor = supports[2] - u2;

        /// generate the environment

        // make sure the boundary is always visible
        boundary.SetVisible(true);

        // create the ground plane
        Vector3 boxDims = boundary.GetDimensions(OVRBoundary.BoundaryType.OuterBoundary);
        GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
        ground.transform.position = FindObjectsOfType<OVRCameraRig>()[0].trackingSpace.transform.position; // make sure the plan is in line with the ground

        // some spheres to highlight the supports
        createSphere(topAnchor + Vector3.up, Color.red);
        createSphere(rightAnchor + Vector3.up, Color.blue);
        createSphere(bottomAnchor + Vector3.up, Color.yellow);
        createSphere(leftAnchor + Vector3.up, Color.green);

        // we need to compute the 4 corners of the rectangle which are defined
        // as the intersection points between the support lines
    }

    Rectangle boundingBox()
    {
        // grab the play boundary defined by the oculus system
        OVRBoundary boundary = OVRManager.boundary;

        if (!boundary.GetConfigured())
        {
            throw new System.InvalidOperationException("Boundary is not configured");
        }

        // grab the actual points that make up the boundary
        Vector3[] points = boundary.GetGeometry(OVRBoundary.BoundaryType.OuterBoundary);

        // compute the oriented bounding box in the ground plane of the play area
        return OrientedBoundingBox2D(points.Select(point => new Vector2(point.x, point.z)));
    }

    void createSphere(Vector3 location, Color color)
    {
        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.transform.position = location;
        sphere.GetComponent<Renderer>().material.SetColor("_Color", color);
    }
}
