using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {
	// Use this for initialization
	void Start () {
		
	}
	// Update is called once per frame
	void Update()
	{
		if (PlayerRoot.obj_me != null)
		{
			Vector3 pos = PlayerRoot.obj_me.transform.position;
			pos.y += 4;
			pos.z -= 6;
			gameObject.transform.position = pos;
		}

	}
}
