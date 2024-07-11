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
        public GameObject swipeMover;
        public GameObject cameraObject;

        public float minDistance = 1.0f;
        public float dragSpeed = 0.0001f;
        public InputAction draging;

        private float prevMagnitude = 0;
        private int touchCount = 0;
        private bool allowMove = false;
        private Vector2 touch0StartPos;
        private Vector2 dragVector;

        private void Awake()
        {
            draging.Enable();
            draging.performed += context => dragVector = context.ReadValue<Vector2>();
        }

        
        private IEnumerator SwipeMove()
        {
            allowMove = true;

            while(allowMove)
            {
                //Move
                if(touchCount==1)
                {
                    pinchText.text = dragVector.x.ToString();
                    swipeMover.transform.position += dragVector.x * dragSpeed * cameraObject.transform.right;
                    swipeMover.transform.position += dragVector.y * dragSpeed * cameraObject.transform.up;
                }
                
                yield return null;
            }
        }        

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

            touch0contact.performed += _ =>
            {
                touchCount++;
                //StartCoroutine(SwipeMove());
            };

            touch1contact.performed += _ =>
            {
                touchCount++;
            };

            touch0contact.canceled += _ =>
           {
               touchCount--;
               prevMagnitude = 0;
               allowMove = false;
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
                if (touchCount < 2)
                {
                    Vector2 touch0NowPos = touch0pos.ReadValue<Vector2>();
                    float touch0distance = Vector2.Distance(touch0StartPos, touch0NowPos);
                    if (touch0distance > 20)
                    {
                        StartCoroutine(SwipeMove());
                    }
                    return;
                }

                var magnitude = (touch0pos.ReadValue<Vector2>() - touch1pos.ReadValue<Vector2>()).magnitude;

                if(prevMagnitude == 0)
                {
                    prevMagnitude = magnitude;
                }

                var delta = magnitude - prevMagnitude;
                prevMagnitude = magnitude;

                moreText.text = delta.ToString();

                Vector3 dir = cameraObject.transform.forward;
                Vector3 keepPos = pinchZoomer.transform.position;
                pinchZoomer.transform.position += dir * (-0.001f * delta);

                float distance = objectDistance();

                if (distance < minDistance)
                {
                    pinchZoomer.transform.position = keepPos;
                }
                pinchText.text = distance.ToString();
            };
        }
        float objectDistance()
        {
            Vector3 objPos = objectCenter.transform.position;
            Vector3 camPos = cameraObject.transform.position;
            return Vector3.Distance(objPos, camPos);            
        }

        void OnMouseScroll(float increment) => pinchText.text = increment.ToString();
        
    }
}
