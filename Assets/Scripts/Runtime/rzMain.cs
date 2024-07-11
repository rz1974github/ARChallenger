using UnityEngine;
using TMPro;
using System;
using System.Collections;
using UnityEngine.XR.ARFoundation;

namespace UnityEngine.XR.ARFoundation.Samples
{
    enum GameState
    {
        GS_None,
        GS_Game,
        GS_GameSucceed,
        GS_GameFailed,
        GS_TakingPicture
    }

    [RequireComponent(typeof(ARTrackedImageManager))]
    public class rzMain : MonoBehaviour
    {
        public GameObject[] mainArray;
        public GameObject[] wireArray;

        float[] rankArray = new float[4];
        public TMPro.TMP_Text[] rankTextArray;

        //public GameObject theModel;
        public MessageTimer correctBox;
        public MessageTimer EndMessageBox;
        public GameObject takePicButton;
        public GameObject targetUI;
        public GameObject targetHint;
        public GameObject resetButton;
        public GameObject clockSet;
        public GameObject moreButton;
        public GameObject rankButton;
        public GameObject menuPanel;
        public GameObject rankPanel;
        public GameObject partsSelection;
        public GameObject countMask;
        public TMPro.TMP_Text timeText;
        public TMPro.TMP_Text EndMessage;
        public TMPro.TMP_Text countText;
        public Camera MainCamera;
        public GameObject UpRegion;
        public GameObject gameScene;
        public GameObject pinchZoomer;
        public GameObject swipeRotator;

        partType turnIndex = partType.PT_Pillow1;

        public float total_time = 360.0f;

        float timer;

        GameState mGameState;

        [SerializeField]
        ARTrackedImageManager m_TrackedImageManager;
        
        void OnEnable()
        {
            m_TrackedImageManager.trackedImagesChanged += OnChanged;
        }

        void OnDisable()
        {
            m_TrackedImageManager.trackedImagesChanged -= OnChanged;
        }

        void OnChanged(ARTrackedImagesChangedEventArgs eventArgs)
        {
            foreach (var newImage in eventArgs.added)
            {
                // Handle added event
                gameScene.transform.position = newImage.transform.position;
                gameScene.transform.rotation = newImage.transform.rotation;
            }

            foreach (var updatedImage in eventArgs.updated)
            {
                // Handle updated event
                gameScene.transform.position = updatedImage.transform.position;
                gameScene.transform.rotation = updatedImage.transform.rotation;
            }

            foreach (var removedImage in eventArgs.removed)
            {
                // Handle removed event
            }
        }
        
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            rankArray[0] = 0;
            rankArray[1] = 0;
            rankArray[2] = 0;
            rankArray[3] = 0;

            rzPanelScript.windowClosedEvent += onPanelClosed;
            countMask.SetActive(false);
            resetToReady(GameState.GS_None);
            resetModels();
            takePicButton.SetActive(false);

            Debug.Log("Main Started");
        }

        // Update is called once per frame
        void Update()
        {
            if (mGameState != GameState.GS_Game) return;

            timer -= Time.deltaTime;
            timer = Mathf.Max(0, timer);
            timeText.text = floatTimeToString(timer);

            if (timer <= 0)
            {
                Debug.Log("Game Failed!");

                resetToReady(GameState.GS_GameFailed);

                EndMessageBox.clearAllTimeUpHandler();
                EndMessageBox.onTimeUp += whenFailedFinished;
                EndMessage.text = "時間到! 任務失敗!";
                EndMessageBox.ShowTime(3.0f);
            }
        }

        void activateIndex(int index)
        {
            if (index < mainArray.Length)
            {
                mainArray[index].SetActive(true);
                wireArray[index].SetActive(false);
            }
        }

        void checkToWriteRank(float time)
        {
            int nFound = 999;
            for (int i = 0; i < 4; i++)
            {
                if (rankArray[i] <
                    time)
                {
                    nFound = i;
                    break;
                }
            }

            if (nFound != 999)
            {
                for (int j = 3; j >= nFound; j--)
                {
                    int k = j + 1;
                    if (k < 4)
                    {
                        rankArray[k] = rankArray[j];
                    }
                }
                rankArray[nFound] = timer;

                //reflect to Text
                for (int n = 0; n < 4; n++)
                {
                    rankTextArray[n].text = floatTimeToString(rankArray[n]);
                    Debug.Log("rank[" + n + "]=" + rankArray[n]);
                }
            }
        }

        void onPanelClosed()
        {
            moreButton.SetActive(true);
        }

        void resetToReady(GameState toState)
        {
            mGameState = toState;
            turnIndex = partType.PT_Pillow1;
            //hide
            targetUI.SetActive(false);
            targetHint.SetActive(false);
            clockSet.SetActive(false);
            menuPanel.SetActive(false);
            rankPanel.SetActive(false);
            correctBox.gameObject.SetActive(false);
            partsSelection.SetActive(false);

            //show
            resetButton.SetActive(true);
            moreButton.SetActive(true);
            rankButton.SetActive(true);

            timeText.text = floatTimeToString(timer);

            pinchZoomer.transform.localPosition = Vector3.zero;
            swipeRotator.transform.localRotation = Quaternion.identity;
        }

        void resetModels()
        {
            foreach (GameObject gmo in mainArray)
            {
                gmo.SetActive(false);
            }

            foreach (GameObject gmo in wireArray)
            {
                gmo.SetActive(true);
            }
        }

        String floatTimeToString(float value)
        {

            int nSec = (int)value % 60;
            int nMin = (int)(value - (float)nSec) / 60;

            return nMin.ToString("00") + ":" + nSec.ToString("00");
        }

        partType checkType(GameObject gameo)
        {
            Selectable sel = gameo.GetComponent<Selectable>();

            if (sel != null)
            {
                return sel.myType;
            }

            return partType.PT_None;
        }

        partType checkFamilyPartType(GameObject gameo)
        {
            partType finalType = checkType(gameo);

            if (finalType != partType.PT_None) return finalType;

            GameObject father = gameo.transform.parent.gameObject;
            if(father!=null)
            {
                finalType = checkType(father);

                if (finalType != partType.PT_None) return finalType;

                GameObject grandFather = father.transform.parent.gameObject;
                if (grandFather != null)
                {
                    finalType = checkType(grandFather);
                    if (finalType != partType.PT_None) return finalType;
                }
            }

            return partType.PT_None;
        }

        public void rayCastTest()
        {
            if (mGameState != GameState.GS_Game) return;

            Vector3 startPos = MainCamera.transform.position;

            Vector3 direction = MainCamera.transform.forward;

            RaycastHit hitInfo;

            bool Touched = false;
            if(Physics.Raycast(startPos, direction, out hitInfo, 100.0f, Physics.DefaultRaycastLayers, QueryTriggerInteraction.UseGlobal))
            {
                if(hitInfo.collider!=null)
                {
                    Debug.Log("Hit collider = " + hitInfo.collider.ToString());

                    GameObject hitObject = hitInfo.collider.gameObject;
                    if (hitObject != null)
                    {
                        Debug.Log("Hit Object = " + hitObject.ToString());

                        partType pt = checkFamilyPartType(hitObject);
                        if (pt != partType.PT_None)
                        {
                            /*
                            EndMessageBox.clearAllTimeUpHandler();
                            EndMessage.text = "Part type=" + pt.ToString();
                            EndMessageBox.ShowTime(2.0f);
                            */
                            if(pt==turnIndex)
                            {
                                CorrectActive();
                            }
                            else if (pt == partType.PT_Fake0)
                            {
                                EndMessageBox.clearAllTimeUpHandler();
                                EndMessage.text = "零件錯誤~";
                                EndMessageBox.ShowTime(2.0f);
                            }
                            else
                            {
                                EndMessageBox.clearAllTimeUpHandler();
                                EndMessage.text = "順序錯了!";
                                EndMessageBox.ShowTime(2.0f);
                            }
                            Touched = true;
                        }
                    }
                }
            }
            if(!Touched)
            {
                EndMessageBox.clearAllTimeUpHandler();
                EndMessage.text = "MISS!";
                EndMessageBox.ShowTime(1.0f);
                Touched = true;
            }
        }

        public void CorrectActive()
        {
            if (mGameState != GameState.GS_Game ) return;

            activateIndex((int)turnIndex);
            turnIndex = (partType)((int)turnIndex + 1);

            if (turnIndex == partType.PT_Fake0)
            {
                resetToReady(GameState.GS_GameSucceed);

                EndMessageBox.clearAllTimeUpHandler();
                EndMessageBox.onTimeUp += whenSucceedFinished;
                EndMessage.text = "任務成功! 時間 " + timeText.text;
                EndMessageBox.ShowTime(3.0f);
                checkToWriteRank(timer);

                Debug.Log("Game Succeed");
            }
            else
            {
                correctBox.ShowTime(0.75f);
            }
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

            NativeGallery.SaveImageToGallery(imageBytes, "EngineerAR", filename, null);

            UpRegion.SetActive(true);
            takePicButton.SetActive(true);

        }

        int nCountDown = 5;
        IEnumerator OneSecondTick()
        {
            countText.text = nCountDown.ToString();
            yield return new WaitForSeconds(1.0f);

            nCountDown -= 1;
            if(nCountDown==0)
            {
                countMask.SetActive(false);
                StartCoroutine(TakeAndSaveScreenshot());
            }
            else
            {
                StartCoroutine(OneSecondTick());
            }
        }

        public void takePicture()
        {
            nCountDown = 5;
            mGameState = GameState.GS_TakingPicture;
            countMask.SetActive(true);

            UpRegion.SetActive(false);
            takePicButton.SetActive(false);

            StartCoroutine(OneSecondTick());
        }

        public void onRankClosed()
        {
            rankButton.SetActive(true);
        }

        public void resetSwitch()
        {
            if(mGameState == GameState.GS_Game)
            {
                resetToReady(GameState.GS_None);
                resetModels();
            }
            else
            {
                resetToReady(GameState.GS_None);
                resetModels();
                startPressed();
            }
        }

        public void showMenuPanel()
        {
            menuPanel.SetActive(true);
            rankButton.SetActive(true);

            rankPanel.SetActive(false);

            moreButton.SetActive(false);
        }

        public void showRankPanel()
        {
            rankPanel.SetActive(true);
            moreButton.SetActive(true);

            rzPanelScript menuScript = menuPanel.GetComponent<rzPanelScript>();

            menuScript.closeAllPages();

            rankButton.SetActive(false);
            takePicButton.SetActive(false);
        }

        public void startPressed()
        {
            moreButton.SetActive(false);
            rankButton.SetActive(false);
            menuPanel.SetActive(false);
            rankPanel.SetActive(false);
            takePicButton.SetActive(false);

            timer = total_time;
            mGameState = GameState.GS_Game;

            targetUI.SetActive(true);
            targetHint.SetActive(true);
            resetButton.SetActive(true);
            clockSet.SetActive(true);
            partsSelection.SetActive(true);
            //Selectable.bRotating = true;

            pinchZoomer.transform.localPosition = Vector3.zero;
            swipeRotator.transform.localRotation = Quaternion.identity;
        }

        public void whenSucceedFinished()
        {
            mGameState = GameState.GS_None;

            takePicButton.SetActive(true);

            Debug.Log("whenSucceedFinished done");
        }

        public void whenFailedFinished()
        {
            mGameState = GameState.GS_None;
            resetModels();
            Debug.Log("whenFailedFinished done");
        }
    }
}
