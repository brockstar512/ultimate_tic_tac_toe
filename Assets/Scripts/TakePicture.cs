using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TakePicture : MonoBehaviour
{
    int i = 0;

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            ScreenCapture.CaptureScreenshot($"screenshot{i}.png");
            Debug.Log("A screenshot was taken!");
            i++;
        }
    }
}

//public class ImageCapture : MonoBehaviour
//{
//    public KeyCode screenShotButton = KeyCode.P;
//    int i = 0;

//    void Update()
//    {
//        if (Input.GetKeyDown(screenShotButton))
//        {
//            StartCoroutine(TakeScreenShot());
//        }
//    }
//    IEnumerator TakeScreenShot()
//    {
//        yield return new WaitForEndOfFrame();
//        ScreenCapture.CaptureScreenshot($"screenshot{i}.png");
//        i++;
//    }
//}
