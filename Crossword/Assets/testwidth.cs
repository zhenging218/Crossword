using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class testwidth : MonoBehaviour {

	// Use this for initialization
	void Start () {
        var r = GetComponent<RectTransform>();
        if(r != null)
        {
            Debug.Log(r.rect.width);
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
