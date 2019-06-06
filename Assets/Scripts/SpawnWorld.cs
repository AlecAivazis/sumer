using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnWorld : MonoBehaviour {

    // the only thing we need to do is create the scene when we spawn
    void Start() {
        // grab the play boundary defined by the oculus system 
        OVRBoundary boundary = OVRManager.boundary;

        // all of the coordinates we get from the VR system is relative to this point
        Transform origin = FindObjectsOfType<OVRCameraRig>()[0].trackingSpace;

	    // we need extreme values that will always pass the first max or min we try
        Vector3 
		    minX = new Vector3(Mathf.Infinity, 0, 0), 
		    minZ = new Vector3(0, 0, Mathf.Infinity),
		    maxX = new Vector3(-Mathf.Infinity, 0,0 ),
		    maxZ = new Vector3(0, 0, -Mathf.Infinity);

	    // get the list of points that make up the boundary
        Vector3[] points = boundary.GetGeometry(OVRBoundary.BoundaryType.OuterBoundary);

        // foreach point that makes up the boundary
	    foreach (Vector3 point in points) {
	    	// find the smallest x
	    	if (point.x < minX.x) {
	    		minX = point;
	    	} 
	    	
	    	// find the largest x
	    	if (point.x > maxX.x) {
	    		maxX = point;
	    	}
	    	
	    	// find the smallest z
	    	if (point.z < minZ.z) {
	    		minZ = point;
	    	} 
	    	
	    	// find the largest z
	    	if (point.z > maxZ.z) {
	    		maxZ = point;
	    	}
	    }
	    
	    // zero out the ys
	    minX.y = origin.position.y;
	    minZ.y = origin.position.y;
	    maxX.y = origin.position.y;
	    maxZ.y = origin.position.y;
	    
	    // create the ground plane
	    GameObject ground  = GameObject.CreatePrimitive(PrimitiveType.Plane);
	    ground.transform.position = origin.position;
        ground.transform.rotation = origin.rotation;

        // some spheres for good measure
        createSphere(minX + new Vector3(0, 1, 0), Color.red);
        createSphere(minZ + new Vector3(0, 1, 0), Color.blue);
        createSphere(maxX + new Vector3(0, 1, 0), Color.yellow);
        createSphere(maxZ + new Vector3(0, 1, 0), Color.green);
    }
    
	void createSphere(Vector3 location, Color color) {
		GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
		sphere.transform.position = location;
		sphere.GetComponent<Renderer>().material.color = color;

	}
}


