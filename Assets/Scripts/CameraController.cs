using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float sensitivity = 10;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Mouse1))
        {
            float rotateHorizontal = Input.GetAxis("Mouse X");
            transform.Rotate(0, rotateHorizontal * sensitivity, 0);

        }
    }
}
