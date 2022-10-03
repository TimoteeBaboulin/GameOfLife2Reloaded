using System.Globalization;
using UnityEngine;

public class UIInputs : MonoBehaviour
{
    [SerializeField] private GameObject GameOfLifeGO;
    [SerializeField] private GameObject PlayAreaPannelGO;

    private float _generationSpeed = 0.5f;

    public float GenerationSpeed { get => _generationSpeed; }
    
    private bool _pause = true;

    public bool Pause { get => _pause; }

    public void ChangeScreenSize(string value)
    {
        if (float.TryParse(value, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out float parsedValue))
        {
            if (parsedValue < 1)
            {
                Debug.Log("Can't have negative or null screen size.");
                return;
            }

            Camera.main.orthographicSize = parsedValue;
            return;
        }
        Debug.Log("Please use numbers with no letters and no special characters.");
    }

    public void ChangeX(string value)
    {
        if (int.TryParse(value, out int parsedValue))
        {
            if (parsedValue < 1)
            {
                Debug.Log("Can't have negative or null play area size.");
                return;
            }

            Grid.Instance.X = parsedValue;
            return;
        }
        Debug.Log("Please use numbers with no letters and no special characters.");
    }

    public void ChangeY(string value)
    {
        if (int.TryParse(value, out int parsedValue))
        {
            if (parsedValue < 1)
            {
                Debug.Log("Can't have negative or null play area size.");
                return;
            }

            Grid.Instance.Y = parsedValue;
            return;
        }
        Debug.Log("Please use numbers with no letters and no special characters.");
    }

    public void PauseButton()
    {
        if (GameOfLifeGO == null)
            return;
        
        _pause = !_pause;
    }
    
    

    public void ChangeRNG(float value)
    {
        Grid.Instance.RNG = value;
    }

    public void ChangeSpeed(string value)
    {
        if (float.TryParse(value, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out float parsedValue))
        {
            if (parsedValue <= 0)
            {
                Debug.Log("Can't have negative or null play area size.");
                return;
            }

            _generationSpeed = parsedValue;
            return;
        }
        Debug.Log("Please use numbers with no letters and no special characters.");
    }
}
