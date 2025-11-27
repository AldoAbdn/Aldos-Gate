using UnityEngine;

public abstract class Piece : MonoBehaviour
{
    private Color originalColour;

    public bool IsFlipped = false;

    public void Start()
    {
        originalColour = GetComponent<Renderer>().material.color;
        UpdateVisual();
    }

    public void Flip()
    {
        IsFlipped = !IsFlipped;
        UpdateVisual();
    }

    private void UpdateVisual()
    {
        // Update the visual representation of the piece based on IsFlipped
        // This is a placeholder implementation; actual implementation may vary
        if (IsFlipped)
        {
            // Change to flipped appearance
            GetComponent<Renderer>().material.color = Color.cadetBlue;
        }
        else
        {
            // Change to normal appearance
            GetComponent<Renderer>().material.color = originalColour;
        }
    }
}
