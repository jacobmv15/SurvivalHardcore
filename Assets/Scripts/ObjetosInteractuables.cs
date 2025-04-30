using UnityEngine;

public class ObjetosInteractuables : MonoBehaviour
{
    public string ItemName;

    public bool rangoJugador;

    public string GetItemName()
    {
        return ItemName;

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && rangoJugador && ControladorSeleccion.instancia.checkColider)
        {
            Debug.Log("Item añadido al inventario");
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            rangoJugador = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            rangoJugador=false; 
        }
    }
}