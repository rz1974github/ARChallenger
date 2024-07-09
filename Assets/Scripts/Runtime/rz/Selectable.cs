using UnityEngine;

namespace UnityEngine.XR.ARFoundation.Samples
{
    public enum partType
    {

        PT_Pillow1 = 0,
        PT_DarkGray2 = 1,
        PT_Purple3 = 2,
        PT_TRLight4 = 3,
        PT_Pillars5 = 4,
        PT_Floor6 = 5,
        PT_Front_Panel7 = 6,
        PT_Front_Pillars8 = 7,
        PT_Fake0 = 8,
        PT_None = 9
    }

    public class Selectable : MonoBehaviour
    {
        public static bool bRotating=true;

        public partType myType;
        public float PosDiameter = 1.0f;
        public float angle = 0;
        public float xPlace = -0.4f;
        public float rotateSpeed = 20.0f;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            float posZ = PosDiameter * Mathf.Cos(Mathf.Deg2Rad * angle);
            float posY = PosDiameter * Mathf.Sin(Mathf.Deg2Rad * angle);

            gameObject.transform.localPosition = new Vector3(xPlace, posY , posZ);
        }

        // Update is called once per frame
        void Update()
        {
            if(bRotating)
            {
                gameObject.transform.Rotate(Vector3.left, rotateSpeed * Time.deltaTime);
            }
        }
    }
}
