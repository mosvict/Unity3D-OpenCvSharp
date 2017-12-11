using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mouseDrop : MonoBehaviour {

	//private Vector3 screenPoint;
	//public GameObject[] GraphicModels = new GameObject[5];
	private Vector3 collisionOffset = Vector3.zero;
    private bool bCollision = false;
    // Use this for initialization

    void OnMouseDrag()
	{
        Vector3 screenPoint = Camera.main.WorldToScreenPoint(gameObject.transform.position);
        Vector3 offsetPoint = Camera.main.WorldToScreenPoint(collisionOffset);
        Vector3 mousePosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);

        // Vector3 delta = new Vector3(Input.mousePosition.x, Input.mousePosition.y, transform.position.z);
        float dest = Vector3.Distance(mousePosition, offsetPoint);
        float dest2 = Vector3.Distance(screenPoint, offsetPoint);

        if ((bCollision && (dest > dest2)) || bCollision == false)
        {                        
            Vector3 objPosition = Camera.main.ScreenToWorldPoint(mousePosition);                        
            transform.position = objPosition;
        }
    }

    void OnCollisionEnter(Collision col) {
    
        string hitName = col.transform.name;
		this.GetComponent<Rigidbody> ().velocity = Vector3.zero;

		Debug.Log("Collider:"+hitName);
        bCollision = true;
        collisionOffset = col.transform.position;        
	}

    void OnCollisionStay(Collision col)
    {    
        bCollision = true;
        collisionOffset = col.transform.position;
	}

    void OnCollisionExit(Collision col)
    {
        bCollision = false;        
    }

}
