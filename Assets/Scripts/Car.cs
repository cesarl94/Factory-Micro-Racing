using UnityEngine;
using System.Collections;

public enum Direction
{
    Up, Down, Left, Right, None
}

public class Car : MonoBehaviour
{
    private WheelCollider[] wheels;
    private Rigidbody rb;
    private Vector3 up;
    private float controlVertical;
    private float controlHorizontal;


    [SerializeField] private Vector2[] relationVelocityTorque;
    [SerializeField] private Vector2[] relationVelocityMaxAngle;
    [SerializeField] private Vector2[] relationVelocitySkidding;

    [HideInInspector] public Driver driver;
    [HideInInspector] public int color;
    [HideInInspector] public AudioSource engine;
    [HideInInspector] public AudioSource honk;
    [HideInInspector] public AudioSource crash;

    public float velocity
    {
        get
        {
            return rb.velocity.magnitude;
        }
    }

    public void Awake()
    {
        wheels = GetComponentsInChildren<WheelCollider>();
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = Vector3.zero;

        AudioSource[] sources = GetComponents<AudioSource>();
        engine = sources[0];
        honk = sources[1];
        crash = sources[2];


        if (relationVelocityMaxAngle.Length == 0)
        {
            Debug.LogError("No hay relación velocidad - angulo en el auto");
            Debug.Break();
        }
        if (relationVelocityTorque.Length == 0)
        {
            Debug.LogError("No hay relación velocidad - torque en el auto");
            Debug.Break();
        }
    }

    public void setDriver(Driver driver)
    {
        this.driver = driver;
        if (!driver.isPlayer)
        {
            engine.volume = 0.1f;
        }
    }

    public void setColor(int color)
    {
        this.color = color;
        Transform bodywork = Utils.findNode(transform, "Bodywork", false);
        Material mat = bodywork.GetComponent<Renderer>().material;
        mat.SetFloat("_CarColor", color);
    }

    public void drive(float verticalControl, float horizontalControl)
    {
        engine.pitch = 0.5f + verticalControl * 0.75f;
        controlHorizontal = horizontalControl;
        controlVertical = verticalControl;
    }

    public void FixedUpdate()
    {
        float currentVelocity = rb.velocity.magnitude;

        float angle = Utils.getInterpolatedValueInVectors(relationVelocityMaxAngle, currentVelocity) * controlHorizontal;
        float torque = Utils.getInterpolatedValueInVectors(relationVelocityTorque, currentVelocity) * controlVertical;

        //skiddingAngle = -angle * Utils.getInterpolatedValueInVectors(relationVelocitySkidding, currentVelocity);
        //transform.RotateAround(transform.up, (skiddingAngle - lastSkiddingAngle) * Mathf.Deg2Rad);
        // lastSkiddingAngle = skiddingAngle;

        foreach (WheelCollider wheel in wheels)
        {
            // wheel.steerAngle = -skiddingAngle + (wheel.transform.localPosition.z > 0 ? angle : 0f);
            if (wheel.transform.localPosition.z > 0)
                wheel.steerAngle = angle;

            if (wheel.transform.localPosition.z < 0)
                wheel.motorTorque = torque;

            // modificamos la rotación visual de las ruedas
            wheel.GetWorldPose(out Vector3 p, out Quaternion q);
            Transform shapeTransform = wheel.transform.GetChild(0);
            shapeTransform.position = p;
            shapeTransform.rotation = q;
        }

        up = rb.transform.up;
        if (up.y < 0)
        {
            driver.Kill();
        }

        isTouchingConveyerBelt();
    }

    private void isTouchingConveyerBelt()
    {
        if (PauseMenu.instance.getState()) return;
        Vector3 origin = transform.position + up * 0.05f;
        RaycastHit rch;
        if (Physics.Raycast(origin, -up, out rch, 0.1f, 1 << LayerMask.NameToLayer("ConveyerBelt")))
        {
            string[] xyz = rch.collider.gameObject.name.Split(',');
            Vector3 direction = new Vector3(float.Parse(xyz[0]), float.Parse(xyz[1]), float.Parse(xyz[2]));
            rb.transform.position += -direction * 0.03f;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (PauseMenu.instance.getState()) return;
        crash.Play();
        transform.RotateAround(up, controlVertical * controlHorizontal * 0.02f);
        rb.angularVelocity = Vector3.zero;
    }

    void OnCollisionStay(Collision collision)
    {
        if (PauseMenu.instance.getState()) return;
        transform.RotateAround(up, controlVertical * controlHorizontal * 0.02f);
        rb.angularVelocity = Vector3.zero;
    }

    public void Explode()
    {
        engine.Stop();
        enabled = false;
        rb.isKinematic = true;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        foreach (Renderer r in GetComponentsInChildren<Renderer>())
        {
            r.enabled = false;
        }
        LevelParser.instance.ShowExplosion(transform.position, driver);
    }

    public void Respawn()
    {
        engine.Play();
        enabled = true;
        rb.isKinematic = false;
        foreach (Renderer r in GetComponentsInChildren<Renderer>())
        {
            r.enabled = true;
        }
        Arrow lastCheckpoint = driver.lastCheckpoint != -1 ? LevelParser.instance.checkpointOrigins[driver.lastCheckpoint] : LevelParser.instance.startingPoints[driver.startingPos];
        transform.position = lastCheckpoint.origin;
        transform.forward = lastCheckpoint.forward;

        if (driver.isPlayer)
        {
            LevelParser.instance.player.restoreFollowCamera();
        }
    }
}
