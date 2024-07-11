using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace UnityEngine.XR.ARFoundation.Samples
{
    public class HandGestureManager : MonoBehaviour
    {
        public TMPro.TMP_Text pinchText;
        public TMPro.TMP_Text moreText;

        public GameObject objectCenter;
        public GameObject pinchZoomer;
        public GameObject cameraObject;

        public float minDistance = 1.0f;

        private float prevMagnitude = 0;
        private int touchCount = 0;

        // Start is called before the first frame update
        void Start()
        {
            pinchText.text = "Started!";

            var scrollAction = new InputAction(binding: "<Mouse>/scroll");
            scrollAction.Enable();
            scrollAction.performed += ctx => OnMouseScroll(ctx.ReadValue<Vector2>().y);

            var touch0contact = new InputAction(
                type: InputActionType.Button,
                binding: "<Touchscreen>/touch0/press"
                );
            touch0contact.Enable();

            var touch1contact = new InputAction(
                type: InputActionType.Button,
                binding: "<Touchscreen>/touch1/press"
                );
            touch1contact.Enable();

            touch0contact.performed += _ => touchCount++;
            touch1contact.performed += _ => touchCount++;
            touch0contact.canceled += _ =>
           {
               touchCount--;
               prevMagnitude = 0;
           };
            touch1contact.canceled += _ =>
            {
                touchCount--;
                prevMagnitude = 0;
            };

            var touch0pos = new InputAction(
                type: InputActionType.Value,
                binding: "<Touchscreen>/touch0/position"
                );
            touch0pos.Enable();

            var touch1pos = new InputAction(
                type: InputActionType.Value,
                binding: "<Touchscreen>/touch1/position"
                );
            touch1pos.Enable();

            touch0pos.performed += _ => 
            {
                if (touchCount < 2) return;

                var magnitude = (touch0pos.ReadValue<Vector2>() - touch1pos.ReadValue<Vector2>()).magnitude;

                if(prevMagnitude == 0)
                {
                    prevMagnitude = magnitude;
                }

                var delta = magnitude - prevMagnitude;
                prevMagnitude = magnitude;

                moreText.text = delta.ToString();

                Vector3 dir = cameraObject.transform.forward;
                pinchZoomer.transform.position += dir * (-0.1f);
            };
        }

        void OnMouseScroll(float increment) => pinchText.text = increment.ToString();
        
    }
}
