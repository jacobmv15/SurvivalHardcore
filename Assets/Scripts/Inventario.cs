using UnityEngine;

public class Inventario : MonoBehaviour
{

    public static Inventario Instance { get; set; }

    public GameObject inventoryScreenUI;
    public bool isOpen;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }


    void Start()
    {
        isOpen = false;
    }


    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Tab) && !isOpen)
        {

            Debug.Log("Tab is pressed");
            inventoryScreenUI.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            isOpen = true;

        }
        else if (Input.GetKeyDown(KeyCode.Tab) && isOpen)
        {
            inventoryScreenUI.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            isOpen = false;
        }
    }

}