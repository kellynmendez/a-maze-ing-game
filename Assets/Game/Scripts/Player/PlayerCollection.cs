using UnityEngine;

public class PlayerCollection : MonoBehaviour
{
    private int keysCollected = 0;
    private Door door;
    private bool doorOpened = false;
    UIController uiController;

    private void Awake()
    {
        door = FindObjectsByType<Door>(FindObjectsSortMode.None)[0];
        uiController = FindObjectsByType<UIController>(FindObjectsSortMode.None)[0];
    }

    void Start()
    {

    }

    void Update()
    {
        if (keysCollected == 3 && !doorOpened)
        {
            Debug.Log("All keys collected! Door is opening...");
            doorOpened = true;
            door.OpenDoor();
        }
    }

    public int GetKeysCollected()
    {
        return keysCollected;
    }

    public void AddKey()
    {
        keysCollected++;
        door.KeyCollected();
        uiController.AddKey();
    }
}
