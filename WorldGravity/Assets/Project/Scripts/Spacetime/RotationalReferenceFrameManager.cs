using System.Collections;
using System.Collections.Generic;
using GravityVisualiser;
using UnityEngine;

namespace GravityVisualiser
{
    public class RotationalReferenceFrameManager : MonoBehaviour
    {
        
        //0 = static reference frame
        //1 = rotation reference frame
        public float ReferenceFrameAlong { get; set; }
        
        void LateUpdate()
        {
            Vector3 sunToMainPlanetDelta = GravitationalObject.Attractors[0].transform.localPosition -
                                           GravitationalObject.Attractors[1].transform.localPosition;
            sunToMainPlanetDelta.y = 0f;
            
            Quaternion staticReferenceFrame = Quaternion.identity;
            Quaternion rotationalReferenceFrame = Quaternion.FromToRotation(sunToMainPlanetDelta, Vector3.forward);

            Quaternion currentRotation = Quaternion.Slerp(staticReferenceFrame, rotationalReferenceFrame, ReferenceFrameAlong);
            
            transform.rotation = currentRotation;
        }
    }

}