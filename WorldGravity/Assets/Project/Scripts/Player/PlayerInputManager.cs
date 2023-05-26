using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace GravityVisualiser
{
    public class PlayerInputManager : MonoBehaviour
    {
        [SerializeField] private GravObjectPusher gravObjectPusher;
        [SerializeField] private RelativeRotationSwitcher relativeRotationSwitcher;
        [SerializeField] private PlayerViewMode playerViewMode;
        
        void Update()
        {
            CheckForMovementInput();
            CheckForRotationalReferenceFrameSwitchingInput();
            CheckForOverViewModeChange();
        }

        private void CheckForOverViewModeChange()
        {
            if (Input.GetKeyDown(KeyCode.Space))
                playerViewMode.OverViewModeActive = !playerViewMode.OverViewModeActive;
        }
        private void CheckForRotationalReferenceFrameSwitchingInput()
        {
            if (Input.GetKeyDown(KeyCode.Tab))
                relativeRotationSwitcher.RotationalReferenceFrame = !relativeRotationSwitcher.RotationalReferenceFrame;
        }
        private void CheckForMovementInput()
        {
            float horizontalInput = Input.GetAxisRaw("Horizontal");
            float forwardInput = Input.GetAxisRaw("Vertical");
            
            gravObjectPusher.RelativePushDirection = new Vector3(horizontalInput, 0f, forwardInput);
        }
    }
}