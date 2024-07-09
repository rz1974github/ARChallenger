using UnityEngine;

namespace UnityEngine.XR.ARFoundation.Samples
{
    public class MessageTimer : MonoBehaviour
    {
        public delegate void WhenTimeUp();
        public event WhenTimeUp onTimeUp;

        float showtime = 0;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            //gameObject.GetComponent<Renderer>().enabled = false;
            gameObject.SetActive(false);
        }

        public void clearAllTimeUpHandler()
        {
            Debug.Log("clearAllTimeUpHandler");
            /*
            foreach (System.Delegate d in onTimeUp.GetInvocationList())
            {
                onTimeUp -= (WhenTimeUp)d;
            }
            */
            onTimeUp = null;

            Debug.Log("clearAllTimeUpHandler Done");
        }

        public void ShowTime(float duration)
        {
            showtime = duration;
            Debug.Log("ShowTime:" + duration);
            gameObject.SetActive(true);
        }

        // Update is called once per frame
        void Update()
        {
            if(showtime>0)
            {
                showtime -= Time.deltaTime;
                //gameObject.GetComponent<Renderer>().enabled = true;
            }
            else
            {
                //gameObject.GetComponent<Renderer>().enabled = false;
                gameObject.SetActive(false);
                if (onTimeUp != null) onTimeUp();
            }
        }
    }
}
