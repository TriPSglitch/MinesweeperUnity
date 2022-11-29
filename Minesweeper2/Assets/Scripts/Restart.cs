using UnityEngine;
using UnityEngine.SceneManagement;

public class Restart : MonoBehaviour
{
    public void Restar()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}