using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class movement_ball : MonoBehaviour {

    public Rigidbody rb;

    public float topForce = 100f;
    public float sidewaysForce = 100f;

    // Update is called once per frame
    void FixedUpdate()
    {
        // Add a forward force

        if (Input.GetKey("w"))
        {
            rb.AddForce(0, topForce * Time.deltaTime, 0, ForceMode.VelocityChange);
        }

        if (Input.GetKey("a"))
        {
            rb.AddForce(-sidewaysForce * Time.deltaTime, 0, 0, ForceMode.VelocityChange);
        }

        if (Input.GetKey("d"))
        {
            rb.AddForce(sidewaysForce * Time.deltaTime, 0, 0, ForceMode.VelocityChange);
        }
    }
}
