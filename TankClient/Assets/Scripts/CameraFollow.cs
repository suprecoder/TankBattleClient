using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {
	public Transform player ;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 pos = player.position;
		pos.y += 4;
		pos.z -= 6;
		GetComponent<Transform>().position = pos;

    }
}
