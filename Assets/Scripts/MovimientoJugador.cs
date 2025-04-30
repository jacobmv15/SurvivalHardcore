using UnityEngine;

public class MovimientoJugador : MonoBehaviour
{
    public CharacterController controller;

    public float velocidad = 12f;
    public float gravedad = -9.81f * 2;
    public float alturaSalto = 3f;

    public Transform groundCheck;
    public float distanciaSuelo = 0.4f;
    public LayerMask groundMask;

    Vector3 velocity;

    bool isGrounded;

    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, distanciaSuelo, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        //right is the red Axis, foward is the blue axis
        Vector3 move = transform.right * x + transform.forward * z;

        controller.Move(move * velocidad * Time.deltaTime);

        //check if the player is on the ground so he can jump
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            //the equation for jumping
            velocity.y = Mathf.Sqrt(alturaSalto * -2f * gravedad);
        }

        velocity.y += gravedad * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);
    }
}