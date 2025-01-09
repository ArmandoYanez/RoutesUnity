using UnityEngine;

public class PlaneClass : MonoBehaviour
{
    // Propiedades para las coordenadas X y Y
     public int X { get; private set; }
     public int Y { get; private set; }
    
    
    public void Initialize(int x, int y)
    {
        X = x;
        Y = y;
        Debug.Log("Se  inicializo " + x +"," + y);
    }
    
    public override string ToString()
    {
        return $"Plane [X: {X}, Y: {Y}]";
    }
}