using UnityEngine;
using UnityEngine.InputSystem;

public class Tile : MonoBehaviour
{
    private Color originalColour;
    private InputAction interactAction;

    public GameManager GameManager;
    public GameObject CurrentPiece;
    public bool IsOccupied { get => CurrentPiece != null; }
    public int X;
    public int Y;

    void Start()
    {
        originalColour = this.gameObject.GetComponent<SpriteRenderer>().color;
        interactAction = InputSystem.actions.FindAction("Attack");
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
                GameManager.TileClicked(this.gameObject);
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
}