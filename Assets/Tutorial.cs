using System.Collections;

using UnityEngine;
using TMPro;

public class Tutorial : MonoBehaviour
{
    bool tutorialActive;
    [SerializeField] string tutorialText;
    [SerializeField] float tutorialEndDistance;
    [SerializeField] TextMeshProUGUI uiTutorial;
    [SerializeField] float textFadeInTime;

    bool allreadyTriggered;

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.GetComponent<PlayerCameraController>() == null || allreadyTriggered)
            return;
        uiTutorial.text = tutorialText;
        tutorialActive = true;
        allreadyTriggered = true;
        StartCoroutine(FadeTextIn());

    }
    void Update()
    {
        if (!tutorialActive)
            return;

        if (Vector3.Distance(transform.position, PlayerMovement.playerPosition) > tutorialEndDistance)
        {
            uiTutorial.text = "";
            StartCoroutine(FadeTextOut());
        }

    }

    IEnumerator FadeTextIn()
    {
        float timer = 0;
        Color textColor = uiTutorial.color;
        while (timer < 1)
        {
            timer += Time.deltaTime;
            textColor.a = Mathf.Lerp(0, 1, timer);
            uiTutorial.color = textColor;
            yield return null;
        }
        yield return null;
    }

    IEnumerator FadeTextOut()
    {
        float timer = textFadeInTime;
        Color textColor = uiTutorial.color;
        while (timer > 0)
        {
            timer -= Time.deltaTime;
            textColor.a = Mathf.Lerp(1, 0, timer);
            uiTutorial.color = textColor;
            yield return null;
        }
        tutorialActive = false;
        yield return null;
    }
}
