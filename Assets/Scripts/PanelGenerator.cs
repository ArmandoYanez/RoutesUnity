using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;

public class PanelGenerator : MonoBehaviour
{
    [Header("Plane generator")]
    // Prefab del plano que multiplicaremos
    public GameObject plane;
 
    // Para declarar el ancho y alto que deseamos del mapa
    public int width;
    public int height;

    [Header("Materials")]
    // Materiales que se usarán para cambiar de color al primer y último.
    public Material first;
    public Material last;

    [Header("Canvas")]
    public TextMeshProUGUI Text;
    
    [Header("Principal functions")]
    private GameObject selectedPanel; // Panel seleccionado
    private bool startSelected = false; 
    private bool endSelected = false;

    private GameObject fisrtPanel;
    private GameObject endPanel;
    
    private bool newcube = true;
    private bool isMoving = false;
    
    public GameObject cube;
    
    // Start is called before the first frame update
    void Start()
    { 
        // Generamos los paneles deseados
        panelGenerator();
        
        // Reacomodamos la cámara al centro
        updateCameraPosition();
        
        // Solicitar la primer posición
        Text.text = "SELECCIONA TU INICIO";
    }
    
    void Update()
    {
        if (!startSelected)
        {
            if (Input.GetMouseButtonDown(0))
            {
                DetectarPanel("inicio");
            }
        }
        else if (!endSelected)
        {
            if (Input.GetMouseButtonDown(0))
            {
                DetectarPanel("final");
            }
        }

        if (startSelected && endSelected && newcube)
        {
            GameObject newCube = Instantiate(cube);
            newCube.transform.localPosition = fisrtPanel.transform.localPosition;
            cube = newCube;
            newcube = false;
        }
        else if (startSelected && endSelected && !isMoving)
        {
            PlaneClass startPlane = fisrtPanel.GetComponent<PlaneClass>();
            PlaneClass endPlane = endPanel.GetComponent<PlaneClass>();

            isMoving = true; // Marca que la corrutina ha comenzado
            MoverCubo(cube, startPlane, endPlane);
        }
    }
    
    #region PanelSelection
    // Método para detectar y seleccionar el panel (inicio o final)
    public void DetectarPanel(string tipo)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); // Crear un rayo desde la cámara al punto del clic
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit)) // Verifica si el rayo toca algo
        {
            if (hit.collider != null) // Si el rayo toca un collider
            {
                // Dependiendo de si es inicio o final, cambiamos el material
                if (tipo == "inicio" && !startSelected)
                {
                    selectedPanel = hit.collider.gameObject;
                    selectedPanel.GetComponent<Renderer>().material = first; // Cambiar a material "first"
                    fisrtPanel = selectedPanel;
                    startSelected = true;
                    Text.text = "SELECCIONA EL FINAL"; // Cambiar el texto para pedir el final
                }
                else if (tipo == "final" && !endSelected)
                {
                    selectedPanel = hit.collider.gameObject;
                    selectedPanel.GetComponent<Renderer>().material = last; // Cambiar a material "last"
                    endPanel = selectedPanel;
                    endSelected = true;
                    Text.text = "FINAL SELECCIONADO"; // Cambiar el texto para indicar que se seleccionó el final
                }
            }
        }
    }
    #endregion
    
    #region PanelGenerator

    public void panelGenerator()
    {
        // Generar los paneles
        float x = 0;
        float z = 0;

        for (int i = 0; i < height; i++) // Iterar sobre filas (Y)
        {
            for (int u = 0; u < width; u++) // Iterar sobre columnas (X)
            {
                GameObject newPlane = Instantiate(plane);
                newPlane.transform.localPosition = new Vector3(x, 0, z);

                // Obtener el componente PlaneClass del panel
                PlaneClass planeComponent = newPlane.GetComponent<PlaneClass>();
                if (planeComponent != null)
                {
                    // Asignar las coordenadas X e Y
                    planeComponent.Initialize(u, i);
                }

                x += 11; 
            }

            z += 11; 
            x = 0; 
        }
    }

    public void updateCameraPosition()
    {
        Camera.main.transform.localPosition = new Vector3(((height * 11) / 2), 90, ((width * 11) / 2));
    }

    #endregion
    
    #region CubeMovement

    // Método para mover el cubo desde el inicio hasta el final
    public void MoverCubo(GameObject cube, PlaneClass start, PlaneClass end)
    {
        StartCoroutine(MoverCuboRuta(cube, start, end));
    }

    // Corrutina que mueve el cubo paso a paso
    private IEnumerator MoverCuboRuta(GameObject cube, PlaneClass start, PlaneClass end)
    {
        Vector3 currentPosition = cube.transform.localPosition;
        Vector3 targetPosition = new Vector3(end.X * 11, 0, end.Y * 11);

        while (currentPosition != targetPosition)
        {
            // Determinar la siguiente posición en la ruta (un paso a la vez)
            Vector3 nextStep = currentPosition;

            if (currentPosition.x < targetPosition.x)
            {
                nextStep.x += 11;
            }
            else if (currentPosition.x > targetPosition.x)
            {
                nextStep.x -= 11;
            }
            else if (currentPosition.z < targetPosition.z)
            {
                nextStep.z += 11;
            }
            else if (currentPosition.z > targetPosition.z)
            {
                nextStep.z -= 11;
            }
            
            float elapsedTime = 0f;
            float duration = 0.5f; 
            
            while (elapsedTime < duration)
            {
                cube.transform.localPosition = Vector3.Lerp(
                    currentPosition,
                    nextStep,
                    elapsedTime / duration
                );

                elapsedTime += Time.deltaTime;
                yield return null; // Esperar al siguiente frame
            }
            
            currentPosition = nextStep;
            cube.transform.localPosition = currentPosition;

            yield return null; // Esperar al siguiente frame antes de continuar
        }

        // El cubo ha llegado al objetivo final
        cube.transform.localPosition = targetPosition;
    }
    #endregion
    
}
