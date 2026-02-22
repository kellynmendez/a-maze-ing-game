using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class TimerAdd : MonoBehaviour
{
    [SerializeField] GameObject _textPrefab;
    [SerializeField] Canvas _canvas;
    [SerializeField] float _addedTime = 2f;
    [SerializeField] float _moveDist = 3f;
    [SerializeField] float _fadeTime = 1f;
    [SerializeField] float _scrollTime = 1.5f;

    public void DisplayTimeUpdate()
    {
        GameObject textObj = Instantiate(_textPrefab, _canvas.transform);
        // Setting text
        TMP_Text textComp = textObj.GetComponent<TMP_Text>();
        textComp.text = "+" + _addedTime + " seconds!";
        // Getting canvas group
        CanvasGroup group = textObj.GetComponent<CanvasGroup>();
        // Getting end position of text upward movement
        Vector3 textStartPosition = textObj.transform.position;
        Vector3 textEndPosition = new Vector3(textStartPosition.x, textStartPosition.y + _moveDist, textStartPosition.z);

        // Fading and moving text upwards
        StartCoroutine(LerpAlpha(group, 1, 0, _fadeTime));
        StartCoroutine(LerpPosition(textObj.transform, textStartPosition, textEndPosition, _scrollTime,
            () =>
            {
                Destroy(textObj);
            }));
    }

    IEnumerator LerpAlpha(CanvasGroup group, float from, float to, float duration, System.Action OnComplete = null)
    {
        // initial value
        group.alpha = from;

        // animate value
        float elapsedTime = 0;
        while (elapsedTime < duration)
        {
            group.alpha = Mathf.Lerp(from, to, elapsedTime / duration);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // final value
        group.alpha = to;
        if (OnComplete != null) { OnComplete(); }
        yield break;
    }

    IEnumerator LerpPosition(Transform target, Vector3 from, Vector3 to, float duration, System.Action OnComplete = null)
    {
        // initial value
        target.position = from;

        // animate value
        float elapsedTime = 0;
        while (elapsedTime < duration)
        {
            target.position = Vector3.Lerp(from, to, elapsedTime / duration);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // final value
        target.position = to;
        if (OnComplete != null) { OnComplete(); }
        yield break;
    }
}
