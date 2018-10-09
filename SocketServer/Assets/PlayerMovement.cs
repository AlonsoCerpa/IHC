using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

    private Vector2 joystick;
    private Vector3 newPosition;
    private float speed = 1;

    public GameObject centerEye;
    public GameObject pObject;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        joystick = OVRInput.Get(OVRInput.Axis2D.PrimaryTouchpad, OVRInput.Controller.RTrackedRemote);

        transform.eulerAngles = new Vector3(0, centerEye.transform.localEulerAngles.y, 0);
        transform.Translate(Vector3.forward * speed * joystick.y * Time.deltaTime);
        transform.Translate(Vector3.right * speed * joystick.x * Time.deltaTime);

        pObject.transform.position = Vector3.Lerp(pObject.transform.position, transform.position, 10f * Time.deltaTime);
    }
}
