using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class CarHUD : MonoBehaviour
{
    public Text speedText;  
    private Rigidbody rb;  

    void Start()
    {
        rb = GetComponent<Rigidbody>(); 
    }

    void Update()
    {
        float speedOfTheCar = rb.velocity.magnitude * 3.6f; 
        speedText.text = Mathf.RoundToInt(speedOfTheCar) + " km/h"; 
    }
}
