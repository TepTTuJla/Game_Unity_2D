using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject authMenu;
    public GameObject registerMenu;
    public GameObject ratingMenu;
    
    private bool _authenticated;
    private bool _register;
    private bool _rating;

    public void Authenticated()
    {
        if (!_authenticated)
        {
            mainMenu.SetActive(false);
            authMenu.SetActive(true);
            _authenticated = true;
        }
        else
        {
            authMenu.SetActive(false);
            mainMenu.SetActive(true);
            _authenticated = false;
        }
    }

    public void Register()
    {
        if (!_register)
        {
            authMenu.SetActive(false);
            registerMenu.SetActive(true);
            _register = true;
        }
        else
        {
            registerMenu.SetActive(false);
            authMenu.SetActive(true);
            _register = false;
        }
    }

    public void PlayGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void Rating()
    {
        if (!_rating)
        {
            mainMenu.SetActive(false);
            ratingMenu.SetActive(true);
            _rating = true;
        }
        else
        {
            ratingMenu.SetActive(false);
            mainMenu.SetActive(true);
            _rating = false;
        }
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
