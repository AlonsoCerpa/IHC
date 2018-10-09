using UnityEngine;
using System.Collections;
using System.Net;
using UnityEngine.Networking;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.UI;
using System;

/* This script has one purpose.
 *	1 - It should be launched on an android phone ( or any other second device)
 *		a)  - the phone creates a socket connection on a port 8888 and it listens for a datastream
 *		b)	- once it receives data it deserializes the data into MyBody structure that contains position and rotation of
 *				left hand, right hand and head.
 *		c)	- you need to assign 3 game objects to the variables  (headRec, leftHandRec, rightHandRec)
 *		d)	- if everything goes as it should the original 3 objects tracked by kinect in the script SocketClient
 *				should replicate the movement into the unity instance that is running on your phone 
 *				(maybe with GearVR or the cardboard)
 *
 * */

/*	Instructions
 * 	1	-	Put this script into a scene
 * 	2	-	Assign 3 objects to the headRec, leftHandRec and rightHandRec - these objects will replicate the movement
 * 			of the 3 objects tracked by kinect
 * 	3	-	Change the socket number if needed
 * 	4	-	ATTENTION!!! i take absolute position, you can do relative and do gameObject.transform.position+tracked position
 * */

public class SocketServer : MonoBehaviour
{

    public GameObject hipCenter, spine, neck, head, leftShoulder, leftElbow, leftWrist, leftHand, rightShoulder, rightElbow, rightWrist,
                      rightHand, leftHip, leftKnee, leftAnkle, leftFoot, rightHip, rightKnee, rightAnkle, rightFoot, spineShoulder, leftTipHand,
                      leftThumb, rightTipHand, rightThumb;
    public int hostId;
    private int myChan;
    private int socketId; // above Start()
    private int socketPort = 8888; // Also a class member variable

    int connectionId;
    // Use this for initialization
    void Start()
    {

        bufferSize = 2048; 
        recBuffer = new byte[bufferSize];
        initServer();
    }

    //create the socket connection so we can send or receive data
    public void initServer()
    {
        NetworkTransport.Init();
        ConnectionConfig config = new ConnectionConfig();
        myChan = config.AddChannel(QosType.UnreliableFragmented);
        HostTopology topology = new HostTopology(config, 10);
        socketId = NetworkTransport.AddHost(topology, socketPort);
        Debug.Log("Socket Open. SocketId is: " + socketId);

    }

    private byte error;

    private BinaryFormatter formatter = new BinaryFormatter();
    private int bufferSize;  

    private int recHostId;
    private int recConnectionId;
    private int recChannelId;
    private int dataSize;
    private NetworkEventType recNetworkEvent;

    private MyBody receivedBody;

    void Update()
    {
        handleSocketConnection();
    }

    private byte[] recBuffer;

    public void handleSocketConnection()
    {
        byte error;
        recNetworkEvent = NetworkTransport.Receive(out recHostId, out recConnectionId, out recChannelId, recBuffer, bufferSize, out dataSize, out error);
        switch (recNetworkEvent)
        {
            case NetworkEventType.Nothing:
                break;
            case NetworkEventType.ConnectEvent:
                Debug.Log("incoming connection event received");
                break;
            case NetworkEventType.DataEvent:
                Stream stream = new MemoryStream(recBuffer);
                BinaryFormatter formatter = new BinaryFormatter();

                receivedBody = (MyBody)formatter.Deserialize(stream);

                handleReceivedData();

                break;
            case NetworkEventType.DisconnectEvent:
                Debug.Log("remote client event disconnected");
                break;
        }

    }

    //here we deal with the data we have received and we assign the coordinance
    //i ignore the rotation of the head because that is bing controlled by cardboard/gearVR
    public void handleReceivedData()
    {
        hipCenter.transform.position = receivedBody.hipCenter;
        hipCenter.transform.rotation = receivedBody.hipCenter;
        spine.transform.position = receivedBody.spine;
        spine.transform.rotation = receivedBody.spine;
        neck.transform.position = receivedBody.neck;
        neck.transform.rotation = receivedBody.neck;
        head.transform.position = receivedBody.head;
        head.transform.rotation = receivedBody.head;

        leftShoulder.transform.position = receivedBody.leftShoulder;
        leftShoulder.transform.rotation = receivedBody.leftShoulder;
        leftElbow.transform.position = receivedBody.leftElbow;
        leftElbow.transform.rotation = receivedBody.leftElbow;
        leftWrist.transform.position = receivedBody.leftWrist;
        leftWrist.transform.rotation = receivedBody.leftWrist;
        leftHand.transform.position = receivedBody.leftHand;
        leftHand.transform.rotation = receivedBody.leftHand;

        rightShoulder.transform.position = receivedBody.rightShoulder;
        rightShoulder.transform.rotation = receivedBody.rightShoulder;
        rightElbow.transform.position = receivedBody.rightElbow;
        rightElbow.transform.rotation = receivedBody.rightElbow;
        rightWrist.transform.position = receivedBody.rightWrist;
        rightWrist.transform.rotation = receivedBody.rightWrist;
        rightHand.transform.position = receivedBody.rightHand;
        rightHand.transform.rotation = receivedBody.rightHand;

        leftHip.transform.position = receivedBody.leftHip;
        leftHip.transform.rotation = receivedBody.leftHip;
        leftKnee.transform.position = receivedBody.leftKnee;
        leftKnee.transform.rotation = receivedBody.leftKnee;
        leftAnkle.transform.position = receivedBody.leftAnkle;
        leftAnkle.transform.rotation = receivedBody.leftAnkle;
        leftFoot.transform.position = receivedBody.leftFoot;
        leftFoot.transform.rotation = receivedBody.leftFoot;

        rightHip.transform.position = receivedBody.rightHip;
        rightHip.transform.rotation = receivedBody.rightHip;
        rightKnee.transform.position = receivedBody.rightKnee;
        rightKnee.transform.rotation = receivedBody.rightKnee;
        rightAnkle.transform.position = receivedBody.rightAnkle;
        rightAnkle.transform.rotation = receivedBody.rightAnkle;
        rightFoot.transform.position = receivedBody.rightFoot;
        rightFoot.transform.rotation = receivedBody.rightFoot;

        spineShoulder.transform.position = receivedBody.spineShoulder;
        spineShoulder.transform.rotation = receivedBody.spineShoulder;

        leftTipHand.transform.position = receivedBody.leftTipHand;
        leftTipHand.transform.rotation = receivedBody.leftTipHand;
        leftThumb.transform.position = receivedBody.leftThumb;
        leftThumb.transform.rotation = receivedBody.leftThumb;

        rightTipHand.transform.position = receivedBody.rightTipHand;
        rightTipHand.transform.rotation = receivedBody.rightTipHand;
        rightThumb.transform.position = receivedBody.rightThumb;
        rightThumb.transform.rotation = receivedBody.rightThumb;
    }
}


[System.Serializable]
public struct MyBody
{
    public MyBodyPart hipCenter, spine, neck, head, leftShoulder, leftElbow, leftWrist, leftHand, rightShoulder, rightElbow, rightWrist,
                      rightHand, leftHip, leftKnee, leftAnkle, leftFoot, rightHip, rightKnee, rightAnkle, rightFoot, spineShoulder, leftTipHand,
                      leftThumb, rightTipHand, rightThumb;
    public MyBody(MyBodyPart p_hipCenter, MyBodyPart p_spine, MyBodyPart p_neck, MyBodyPart p_head, MyBodyPart p_leftShoulder, MyBodyPart p_leftElbow,
                  MyBodyPart p_leftWrist, MyBodyPart p_leftHand, MyBodyPart p_rightShoulder, MyBodyPart p_rightElbow, MyBodyPart p_rightWrist,
                  MyBodyPart p_rightHand, MyBodyPart p_leftHip, MyBodyPart p_leftKnee, MyBodyPart p_leftAnkle, MyBodyPart p_leftFoot,
                  MyBodyPart p_rightHip, MyBodyPart p_rightKnee, MyBodyPart p_rightAnkle, MyBodyPart p_rightFoot, MyBodyPart p_spineShoulder,
                  MyBodyPart p_leftTipHand, MyBodyPart p_leftThumb, MyBodyPart p_rightTipHand, MyBodyPart p_rightThumb)
    {
        hipCenter = p_hipCenter;
        spine = p_spine;
        neck = p_neck;
        head = p_head;
        leftShoulder = p_leftShoulder;
        leftElbow = p_leftElbow;
        leftWrist = p_leftWrist;
        leftHand = p_leftHand;
        rightShoulder = p_rightShoulder;
        rightElbow = p_rightElbow;
        rightWrist = p_rightWrist;
        rightHand = p_rightHand;
        leftHip = p_leftHip;
        leftKnee = p_leftKnee;
        leftAnkle = p_leftAnkle;
        leftFoot = p_leftFoot;
        rightHip = p_rightHip;
        rightKnee = p_rightKnee;
        rightAnkle = p_rightAnkle;
        rightFoot = p_rightFoot;
        spineShoulder = p_spineShoulder;
        leftTipHand = p_leftTipHand;
        leftThumb = p_leftThumb;
        rightTipHand = p_rightTipHand;
        rightThumb = p_rightThumb;
    }

    public void add(MyBody m)
    {
        hipCenter.add(m.hipCenter);
        spine.add(m.spine);
        neck.add(m.neck);
        head.add(m.head);
        leftShoulder.add(m.leftShoulder);
        leftElbow.add(m.leftElbow);
        leftWrist.add(m.leftWrist);
        leftHand.add(m.leftHand);
        rightShoulder.add(m.rightShoulder);
        rightElbow.add(m.rightElbow);
        rightWrist.add(m.rightWrist);
        rightHand.add(m.rightHand);
        leftHip.add(m.leftHip);
        leftKnee.add(m.leftKnee);
        leftAnkle.add(m.leftAnkle);
        leftFoot.add(m.leftFoot);
        rightHip.add(m.rightHip);
        rightKnee.add(m.rightKnee);
        rightAnkle.add(m.rightAnkle);
        rightFoot.add(m.rightFoot);
        spineShoulder.add(m.spineShoulder);
        leftTipHand.add(m.leftTipHand);
        leftThumb.add(m.leftThumb);
        rightTipHand.add(m.rightTipHand);
        rightThumb.add(m.rightThumb);

    }

    public void divide(int i)
    {
        hipCenter.divide(i);
        spine.divide(i);
        neck.divide(i);
        head.divide(i);
        leftShoulder.divide(i);
        leftElbow.divide(i);
        leftWrist.divide(i);
        leftHand.divide(i);
        rightShoulder.divide(i);
        rightElbow.divide(i);
        rightWrist.divide(i);
        rightHand.divide(i);
        leftHip.divide(i);
        leftKnee.divide(i);
        leftAnkle.divide(i);
        leftFoot.divide(i);
        rightHip.divide(i);
        rightKnee.divide(i);
        rightAnkle.divide(i);
        rightFoot.divide(i);
        spineShoulder.divide(i);
        leftTipHand.divide(i);
        leftThumb.divide(i);
        rightTipHand.divide(i);
        rightThumb.divide(i);
    }
}

[System.Serializable]
public struct MyBodyPart
{
    public float x, y, z;
    public float rx, ry, rz, rw;

    public void add(MyBodyPart m)
    {
        x += m.x;
        y += m.y;
        z += m.z;
        rx += m.rx;
        ry += m.ry;
        rz += m.rz;
        rw += m.rw;
    }

    public void divide(int i)
    {
        x = x / i;
        y = y / i;
        z = z / i;
        rx = rx / i;
        ry = ry / i;
        rz = rz / i;
        rw = rw / i;
    }


    public MyBodyPart(float rX, float rY, float rZ, float rrx, float rry, float rrz, float rrw)
    {
        x = rX;
        y = rY;
        z = rZ;
        rx = rrx;
        ry = rry;
        rz = rrz;
        rw = rrw;

    }

    public MyBodyPart(Vector3 rValue, Quaternion rQuat)
    {
        x = rValue.x;
        y = rValue.y;
        z = rValue.z;
        rx = rQuat.x;
        ry = rQuat.y;
        rz = rQuat.z;
        rw = rQuat.w;
    }

    public void populate(Vector3 rValue, Quaternion rQuat)
    {
        x = rValue.x;
        y = rValue.y;
        z = rValue.z;
        rx = rQuat.x;
        ry = rQuat.y;
        rz = rQuat.z;
        rw = rQuat.w;
    }

    public override string ToString()
    {
        return String.Format("[{0}, {1}, {2}:{3}, {4}, {5}]", x, y, z, rx, ry, rz);
    }

    public static implicit operator Quaternion(MyBodyPart rValue)
    {
        return new Quaternion(rValue.rx, rValue.ry, rValue.rz, rValue.rw);
    }

    public static implicit operator Vector3(MyBodyPart rValue)
    {
        return new Vector3(rValue.x, rValue.y, rValue.z);
    }
}
