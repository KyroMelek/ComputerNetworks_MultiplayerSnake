using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup : MonoBehaviour
{
    GameObject powerup;
    // Start is called before the first frame update
    void Start()
    {
        powerup = this.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        powerup.transform.position = new Vector3(500, 500,0);        
    }
}
