﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[DisallowMultipleComponent]
public class Oscillator : MonoBehaviour {

    [SerializeField] Vector3 movementVector = new Vector3(10f, 10f, 10f);
    [SerializeField] float period = 2f;
    
    float movementFactor; //0 for not moved, 1 for fully moved

    Vector3 startingPos;
    // Use this for initialization
    void Start() {
        startingPos = transform.position;
    }

    // Update is called once per frame
    void Update() {

        if (period <= Mathf.Epsilon) { return; }

        float cycles = Time.time / period;     

        const float tau = Mathf.PI * 2;

        float rawSineWave = Mathf.Sin(cycles * tau);
        
        movementFactor = rawSineWave / 2f + 0.5f;
        Vector3 offset = movementVector * movementFactor;
        transform.position = startingPos + offset;
    }
}
