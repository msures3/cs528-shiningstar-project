using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarData
{
   // Class to store star data
    public int hip; // Hipparcos number
    public float distance; // Distance from Sol in parsecs
    public Vector3 position; // 3D position relative to Sol in parsecs
    public float absoluteMagnitude; // Absolute magnitude
    public float relativeMagnitude; // Relative magnitude
    public Vector3 velocity; // Velocity relative to Sol in parsecs per year
    public string spect; // Spectral class
    public GameObject starObject; // Reference to the corresponding game object

}
