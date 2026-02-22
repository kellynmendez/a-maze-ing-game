using UnityEngine;

public class UpDownMove : MonoBehaviour
{
    [SerializeField] float amplitude = 0.2f;
    [SerializeField] float speed = 3f;
    private Vector3 startPosition;

    void Awake()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        float yMove = startPosition.y + Mathf.Sin( Time.time * speed ) * amplitude;
        transform.position = new Vector3(startPosition.x, yMove, startPosition.z);
    }
}
