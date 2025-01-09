using UnityEngine.SceneManagement;
using UnityEngine;

public class Reset : MonoBehaviour
{
    public void ReiniciarEscena()
    {
        // Obtener el nombre de la escena actual
        string escenaActual = SceneManager.GetActiveScene().name;
    
        // Recargar la escena actual
        SceneManager.LoadScene(escenaActual);
    }
}
