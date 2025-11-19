using UnityEngine;
using UnityEngine.SceneManagement;

public class NavigationManager : MonoBehaviour
{   
    public void NavigateToGameScene()
    {
        SceneManager.LoadScene("AldosGate2DScene");
    }
}
