using UnityEngine;
using UnityEngine.InputSystem;

public abstract class Piece : MonoBehaviour
{
    private Color originalColour;
    private InputAction interactAction;

    public GameManager GameManager;
    public bool IsFlipped = false;

    public void Start()
    {
        originalColour = GetComponent<Renderer>().material.color;
        interactAction = InputSystem.actions.FindAction("Attack");
        UpdateVisual();
    }

    void Update()
    {
        if (interactAction.WasPerformedThisFrame())
        {
            if (WasColliderHit())
            {
                this.gameObject.GetComponent<SpriteRenderer>().color = Color.yellow;
            }
            else
            {
                this.gameObject.GetComponent<SpriteRenderer>().color = originalColour;
            }
        }

        if (interactAction.WasCompletedThisFrame())
        {
            if (WasColliderHit())
            {
                GameManager.PieceClicked(this.gameObject);
                this.gameObject.GetComponent<SpriteRenderer>().color = originalColour;
            }
        }
    }

    bool WasColliderHit()
    {
        Ray mouseRay = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit2D raycastHit2D = Physics2D.Raycast(mouseRay.origin, mouseRay.direction);
        Collider2D clickedCollider = raycastHit2D ? raycastHit2D.collider : null;
        if (clickedCollider == this.gameObject.GetComponent<Collider2D>())
        {
            return true;
        }
        else
        {
            return false;
        }
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
