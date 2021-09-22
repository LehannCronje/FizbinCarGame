using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wheel : MonoBehaviour
{
    private float _grip;
    public float Grip
    {
        get
        {
            return _grip;
        }
        set
        {
            _grip = value;
        }
    }

    private float _speed;

    public float Speed
    {
        get
        {
            return _speed;
        }
        set
        {
            _speed = value;
        }
    }



}
