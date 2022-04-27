using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup : MonoBehaviour
{
    public int snackNumber;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (snackNumber == 1)
        {
            Client.theClient.sendSnackStatus("Snack1");
        }
        else
        {
            Client.theClient.sendSnackStatus("Snack2");
        }
        
        Destroy(gameObject);
    }
}
