using UnityEngine;
using System.Collections;
using System.Net;
using UnityEngine.Networking;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.UI;
using System;

public class SocketClient : MonoBehaviour
{

    /* This script has two purposes.
 * 1 - It should be launched on a pc that has a kinect.
 * 		a) 	- you need to assign 3 objects to the skeleton that will be tracked ( assigne the objects to the bones)
 * 			- for now I am tracking left hand, right hand and head
 *  	b) 	- the positions and rotations of the above objects are stored in a struct MyBody declared below
 * 		c) 	- MyBody structure has individual objects that can represent body parts ( another structure called MyBodyPart
 *		d)	- If we click connect and insert a correct IP, this instance will connect to another unity instance that runs
 *				on the phone.(you need to assign inputfield to the public variable below or hardcode the value
 *			- The connection is done via wifi/socket 8888 defined below ( feel free to change it )
 *		e)	- once we click connect, each X seconds a data is being send to the receiver.The data represents the locations/rotations
 *				of the tracked objects.
 *			- X is defined by the public variable sendInterval ( if the numbe is too low then every update the message
 *				will be send to the receiver and it will overload the receiver and create a big delay
 *			- To send the update 20 per second set the variable to 0.05, 60 times per second 0.033 atd
 */

    /*	Instructions
 * 	1	-	Put this script into a scene where is your kinect skeleton
 * 	2	-	Create 3 game objects and assign any three bones as a parent ( i have chosen head, left and right hand )
 *  3 	-	Assigne the 3 game objects to the below public variables head, leftHand rightHand
 * 	4	-	Create a input field that will contain the IP address and that will populate the object "join" in the method Connect
 *  5	-	Create a button that will call the method Connect in this script
 */

    public GameObject hipCenter, spine, neck, head, leftShoulder, leftElbow, leftWrist, leftHand, rightShoulder, rightElbow, rightWrist,
                      rightHand, leftHip, leftKnee, leftAnkle, leftFoot, rightHip, rightKnee, rightAnkle, rightFoot, spineShoulder, leftTipHand,
                      leftThumb, rightTipHand, rightThumb;
    public string serverIp;
    public int hostId;
    private int myChan;
    private int socketId; // above Start()
    private int socketPort = 8888; // Also a class member variable

    //public Text result; 
    public InputField inputField;

    public float sendInterval; //recommended value 0.033 ( 60 Frames per second)
    private float intervalCounter;
    private int countsBetweenSending = 0;

    private byte[] buffer;
    private byte[] recBuffer;
    private int bufferSize;

    int connectionId;
    // Use this for initialization
    void Start()
    {

        bufferSize = 2048;  // System.Runtime.InteropServices.Marshal.SizeOf(typeof(MyBody)); 
        buffer = new byte[bufferSize];
        recBuffer = new byte[bufferSize];
        initServer();
        myBody = new MyBody(
                    new MyBodyPart(Vector3.one, Quaternion.identity),
                    new MyBodyPart(Vector3.one, Quaternion.identity),
                    new MyBodyPart(Vector3.one, Quaternion.identity),
                    new MyBodyPart(Vector3.one, Quaternion.identity),
                    new MyBodyPart(Vector3.one, Quaternion.identity),
                    new MyBodyPart(Vector3.one, Quaternion.identity),
                    new MyBodyPart(Vector3.one, Quaternion.identity),
                    new MyBodyPart(Vector3.one, Quaternion.identity),
                    new MyBodyPart(Vector3.one, Quaternion.identity),
                    new MyBodyPart(Vector3.one, Quaternion.identity),
                    new MyBodyPart(Vector3.one, Quaternion.identity),
                    new MyBodyPart(Vector3.one, Quaternion.identity),
                    new MyBodyPart(Vector3.one, Quaternion.identity),
                    new MyBodyPart(Vector3.one, Quaternion.identity),
                    new MyBodyPart(Vector3.one, Quaternion.identity),
                    new MyBodyPart(Vector3.one, Quaternion.identity),
                    new MyBodyPart(Vector3.one, Quaternion.identity),
                    new MyBodyPart(Vector3.one, Quaternion.identity),
                    new MyBodyPart(Vector3.one, Quaternion.identity),
                    new MyBodyPart(Vector3.one, Quaternion.identity),
                    new MyBodyPart(Vector3.one, Quaternion.identity),
                    new MyBodyPart(Vector3.one, Quaternion.identity),
                    new MyBodyPart(Vector3.one, Quaternion.identity),
                    new MyBodyPart(Vector3.one, Quaternion.identity),
                    new MyBodyPart(Vector3.one, Quaternion.identity)
                    );
        Connect();
    }

    /* Once the button connect is pressed on the PC
	 * a connection is created between this PC that has kinect and the other device
	 * if you hardcode the IP adresse in the variable "join" you don't need the input field
	 */
    public void Connect()
    {
        string join;
        //join = "192.168.0.102";
        join = serverIp;

        byte error;
        connectionId = NetworkTransport.Connect(socketId, join, socketPort, 0, out error);
        Debug.Log("Connected to server. ConnectionId: " + connectionId);
        sendingData = true;
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
    private bool sendingData = false;

    //call this message to send the data
    public void SendSocketMessage()
    {

        //buffer = new byte[bufferSize];
        Stream stream = new MemoryStream(buffer);

        //here we calculate the data
        populateMyBody();

        formatter.Serialize(stream, myBody);
        NetworkTransport.Send(hostId, connectionId, myChan, buffer, bufferSize, out error);
    }

    //pc with kinect populates this varible that will be sent over the network
    public MyBody myBody;

    /*	If you want to expand the content of myBody you need to also update the method below
	 *  to populate the structure that will be sent
	 * */
    public void populateMyBody()
    {
        myBody.hipCenter.populate(hipCenter.transform.position, hipCenter.transform.rotation);
        myBody.spine.populate(spine.transform.position, spine.transform.rotation);
        myBody.neck.populate(neck.transform.position, neck.transform.rotation);
        myBody.head.populate(head.transform.position, head.transform.rotation);

        myBody.leftShoulder.populate(leftShoulder.transform.position, leftShoulder.transform.rotation);
        myBody.leftElbow.populate(leftElbow.transform.position, leftElbow.transform.rotation);
        myBody.leftWrist.populate(leftWrist.transform.position, leftWrist.transform.rotation);
        myBody.leftHand.populate(leftHand.transform.position, leftHand.transform.rotation);

        myBody.rightShoulder.populate(rightShoulder.transform.position, rightShoulder.transform.rotation);
        myBody.rightElbow.populate(rightElbow.transform.position, rightElbow.transform.rotation);
        myBody.rightWrist.populate(rightWrist.transform.position, rightWrist.transform.rotation);
        myBody.rightHand.populate(rightHand.transform.position, rightHand.transform.rotation);

        myBody.leftHip.populate(leftHip.transform.position, leftHip.transform.rotation);
        myBody.leftKnee.populate(leftKnee.transform.position, leftKnee.transform.rotation);
        myBody.leftAnkle.populate(leftAnkle.transform.position, leftAnkle.transform.rotation);
        myBody.leftFoot.populate(leftFoot.transform.position, leftFoot.transform.rotation);

        myBody.rightHip.populate(rightHip.transform.position, rightHip.transform.rotation);
        myBody.rightKnee.populate(rightKnee.transform.position, rightKnee.transform.rotation);
        myBody.rightAnkle.populate(rightAnkle.transform.position, rightAnkle.transform.rotation);
        myBody.rightFoot.populate(rightFoot.transform.position, rightFoot.transform.rotation);

        myBody.spineShoulder.populate(spineShoulder.transform.position, spineShoulder.transform.rotation);

        myBody.leftTipHand.populate(leftTipHand.transform.position, leftTipHand.transform.rotation);
        myBody.leftThumb.populate(leftThumb.transform.position, leftThumb.transform.rotation);

        myBody.rightTipHand.populate(rightTipHand.transform.position, rightTipHand.transform.rotation);
        myBody.rightThumb.populate(rightThumb.transform.position, rightThumb.transform.rotation);
    }



    private int recHostId;
    private int recConnectionId;
    private int recChannelId;
    private int dataSize;
    private NetworkEventType recNetworkEvent;



    void Update()
    {

        if (sendingData)
        {
            intervalCounter = intervalCounter + Time.deltaTime;
            /* attempt for smoothing */
            if (intervalCounter < sendInterval)
            {
                MyBody temp = myBody;
                populateMyBody();
                temp.add(myBody);
                myBody = temp;
                countsBetweenSending++;
            }

            /*attempt for smoothing
			*/

            //data is being sent only when certain time has passed because if we send too much data(every frame)
            //it will overload the receiver and create a delay
            if (intervalCounter > sendInterval)
            {
                print(countsBetweenSending);
                myBody.divide(countsBetweenSending);
                countsBetweenSending = 0;
                if (connectionId > 0)
                    SendSocketMessage();
                intervalCounter = 0;
            }
        }
        handleSocketConnection();
    }

    //this method deals with the socket connection
    //connection, deconnecion, recieving of data
    public void handleSocketConnection()
    {

        //byte[] recBuffer = new byte[bufferSize];
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
                Debug.Log("we should not receive any data");
                break;
            case NetworkEventType.DisconnectEvent:
                Debug.Log("remote client event disconnected");
                //Connect();
                break;
        }

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