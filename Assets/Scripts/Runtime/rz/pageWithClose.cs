using UnityEngine;

namespace UnityEngine.XR.ARFoundation.Samples
{
    public class pageWithClose : MonoBehaviour
    {
        public delegate void closeDelegate();
        public static event closeDelegate closeEvent;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
        
        }

        public void closePressed()
        {
            if (closeEvent != null) closeEvent();

            gameObject.SetActive(false);
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
