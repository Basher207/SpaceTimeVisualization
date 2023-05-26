using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GravityVisualiser
{
    public class PlayerViewMode : MonoBehaviour
    {

        [SerializeField] private Transform playerCamera;
        [SerializeField] private float defaultLocalY = 3f;
        [SerializeField] private float overViewModeLocalY = 50f;

        public bool OverViewModeActive { get; set; } = true;

        //0 if at default hight
        //1 if at overview hight

        private float overViewAlong;

        private void Awake()
        {
            overViewAlong = OverViewModeActive ? 1f : 0f;
        }

        private void Update()
        {
            overViewAlong = Mathf.Lerp(overViewAlong, OverViewModeActive ? 1f : 0f, Time.deltaTime * 8f);

            Vector3 pos = playerCamera.localPosition;
            pos.y = Mathf.Lerp(defaultLocalY, overViewModeLocalY, Mathf.SmoothStep(0f, 1f, overViewAlong));

            playerCamera.localPosition = pos;
        }
    }
}