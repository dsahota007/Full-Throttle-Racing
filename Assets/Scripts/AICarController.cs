using UnityEngine;

public class AICarController : MonoBehaviour
{
    public WheelCollider frontLeftCollider;
    public WheelCollider frontRightCollider;
    public WheelCollider rearLeftCollider;
    public WheelCollider rearRightCollider;

    public Transform frontLeftMesh;
    public Transform frontRightMesh;
    public Transform rearLeftMesh;
    public Transform rearRightMesh;

    public Transform waypointsParent; // Parent object containing all waypoints
    public float maxSteerAngle = 10f;
    public float motorForce = 1500f;
    public float brakeForce = 20000f;
    public float waypointReachThreshold = 5; // Distance to consider a waypoint "reached"
    public float steerSpeed = 45f;  
    public float reverseForce = -1000f;  
    public float stuckCheckTime = 0.5f;
    public float reverseTime = 1.2f;
    public float recoveryTime = 0.8f;  

    private Transform[] waypoints; // Array to store waypoint positions
    private int currentWaypointIndex = 0; // Current waypoint AI is targeting also good for resetting for circuts
    private Rigidbody rb;
    private float currentSteerAngle = 0f; // start off str8
    private float collisionTimer = 0f; // Timer to detect prolonged collisions
    private bool recoveringFromCollision = false;  //we start off with obv not hitting anything
    private float recoveryTimer = 0f; // Timer for the recovery process
    private bool reversing = false;  // we start off not reversing 
    private bool wrongDirection = false;  //we start off in correct angle
    private bool isBraking = false;

    void Start()
    {
   
        int count = waypointsParent.childCount; //counting all children in waypoint
        waypoints = new Transform[count]; //we store that in new array/list
        for (int i = 0; i < count; i++)
        {
            waypoints[i] = waypointsParent.GetChild(i);   //iterate and keep adding
        }


        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = new Vector3(0, -0.5f, 0);
    }

    void Update()
    {
        if (!recoveringFromCollision)
        {
            CheckWrongDirection();
            if (!wrongDirection)
            {
                HandleSteering();
            }

            UpdateWheelMeshes();   //when u leave the loop we wanna update the wheel meshes
        }
    }

    void FixedUpdate()
    {
        if (recoveringFromCollision)
        {
            RecoverFromCollision();
        }
        else if (isBraking)
        {
            ApplyBrakes();
        }
        else
        {
            HandleMotor();
        }
    }
    void HandleMotor()
    {
        // Apply motor force to the rear wheels if not reversing or in wrong direction correction
        if (!reversing && !wrongDirection)
        {
            rearLeftCollider.motorTorque = motorForce;
            rearRightCollider.motorTorque = motorForce;
        }

        // Check if the car is close to the current waypoint and move to the next one
        float distanceToWaypoint = Vector3.Distance(transform.position, waypoints[currentWaypointIndex].position);  //Vector3.Distance(Vector3 a, Vector3 b)  -- we sub b from a to ge the vectors between the two point s
        if (distanceToWaypoint < waypointReachThreshold)
        {
            AdvanceToNextWaypoint();
        }
    }

    void HandleSteering()
    {
        //if (waypoints.Length == 0) return; // Do nothing if no waypoints

        //what InverseTransformPoint does:
        //World space is the global coordinate system of your entire game. Example --> A car is at(10, 0, 5) in the world. A waypoint is at(15, 0, 10) in the world.
        //Local space It tells you where something is, relative to the object itself. example --> The waypoint is "5 units to the right and 5 units forward." This is local space, because it's from the car's perspective.

        Vector3 direction = waypoints[currentWaypointIndex].position - transform.position;     //create a vec3 var and calc the position of the current index and than we subtract car position be in the correct position  
        Vector3 localDirection = transform.InverseTransformPoint(waypoints[currentWaypointIndex].position);   //we get pos of the current waypoint and than get local pos (read notes) --> we use local pos becasue it tells x if its right or left and z if front or behind. 

        //Mathf.Clamp is a Unity function that limits a value between a minimum and a maximum ---> Mathf.Clamp(value, min, max)   -- the vlaue u want to limit, the smallest val than the largest val
        // Calculate the target steering angle based on the direction
        float targetSteerAngle = Mathf.Clamp(localDirection.x / localDirection.magnitude, -1f, 1f) * maxSteerAngle;   //magnitude is str8 line to the waypoint  (we divide because if localDirection.x = 5 --> 5 units to the right --if the waypoint far away magnitude = 10 than we can do 5/10 = 0.5 for gentle turn.

        // Smoothly adjust the steering angle
        //Lerp stands for Linear Interpolation -- smoothly moves one value (currentSteerAngle) toward another value (targetSteerAngle) over time.  (start, end, how fast)
        currentSteerAngle = Mathf.Lerp(currentSteerAngle, targetSteerAngle, Time.deltaTime * steerSpeed);  //use time delta bc of pc issuing with frames

        frontLeftCollider.steerAngle = currentSteerAngle;   // Apply the steering angle to the front wheels -- we just updating the colliding 
        frontRightCollider.steerAngle = currentSteerAngle;
    }

    void AdvanceToNextWaypoint()
    {
        currentWaypointIndex++;
        if (currentWaypointIndex >= waypoints.Length)
        {
            currentWaypointIndex = 0;  // Reset to the first waypoint for a circuit
        }
    }

    void UpdateWheelMeshes()
    {
        // Update the position and rotation of the wheel meshes to match the colliders
        UpdateWheelPose(frontLeftCollider, frontLeftMesh);
        UpdateWheelPose(frontRightCollider, frontRightMesh);
        UpdateWheelPose(rearLeftCollider, rearLeftMesh);
        UpdateWheelPose(rearRightCollider, rearRightMesh);
    }

    void UpdateWheelPose(WheelCollider collider, Transform mesh)
    {
        // Get the world position and rotation of the wheel collider
        Vector3 pos;
        Quaternion rot;
        collider.GetWorldPose(out pos, out rot);   

        // Apply the position and rotation to the visual wheel mesh
        mesh.position = pos;
        mesh.rotation = rot;
    }


    void ApplyBrakes()
    {
        frontLeftCollider.brakeTorque = brakeForce;
        frontRightCollider.brakeTorque = brakeForce;
        rearLeftCollider.brakeTorque = brakeForce;
        rearRightCollider.brakeTorque = brakeForce;

        rearLeftCollider.motorTorque = 0;
        rearRightCollider.motorTorque = 0;
    }

    void ReleaseBrakes()
    {
        frontLeftCollider.brakeTorque = 0;
        frontRightCollider.brakeTorque = 0;
        rearLeftCollider.brakeTorque = 0;
        rearRightCollider.brakeTorque = 0;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("RaceBreakPoint"))
        {
            // Get the car's speed in km/h
            float speedOfTheCar = rb.velocity.magnitude * 3.6f; // Convert m/s to km/h

            if (speedOfTheCar > 40f)
            {
                isBraking = true;
                ApplyBrakes(); // Apply braking logic
            }
            else if (speedOfTheCar < 0.1f) // Use a small threshold instead of exact zero
            {
                isBraking = false;
                ReleaseBrakes(); // Release brakes when the car is stationary
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("RaceBreakPoint"))
        {
            isBraking = false; // Ensure braking is disabled when leaving the zone
            ReleaseBrakes();   // Release brakes
        }
    }



    //OnCollisionStay is a Unity event function. Unity automatically calls it every frame while two objects are colliding.
    void OnCollisionStay(Collision collision)     //Unity automatically provides the Collision object as a parameter. Without it, the function wouldn’t know What was hit. Where the collision happened. How to respond.
    {
        // Detect collisions only at the front of the car
        Vector3 collisionDirection = collision.contacts[0].point - transform.position;    //we use index - so we can get the first contact --  .point give u world position.   If the collision happens at (5, 0, 3), .point is the position of that collision and than we subtract the cur pos of the car
        float dotProduct = Vector3.Dot(transform.forward, collisionDirection.normalized);     //Vector3.Dot((1, 0, 0), (0, 0, 1)) = 0    ----    What is transform.forward? The direction the car is currently facing in world space    --- direction of the collision point, normalized to make it easier to compare (length = 1)    1 is fully aligned and 0 is perpindicular and -1 is complete oppisite

        // Trigger recovery only if the collision is at the front
        if (collision.gameObject.CompareTag("Wall") && dotProduct > 0.5f)  //we do more than 0.5 so its mostly the front 
        {
            collisionTimer += Time.deltaTime;     //collisiontimer tracks how long car has been stuck in collision and time delta ensures timer works the same on all comps

            if (collisionTimer > stuckCheckTime)   //If the car has been stuck for too long, it starts the recovery process.
            {
                recoveringFromCollision = true;
                reversing = true; 
                recoveryTimer = reverseTime;  //Sets how long the car should reverse before trying to realign.
                Debug.Log("Car is stuck! Starting recovery...");
            }
        }
    }

    void OnCollisionExit(Collision collision)
    {
        // Reset the collision timer if no longer colliding with a wall
        if (collision.gameObject.CompareTag("Wall"))
        {
            collisionTimer = 0f;
        }
    }

    void CheckWrongDirection()
    {
        // Calculate the direction to the next waypoint
        Vector3 directionToWaypoint = waypoints[currentWaypointIndex].position - transform.position;

        // Check the dot product to see if the car is facing away
        float dotProduct = Vector3.Dot(transform.forward.normalized, directionToWaypoint.normalized);  //Vector3.Dot((1, 0, 0), (0, 0, 1)) = 0  

        if (dotProduct < 0) { // Going in the wrong direction -- 0 means back and 1 would be forward. 
            wrongDirection = true;
        }
        else {
            wrongDirection = false;
        }
    }

    void CorrectWrongDirection()   //VERY CONFUSING FUNCTION !
    {
        rearLeftCollider.motorTorque = 0; //stop the car 
        rearRightCollider.motorTorque = 0;

        // Rotate the car to face the next waypoint
        Vector3 targetDirection = waypoints[currentWaypointIndex].position - transform.position;   //create a vec3 var and calc the position of the current index and than we subtract car position be in the correct position  
        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);    //parameter is where u want to face it 
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);  //slerp is curved path in 3D (current rotation, rotation you want to reach, how fast)    ---> btw we can add a random float but we do time.Delta because on a faster PC it will bug out because of speed vs frames

        // If the car is nearly aligned with the waypoint, reset the wrong direction flag
        if (Quaternion.Angle(transform.rotation, targetRotation) < 5f)
        {
            wrongDirection = false;
            Debug.Log("Wrong direction corrected!");
        }
    }

    void RecoverFromCollision()
    {
        if (reversing)
        {
            rearLeftCollider.motorTorque = reverseForce;
            rearRightCollider.motorTorque = reverseForce;
            recoveryTimer -= Time.deltaTime;  

            if (recoveryTimer <= 0f) //when done recovering
            {
                reversing = false; //stops the wheels from reverse force
                recoveryTimer = recoveryTime;  
            }
        }
        else
        {
            // Rotate the car to face the next waypoint
            Vector3 targetDirection = waypoints[currentWaypointIndex].position - transform.position;   //create a vec3 var and calc the position of the current index and than we subtract car position be in the correct position  
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);    //parameter is where u want to face it 
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);  //slerp is curved path in 3D (current rotation, rotation you want to reach, how fast)    ---> btw we can add a random float but we do time.Delta because on a faster PC it will bug out because of speed vs frames

            // Reduce alignment timer
            recoveryTimer -= Time.deltaTime;

            // Finish recovery after alignment
            if (recoveryTimer <= 0f)
            {
                recoveringFromCollision = false;
                collisionTimer = 0f;
                Debug.Log("Recovery complete!");
            }
        }
    }
}
