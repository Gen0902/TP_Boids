using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Aquarium : MonoBehaviour
{
    public BoidManager boidManager;
    public Transform foodPosition;
    public ParticleSystem particles;
    public Transform dangerSource;

    float timer = 0;

    bool dangerExist = false;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;
        }
        else if (particles.isPlaying)
        {
            particles.Stop();
            boidManager.UpdateInput(null, null);
        }

        if (Input.GetKeyDown(KeyCode.R)) //Flee from danger source
        {
            dangerExist = !dangerExist;
            Transform danger = (dangerExist) ? dangerSource : null;
            boidManager.UpdateInput(null, danger);
        }

        if (Input.GetKeyDown(KeyCode.F)) //Follow food source
        {
            particles.Play();
            timer = 5;
            Transform danger = (dangerExist) ? dangerSource : null;
            boidManager.UpdateInput(foodPosition, danger);
        }

        if (Input.GetKeyDown(KeyCode.G)) //Follow danger source
        {
            timer = 10;
            Transform danger = (dangerExist) ? dangerSource : null;
            boidManager.UpdateInput(dangerSource, null);
        }



    }
}
