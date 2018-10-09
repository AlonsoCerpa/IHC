using UnityEngine;
using System.Collections;
using VRStandardAssets.Utils;

public class InputEvents : MonoBehaviour
{
    [SerializeField] private VRInteractiveItem m_VRInteractiveItem;

    public Rigidbody rb;
    private bool triggerPressed = false;
    Vector3 localStartPoint, worldStartPoint;
    [SerializeField] private Transform m_TrackingSpace = null;
    Matrix4x4 localToWorld;
    Vector3 forceDirection;
    bool arrive;
    Vector3 localEndPoint;
    Vector3 worldEndPoint = Vector3.zero;
    Quaternion orientation;
    bool charged = false;


    public OVRInput.Controller Controller
    {
        get
        {
            OVRInput.Controller controller = OVRInput.GetConnectedControllers();
            if ((controller & OVRInput.Controller.LTrackedRemote) == OVRInput.Controller.LTrackedRemote)
            {
                return OVRInput.Controller.LTrackedRemote;
            }
            else if ((controller & OVRInput.Controller.RTrackedRemote) == OVRInput.Controller.RTrackedRemote)
            {
                return OVRInput.Controller.RTrackedRemote;
            }
            return OVRInput.GetActiveController();
        }
    }

    void Update()
    {
        if (OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger))
        {
            triggerPressed = false;
            if (charged == true)
            {
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;

                localToWorld = m_TrackingSpace.localToWorldMatrix;
                localStartPoint = OVRInput.GetLocalControllerPosition(Controller);
                worldStartPoint = localToWorld.MultiplyPoint(localStartPoint);

                orientation = OVRInput.GetLocalControllerRotation(Controller);
                localEndPoint = localStartPoint + (orientation * Vector3.forward);
                worldEndPoint = localToWorld.MultiplyPoint(localEndPoint);

                forceDirection = worldEndPoint - worldStartPoint;
                rb.AddForce(forceDirection.normalized * 40000 * Time.deltaTime);

                charged = false;
            }
        }

        if (triggerPressed == true)
        {
            //rb.AddForce(Vector3.forward);
            
            localToWorld = m_TrackingSpace.localToWorldMatrix;
            localStartPoint = OVRInput.GetLocalControllerPosition(Controller);
            worldStartPoint = localToWorld.MultiplyPoint(localStartPoint);

            forceDirection = worldStartPoint - transform.position;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.AddForce(forceDirection.normalized * 8000 * Vector3.Distance(worldStartPoint, transform.position) * Time.deltaTime);

            charged = true;
        }
        
        

        /*
        if (triggerPressed == true)
        {
            localToWorld = m_TrackingSpace.localToWorldMatrix;
            localStartPoint = OVRInput.GetLocalControllerPosition(Controller);
            worldStartPoint = localToWorld.MultiplyPoint(localStartPoint);

            if (arrive == false)
            {
                if (transform.position == worldStartPoint)
                {
                    rb.velocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                    arrive = true;
                }
                else
                {
                    forceDirection = worldStartPoint - transform.position;
                    rb.AddForce(forceDirection.normalized * 1000 * Vector3.Distance(worldStartPoint, transform.position) * Time.deltaTime);
                }
            }
            else
            {
                transform.position = worldStartPoint;
            }
            
        }
        else
        {
            arrive = false;
        }*/
    }

    void OnEnable()
    {
        //m_VRInteractiveItem.OnOver += Teleport;
        //m_VRInteractiveItem.OnClick += Teleport2;
        m_VRInteractiveItem.OnDown += TriggerPressed;
        m_VRInteractiveItem.OnUp += TriggerReleased;
    }


    void OnDisable()
    {
        //m_VRInteractiveItem.OnOver -= Teleport;
        //m_VRInteractiveItem.OnClick -= Teleport2;
        m_VRInteractiveItem.OnDown -= TriggerPressed;
        m_VRInteractiveItem.OnUp -= TriggerReleased;
    }

    void TriggerPressed()
    {
        triggerPressed = true;
    }

    void TriggerReleased()
    {
        //triggerPressed = false;
    }


    void Teleport()
    {
        Vector3 pos = transform.position;
        pos.y = Random.Range(1.0f, 3.0f);
        transform.position = pos;
    }

    void Teleport2()
    {
        Vector3 pos = transform.position;
        pos.x = Random.Range(1.0f, 3.0f);
        transform.position = pos;
    }
}