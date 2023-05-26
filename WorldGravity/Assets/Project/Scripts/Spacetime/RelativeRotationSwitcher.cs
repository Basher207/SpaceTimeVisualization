using System;
using System.Collections;
using System.Collections.Generic;
using GravityVisualiser;
using UnityEngine;

namespace GravityVisualiser
{
    public class RelativeRotationSwitcher : MonoBehaviour
    {
        [Header("Time to switch from/to rotational reference frame")] [SerializeField]
        private float timeToSwitch = 4f;

        [SerializeField] private RotationalReferenceFrameManager rotationalReferenceFrameManager;
        [SerializeField] private SpaceTimeMaterialUpdater spaceTimeMaterialUpdater;
        
        public bool RotationalReferenceFrame { get; set; }

        //0 = static reference frame
        //1 = rotation reference frame
        private float staticReferenceFrameAlong = 0f;

        private void Update()
        {
            float staticReferenceFrameAlongTarget = RotationalReferenceFrame ? 1f : 0f;
            staticReferenceFrameAlong = Mathf.Lerp(
                staticReferenceFrameAlong,
                staticReferenceFrameAlongTarget,
                Time.deltaTime / timeToSwitch * 10f);

            float passedAlong = Mathf.SmoothStep(0f, 1f, staticReferenceFrameAlong);
            rotationalReferenceFrameManager.ReferenceFrameAlong = passedAlong;
            spaceTimeMaterialUpdater.AlongCentripetalForce = passedAlong;
        }
    }
}
