using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamScript : MonoBehaviour {

    public Transform target;
    public Vector3 targetOffset;
    public GameObject go;
    public bool change = true;
    public Transform goTrans;
    public float distance = 5.0f;
    public float maxDistance = 20;
    public float minDistance = .6f;
    public float xSpeed = 200.0f;
    public float ySpeed = 200.0f;
    public int yMinLimit = -80;
    public int yMaxLimit = 80;
    public int zoomRate = 40;
    public float panSpeed = 0.3f;
    public float zoomDampening = 5.0f;

    private float xDeg = 0.0f;
    private float yDeg = 0.0f;
    private float currentDistance;
    private float desiredDistance;
    private Quaternion currentRotation;
    private Quaternion desiredRotation;
    private Quaternion rotation;
    private Vector3 position;

    void Start() { Init(); }
    void OnEnable() { Init(); }

    public void Init()
    {
        //If there is no target, create a temporary target at 'distance' from the cameras current viewpoint
        if (!target)
        {
            go = new GameObject("Cam Target");
            go.transform.position = transform.position + (transform.forward * distance);
            target = go.transform;

        }

        distance = Vector3.Distance(transform.position, target.position);
        currentDistance = distance;
        desiredDistance = distance;

        //be sure to grab the current rotations as starting points.
        position = transform.position;
        rotation = transform.rotation;
        currentRotation = transform.rotation;
        desiredRotation = transform.rotation;

        xDeg = Vector3.Angle(Vector3.right, transform.right);
        yDeg = Vector3.Angle(Vector3.up, transform.up);
    }

    /*
     * Camera logic on LateUpdate to only update after all character movement logic has been handled. 
     */
    void LateUpdate()
    {
        if (change)
        {
            changeInitValues();
        }
        // If Control and Alt and Middle button? ZOOM!
        if (Input.GetMouseButton(2) && Input.GetKey(KeyCode.LeftAlt) && Input.GetKey(KeyCode.LeftControl))
        {
            desiredDistance -= Input.GetAxis("Mouse Y") * Time.deltaTime * zoomRate * 0.125f * Mathf.Abs(desiredDistance);
        }
        // If middle mouse and left alt are selected? ORBIT
        else if (Input.GetMouseButton(2) && Input.GetKey(KeyCode.LeftAlt))
        {
            xDeg += Input.GetAxis("Mouse X") * xSpeed * 0.02f;
            yDeg -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;

            ////////OrbitAngle

            //Clamp the vertical axis for the orbit
            yDeg = ClampAngle(yDeg, yMinLimit, yMaxLimit);
            // set camera rotation 
            desiredRotation = Quaternion.Euler(yDeg, xDeg, 0);
            currentRotation = transform.rotation;

            rotation = Quaternion.Lerp(currentRotation, desiredRotation, Time.deltaTime * zoomDampening);
            transform.rotation = rotation;
        }
        // otherwise if middle mouse is selected, we pan by way of transforming the target in screenspace
        //**CHANGED to !target because this moves target object which is no beuno
        else if (Input.GetMouseButton(2) && target.GetComponent<Renderer>() == null)
        {
            //grab the rotation of the camera so we can move in a psuedo local XY space
            target.rotation = transform.rotation;
            target.Translate(Vector3.right * -Input.GetAxis("Mouse X") * panSpeed);
            target.Translate(transform.up * -Input.GetAxis("Mouse Y") * panSpeed, Space.World);
            Vector3 clampedPosition = target.position;
            // Now we can manipulte it to clamp the y element
            clampedPosition.x = Mathf.Clamp(target.position.x, 105f, 395f);
            clampedPosition.y = Mathf.Clamp(target.position.y, 55f, 330f);
            clampedPosition.z = Mathf.Clamp(target.position.z, 115f, 405f);
            // re-assigning the transform's position will clamp it
            target.position = clampedPosition;

        }

        ////////Orbit Position
        if (Input.GetMouseButton(2) && Input.GetKey(KeyCode.LeftAlt))
        {
            // affect the desired Zoom distance if we roll the scrollwheel
            desiredDistance -= Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime * zoomRate * Mathf.Abs(desiredDistance);
            //clamp the zoom min/max
            desiredDistance = Mathf.Clamp(desiredDistance, minDistance, maxDistance);
            // For smoothing of the zoom, lerp distance
            currentDistance = Mathf.Lerp(currentDistance, desiredDistance, Time.deltaTime * zoomDampening);

            // calculate position based on the new currentDistance 
            position = target.position - (rotation * Vector3.forward * currentDistance + targetOffset);
            transform.position = Vector3.MoveTowards(transform.position, position, 100 * Time.deltaTime);// position;
        }
        //else if(Input.GetMouseButton(2))
       // {
            // affect the desired Zoom distance if we roll the scrollwheel
            desiredDistance -= Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime * zoomRate * Mathf.Abs(desiredDistance);
            //clamp the zoom min/max
            desiredDistance = Mathf.Clamp(desiredDistance, minDistance, maxDistance);
            // For smoothing of the zoom, lerp distance
            currentDistance = Mathf.Lerp(currentDistance, desiredDistance, Time.deltaTime * zoomDampening);
       // Debug.Log(currentDistance + " -  - ");
        if (currentDistance <= minDistance + .1f)
        {
            //Debug.Log("3 --" + currentDistance);
            zoomAndChangePosIn();
        }
        else if (currentDistance >= maxDistance - .1f)
        {
            zoomAndChangePosOut();
        }
            // calculate position based on the new currentDistance 
            position = target.position - (rotation * Vector3.forward * currentDistance + targetOffset);
            transform.position = position;
        //}
            

        

    }

    void zoomAndChangePosIn()
    {
        target.Translate(-transform.forward * -Input.GetAxis("Mouse ScrollWheel") * panSpeed * 10f, Space.World);
        Vector3 clampedPosition = target.position;
        // Now we can manipulte it to clamp the y element
        clampedPosition.x = Mathf.Clamp(target.position.x, 105f, 395f);
        clampedPosition.y = Mathf.Clamp(target.position.y, 55f, 330f);
        clampedPosition.z = Mathf.Clamp(target.position.z, 115f, 405f);
        // re-assigning the transform's position will clamp it
        target.position = clampedPosition;
    }

    void zoomAndChangePosOut()
    {
        target.Translate(transform.forward * Input.GetAxis("Mouse ScrollWheel") * panSpeed * 10f, Space.World);
        Vector3 clampedPosition = target.position;
        // Now we can manipulte it to clamp the y element
        clampedPosition.y = Mathf.Clamp(target.position.y, 55f, 330f);
        // re-assigning the transform's position will clamp it
        target.position = clampedPosition;
    }

    private static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360)
            angle += 360;
        if (angle > 360)
            angle -= 360;
        return Mathf.Clamp(angle, min, max);
    }

    void changeInitValues()
    {
        if(target.GetComponent<Renderer>() == null)
        {
            distance = 200;
            maxDistance = 200;
            minDistance = .3f;
            yMinLimit = 0;
            yMaxLimit = 80;
            zoomRate = 200;
            panSpeed = 5;
            zoomDampening = 10;
        }
        else
        {
            distance = 200;
            maxDistance = 300;
            minDistance = 100;
            yMinLimit = 0;
            yMaxLimit = 80;
            zoomRate = 40;
            panSpeed = .1f;
            zoomDampening = 5;
        }
        change = false;
    }
}
