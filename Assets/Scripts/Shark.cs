using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shark : MonoBehaviour
{
    public float speed;
    private Vector3 velocity;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        float xMove = Input.GetAxisRaw("XAxis");
        float yMove = Input.GetAxisRaw("YAxis");
        float zMove = Input.GetAxisRaw("ZAxis");
        Vector3 v = new Vector3(xMove, yMove, zMove) * speed * Time.fixedDeltaTime;
        velocity = Vector3.Lerp(velocity, v, 0.1f);
        transform.position = transform.position + velocity;

        //velocity = Vector3.RotateTowards(velocity, v, 2 * Mathf.Deg2Rad, 2);
        gameObject.transform.rotation = Quaternion.LookRotation(Vector3.Normalize(velocity));

    }
}
