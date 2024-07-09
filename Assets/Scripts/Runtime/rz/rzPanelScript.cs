using UnityEngine;

namespace UnityEngine.XR.ARFoundation.Samples
{
    public class rzPanelScript : MonoBehaviour
    {
        public delegate void OnWindowClosed();
        public static event OnWindowClosed windowClosedEvent;

        public GameObject page0;
        public GameObject page1;
        public GameObject page2;

        public void closeWindow()
        {
            if(windowClosedEvent!=null) windowClosedEvent();
            this.gameObject.SetActive(false);
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            page0.SetActive(false);
            page1.SetActive(false);
            page2.SetActive(false);

            pageWithClose.closeEvent += onPageClosed;
        }

        void onPageClosed()
        {
            this.gameObject.SetActive(true);
        }

        public void closeAllPages()
        {
            page0.SetActive(false);
            page1.SetActive(false);
            page2.SetActive(false);

            this.gameObject.SetActive(false);
        }

        public void openPageIndex(GameObject thePage)
        {
            thePage.SetActive(true);
            gameObject.SetActive(false);
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
