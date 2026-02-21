using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;
    int _score;
    const float MOVE_UP_DISTANCE = 30f;

    [SerializeField]
    internal GameObject _scoreTextPrefab;

    [SerializeField]
    internal Canvas _canvas;

    [SerializeField]
    internal float _fadeTime = 1f;

    [SerializeField]
    internal float _scrollTime = 1.5f;

    void Awake()
    {

    }

    public void DisplayScoreUpdate(int scoreIncrement)
    {
        GameObject textObj = Instantiate(_scoreTextPrefab, _canvas.transform);
        // Setting text
        Text textComp = textObj.GetComponent<Text>();
        if (scoreIncrement > 0)
        {
            textComp.text = "+" + scoreIncrement.ToString();
            textComp.color = Color.green;
        }
        else
        {
            textComp.text = scoreIncrement.ToString();
            textComp.color = Color.red;
        }
        // Getting canvas group
        CanvasGroup group = textObj.GetComponent<CanvasGroup>();
        // Getting end position of text upward movement
        Vector3 textStartPosition = textObj.transform.position;
        Vector3 textEndPosition = new Vector3(textStartPosition.x, textStartPosition.y + MOVE_UP_DISTANCE, textStartPosition.z);

        // Fading and moving text upwards
        StartCoroutine(LerpAlpha(group, 1, 0, _fadeTime));
        StartCoroutine(LerpPosition(textObj.transform, textStartPosition, textEndPosition, _scrollTime,
            () =>
            {
                Destroy(textObj);
            }));
    }

    public static IEnumerator LerpAlpha(CanvasGroup group, float from, float to, float duration, System.Action OnComplete = null)
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

    public static IEnumerator LerpPosition(Transform target, Vector3 from, Vector3 to, float duration, System.Action OnComplete = null)
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
