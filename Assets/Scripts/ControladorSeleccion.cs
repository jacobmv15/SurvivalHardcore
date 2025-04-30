using UnityEngine;
using TMPro;

public class ControladorSeleccion : MonoBehaviour
{

    public static ControladorSeleccion instancia { get; set; }

    public GameObject interaction_Info_UI;
    TextMeshProUGUI interaction_text;

    public bool checkColider;

    private void Start()
    {
        checkColider = false;
        interaction_text = interaction_Info_UI.GetComponent<TextMeshProUGUI>();
    }

    private void Awake() 
    {
        if (instancia != null && instancia != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instancia = this;
        }
    }

    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            var selectionTransform = hit.transform;

            ObjetosInteractuables objetosInteractuables = selectionTransform.GetComponent<ObjetosInteractuables>();

            if (objetosInteractuables && objetosInteractuables.rangoJugador)
            {
                checkColider = true;
                interaction_text.text = objetosInteractuables.GetItemName();
                interaction_Info_UI.SetActive(true);
            }
            else
            {
                checkColider = false;
                interaction_Info_UI.SetActive(false);
            }

        }
        else
        {
            checkColider = false;
            interaction_Info_UI.SetActive(false);
        }
    }
}