using UnityEngine;
using System;
using System.Collections;


namespace UnityEngine.XR.ARFoundation.Samples
{
    public class takePicture : MonoBehaviour
    {
        public delegate void takePictureDone();

        public event takePictureDone takePictureDoneEvent;

        float timer = 0;

        public void takePictureAfterSec(float time)
        {
            timer = time;
        }

        IEnumerator TakeAndSaveScreenshot()
        {
            yield return new WaitForEndOfFrame();

            Texture2D screenImage = new Texture2D(Screen.width, Screen.height);
            //Get Image from screen
            screenImage.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
            screenImage.Apply();
            //Convert to png
            byte[] imageBytes = screenImage.EncodeToPNG();

            //Save image to gallery
            String filename = "IMG" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".png";

            //ScreenCapture.CaptureScreenshot(Application.persistentDataPath + filename);

            NativeGallery.SaveImageToGallery(imageBytes, "AlbumName", filename, null);
        }

        // Update is called once per frame
        void Update()
        {
            if(timer > 0)
            {
                timer -= Time.deltaTime;
                if(timer <=0)
                {
                    //

                    StartCoroutine(TakeAndSaveScreenshot());

                    if(takePictureDoneEvent!=null)
                    {
                        takePictureDoneEvent();
                    }
                }
            }
        }
    }
}
