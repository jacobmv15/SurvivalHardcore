using UnityEngine;

public class MovimientoRaton : MonoBehaviour
{
    public float sensibilidadRaton = 200f;

    float xRotation = 0f;
    float YRotation = 0f;

    void Start()
    {
        //Locking the cursor to the middle of the screen and making it invisible
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        if(Inventario.Instance.isOpen == false)
        {
            float mouseX = Input.GetAxis("Mouse X") * sensibilidadRaton * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * sensibilidadRaton * Time.deltaTime;

            //control rotation around x axis (Look up and down)
            xRotation -= mouseY;

            //we clamp the rotation so we cant Over-rotate (like in real life)
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);

            //control rotation around y axis (Look up and down)
            YRotation += mouseX;

            //applying both rotations
            transform.localRotation = Quaternion.Euler(xRotation, YRotation, 0f);

        }
    }
}