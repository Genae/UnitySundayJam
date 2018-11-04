using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlignHealthBar : MonoBehaviour {

    public GameObject mainCamera;

	// Use this for initialization
	void Start () {
        mainCamera = GameObject.Find("Main Camera");
	}
	
	// Update is called once per frame
	void Update () {
        this.transform.forward = mainCamera.transform.forward * -1f;
	}
}
