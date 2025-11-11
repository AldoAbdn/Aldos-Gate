using UnityEngine;
using UnityEngine.InputSystem;

public class Tile : MonoBehaviour
{
    private Color originalColor;

    public GameManager gameManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        originalColor = this.gameObject.GetComponent<SpriteRenderer>().color;
    }

    // Update is called once per frame
    void Update()
    {
        if (Mouse.current.leftButton.isPressed)
        {
            Ray mouseRay = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            RaycastHit2D raycastHit2D = Physics2D.Raycast(mouseRay.origin, mouseRay.direction);
            Collider2D clickedCollider = raycastHit2D ? raycastHit2D.collider : null;
            if (clickedCollider)
            {
                clickedCollider.GetComponent<SpriteRenderer>().color = Color.yellow;
            }
        }
        else
        {
            this.gameObject.GetComponent<SpriteRenderer>().color = originalColor;
        }
    }

    private void OnMouseDown()
    {
        gameManager.NextPlayer();
    }
}
