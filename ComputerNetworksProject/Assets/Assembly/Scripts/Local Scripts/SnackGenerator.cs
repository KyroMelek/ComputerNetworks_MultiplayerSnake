using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnackGenerator : MonoBehaviour
{
    public GameObject snackPrefab;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void generateSnack(Vector2 position)
    {
        Snack generatedSnack = (Instantiate(snackPrefab, position, Quaternion.identity, transform)).GetComponent<Snack>();

        if (transform.childCount == 0)
            generatedSnack.setSnackNumber(1);
        else
            generatedSnack.setSnackNumber(2);
    }
}
