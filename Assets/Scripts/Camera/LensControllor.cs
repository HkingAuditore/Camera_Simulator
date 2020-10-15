using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LensControllor : MonoBehaviour {

    public GameObject lenTop, lenMid;
    public Camera camera;
    public int fOVmin, fOVmax,fOVmid;

	void Update () {
		if(camera.fieldOfView > fOVmid)
        {
            lenMid.transform.localPosition = new Vector3(0, 0, 0.1f + (camera.fieldOfView-fOVmid)/(fOVmax-fOVmid)*(1.37f-0.1f));
        }
        else
        {
            lenTop.transform.localPosition = new Vector3(0,  -0.03f - (camera.fieldOfView - fOVmin) / (fOVmid - fOVmin) *(0.77f - 0.03f),0);
        }
	}
}
