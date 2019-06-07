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

        // grab the actual points that make up the boundary
        Vector3[] points = boundary.GetGeometry(OVRBoundary.BoundaryType.OuterBoundary);

        // we have to create an oriented bounding box (OBB) instead of an axis-aligned (AABB)

        // to find the boundary, we need extreme values that will always pass the first max or min we try
        Vector3 minX = new Vector3(Mathf.Infinity, 0, 0);
        Vector3 minZ = new Vector3(0, 0, Mathf.Infinity);
        Vector3 maxX = new Vector3(-Mathf.Infinity, 0, 0);
        Vector3 maxZ = new Vector3(0, 0, -Mathf.Infinity);


        // foreach point that makes up the boundary
        foreach (Vector3 point in points)
        {
            // find the smallest x
            if (point.x < minX.x)
            {
                minX = point;
            }

            // find the largest x
            if (point.x > maxX.x)
            {
                maxX = point;
            }

            // find the smallest z
            if (point.z < minZ.z)
            {
                minZ = point;
            }

            // find the largest z
            if (point.z > maxZ.z)
            {
                maxZ = point;
            }
        }
        // the origin of VR space
        Transform origin = FindObjectsOfType<OVRCameraRig>()[0].trackingSpace;

        // zero out the ys
        minX.y = origin.transform.position.y;
        minZ.y = origin.transform.position.y;
        maxX.y = origin.transform.position.y;
        maxZ.y = origin.transform.position.y;

        // return the vectors
        return new Vector3[] { minX, maxX, minZ, maxZ };
    }

    void createSphere(Vector3 location, Color color)
    {
        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.transform.position = location;
        sphere.GetComponent<Renderer>().material.SetColor("_Color", color);
    }
}
