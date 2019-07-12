using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Microsoft.FSharp.Collections;

using static Sumer.Geometry;

public class SpawnWorld : MonoBehaviour
{
    /// the prefab with all of the objects necessary to create the scene
    public GameObject contentPrefab;
    /// the object to use in game for the walls of the generated room
    public GameObject wall;

    // an internal reference to the room game object
    GameObject room;

    // the only thing we need to do is create the scene when we spawn
    void Start()
    {
        // the first thing to do is spawn the prefab content for the room
        GameObject content = Instantiate(contentPrefab, Vector3.zero, Quaternion.identity);
        content.name = "Environment";

        // grab the play boundary defined by the oculus system
        OVRBoundary boundary = OVRManager.boundary;
        // the origin of VR space
        Transform origin = FindObjectsOfType<OVRCameraRig>()[0].trackingSpace.transform;

        /// compute the location of the play area

        // compute the points that make the bounding box of the play area
        Rectangle r = this.boundingBox();

        // convert supports into 3D space
        List<Vector3> corners = (r.Corners).Select(point => new Vector3(
            origin.position.x + point.x,
            origin.position.y,
            origin.position.z + point.y
        )).ToList();

        // the rectangle basis vectors in 3D
        Vector3 u1 = new Vector3(r.BasisVectors.Item1.x, 0, r.BasisVectors.Item1.y);
        Vector3 u2 = new Vector3(r.BasisVectors.Item2.x, 0, r.BasisVectors.Item2.y);

        /// generate the environment

        // make sure the boundary is always visible
        boundary.SetVisible(true);

        // move the light source to the inside of the building
        Light light = FindObjectsOfType<Light>()[0];
        light.transform.position = new Vector3(
            origin.position.x + r.Center.x,
            origin.position.y + 3,
            origin.position.z + r.Center.y
        );

        // a game object to house the entire room
        room = new GameObject();
        room.name = "Room";

        // create the ground plane
        Vector3 boxDims = boundary.GetDimensions(OVRBoundary.BoundaryType.OuterBoundary);
        GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
        ground.transform.position = origin.position; // make sure the plan is in line with the ground
        ground.transform.parent = room.transform;

        // the wall from top left to top right
        createWall(corners[0], corners[1], Color.red).name = "Top Wall";
        // the wall from top right to bottom right
        createWall(corners[1], corners[3], Color.yellow).name = "Right Wall";
        // the wall from top left to bottom left
        createWall(corners[0], corners[2], Color.green).name = "Left Wall";
        // the wall from bottom left to bottom right
        createWall(corners[2], corners[3], Color.blue).name = "Bottom Wall";
    }

    Rectangle boundingBox()
    {
        // grab the play boundary defined by the oculus system
        OVRBoundary boundary = OVRManager.boundary;

        // if we are running in a non-VR environment
        if (!boundary.GetConfigured())
        {
            // return the mock bounding box
            return new Rectangle(
                System.Tuple.Create(Vector2.right, Vector2.up),
                new RectangleVertices(
                    new Vector2(5.0f, -5.0f),
                    new Vector2(-5.0f, -5.0f),
                    new Vector2(5.0f, 5.0f),
                    new Vector2(-5.0f, 5.0f)
                ),
                10.0f,
                10.0f
            );
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

    GameObject createWall(Vector3 p1, Vector3 p2, Color color)
    {
        // instantiate a copy of the wall prefab
        GameObject newWall = Instantiate(wall, p1, Quaternion.identity) as GameObject;
        newWall.transform.parent = room.transform;

        // compute the height of the wall object
        float height = newWall.transform.localScale.y;

        // compute the distance between the two points
        float distance = Vector3.Distance(p1, p2);

        // make the wall face where its going
        newWall.transform.LookAt(p2);
        // put it in the middle
        newWall.transform.position = p1 + distance / 2 * newWall.transform.forward + (height / 2 * Vector3.up);

        Vector3 oldScale = newWall.transform.localScale;
        // stretch the newWall to fill the gap
        newWall.transform.localScale = new Vector3(oldScale.x, oldScale.y, distance);

        // color the newWall
        newWall.GetComponent<Renderer>().material.SetColor("_Color", color);

        // we're done here
        return newWall;
    }
}
