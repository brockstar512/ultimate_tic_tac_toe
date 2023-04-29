using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WinLineHandler : MonoBehaviour
{
    private Image line;
    private RectTransform rt;

    System.Collections.IEnumerator AnimateLine()
    {
        yield return new WaitForSeconds(0.5f);

        while (line.fillAmount < 1f)
        {
            line.fillAmount += 2f * Time.deltaTime;
            yield return null;
        }
    }
}
