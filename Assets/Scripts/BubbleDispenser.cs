using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleDispenser : MonoBehaviour
{
    public GameObject bubbleBp;
    public Transform spawnPos;
    public int rate;

    private float timer = 10f;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.fixedDeltaTime;
        if (timer >= rate)
        {
            timer = 0;
            GameObject go = Instantiate(bubbleBp, spawnPos.position, Quaternion.identity, spawnPos);
        }
    }
}
