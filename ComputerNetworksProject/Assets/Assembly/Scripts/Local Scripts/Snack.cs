using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Snack : MonoBehaviour
{
    Client menuControllerClient;
    private string snackString;

    private void Awake()
    {
        menuControllerClient = GameObject.Find("MenuController").GetComponent<Client>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setSnackNumber(int snackNumber)
    {
        if (snackNumber == 1)
            snackString = "Snack1";
        else
            snackString = "Snack2";
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        menuControllerClient.sendSnackStatus(snackString);
        Destroy(gameObject);
    }
}
