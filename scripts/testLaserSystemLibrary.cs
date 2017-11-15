using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LaserSystemLibrary;





public class testLaserSystemLibrary : MonoBehaviour {

	// Use this for initialization
	void Start () {
        GeoUTMConverter a = new GeoUTMConverter();
        Debug.Log (a.ConvertToUnityCoords(51.538730f, -0.015977f));



    }
	
	// Update is called once per frame
	void Update () {


		
	}
}
