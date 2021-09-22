using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class AxleInfo
{
    public WheelCollider leftWheel;
    public WheelCollider rightWheel;
    public bool motor;
    public bool steering;
}

public class CarController : MonoBehaviour
{
    public List<AxleInfo> axleInfos;
    public float maxMotorTorque;
    public float maxSteeringAngle;
    public carState currentState;
    public enum carState { OFF, IDLE, RUNNING}

    // finds the corresponding visual wheel
    // correctly applies the transform
    public void ApplyLocalPositionToVisuals(WheelCollider collider)
    {
        if (collider.transform.childCount == 0)
        {
            return;
        }

        Transform visualWheel = collider.transform.GetChild(0);

        Vector3 position;
        Quaternion rotation;
        collider.GetWorldPose(out position, out rotation);

        visualWheel.transform.position = position;
        visualWheel.transform.rotation = rotation;
    }

    public void Start()
    {
        currentState = carState.OFF;
    }

    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.E))
        {
            switch (currentState)
            {
                case carState.OFF:
                    turnOnCar();
                    break;
                case carState.IDLE:
                    turnOffCar();
                    break;

            }
        }
    }

    public void FixedUpdate()
    {
        if (!currentState.Equals(carState.OFF))
        {
            float motorInput = Input.GetAxis("Vertical");
            float motor = maxMotorTorque * motorInput;
            float steeringInput = Input.GetAxis("Horizontal");
            float steering = maxSteeringAngle * steeringInput;

            foreach (AxleInfo axleInfo in axleInfos)
            {
                if (axleInfo.steering)
                {
                    axleInfo.leftWheel.steerAngle = steering;
                    axleInfo.rightWheel.steerAngle = steering;
                }
                if (axleInfo.motor)
                {
                    axleInfo.leftWheel.motorTorque = motor;
                    axleInfo.rightWheel.motorTorque = motor;
                }

                //Push the axel into the visualiser.
                ApplyLocalPositionToVisuals(axleInfo.leftWheel);
                ApplyLocalPositionToVisuals(axleInfo.rightWheel);

                if (motor > maxMotorTorque && steering > maxSteeringAngle)
                {
                    WheelFrictionCurve curve = new WheelFrictionCurve();
                    curve.extremumSlip = axleInfo.rightWheel.sidewaysFriction.extremumSlip * (steering * motor / 10);
                    curve.extremumValue = axleInfo.rightWheel.sidewaysFriction.extremumValue;
                    curve.asymptoteSlip = axleInfo.rightWheel.sidewaysFriction.asymptoteSlip;
                    curve.stiffness = axleInfo.rightWheel.sidewaysFriction.stiffness;
                    axleInfo.rightWheel.sidewaysFriction = curve;
                    axleInfo.leftWheel.sidewaysFriction = curve;
                }
                else
                {
                    WheelFrictionCurve curve = new WheelFrictionCurve();
                    curve.extremumSlip = 0.2f;
                    curve.extremumValue = axleInfo.rightWheel.sidewaysFriction.extremumValue;
                    curve.asymptoteSlip = axleInfo.rightWheel.sidewaysFriction.asymptoteSlip;
                    curve.stiffness = axleInfo.rightWheel.sidewaysFriction.stiffness;
                    axleInfo.rightWheel.sidewaysFriction = curve;
                    axleInfo.leftWheel.sidewaysFriction = curve;
                }

                if (Input.GetKeyDown(KeyCode.Space))
                {
                    WheelFrictionCurve curve = new WheelFrictionCurve();
                    curve.extremumSlip = 10f;
                    curve.extremumValue = axleInfo.rightWheel.sidewaysFriction.extremumValue;
                    curve.asymptoteSlip = axleInfo.rightWheel.sidewaysFriction.asymptoteSlip;
                    curve.stiffness = axleInfo.rightWheel.sidewaysFriction.stiffness;
                    axleInfo.rightWheel.sidewaysFriction = curve;
                    axleInfo.leftWheel.sidewaysFriction = curve;
                }

                if (motorInput > 0)
                {
                    setCarToRunning();
                }
            }
        }
    }


    private void turnOnCar()
    {
        Debug.Log("Car is turned on and on idle");
        this.currentState = carState.IDLE;
    }

    private void turnOffCar()
    {
        Debug.Log("Car is turned off");
        this.currentState = carState.OFF;
    }

    private void setCarToRunning()
    {
        this.currentState = carState.RUNNING;
    }
}