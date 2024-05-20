using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MegaCat_Task
{
    public class Rotator : MonoBehaviour
    {
       
        const float MIN_ROTATION = -65;
        const float MAX_ROTATION = 65;

        private void FixedUpdate()
        {
         
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.rotation = Quaternion.LookRotation(Vector3.forward, mousePos - transform.position);

            ClampRotation();
        }

        private void ClampRotation()
        {
            Vector3 euler = transform.localEulerAngles;
            if (euler.z > 180) euler.z = euler.z - 360;
            euler.z = Mathf.Clamp(euler.z, MIN_ROTATION, MAX_ROTATION);
            transform.localEulerAngles = euler;
        }

        

    }
}