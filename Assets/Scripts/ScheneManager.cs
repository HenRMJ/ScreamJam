using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScheneManager : MonoBehaviour
{
    [SerializeField] private string[] gameScenes;

    // Start is called before the first frame update
    private void Start()
    {
        foreach (string sceneName in gameScenes)
        {
            if (SceneManager.GetActiveScene().name == sceneName)
            {
                Enemy.Instance.OnEnemyDied += Enemy_OnEnemyDied;
                Player.Instance.OnPlayerDied += Player_OnPlayerDied;
                break;
            }
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q)) Application.Quit();
    }

    private void OnDisable()
    {
        foreach (string sceneName in gameScenes)
        {
            if (SceneManager.GetActiveScene().name == sceneName)
            {
                Enemy.Instance.OnEnemyDied -= Enemy_OnEnemyDied;
                Player.Instance.OnPlayerDied -= Player_OnPlayerDied;
                break;
            }
        }
    }

    private void Enemy_OnEnemyDied(object sender, EventArgs e)
    {
        SceneManager.LoadScene("Win");
    }

    private void Player_OnPlayerDied(object sender, EventArgs e)
    {
        SceneManager.LoadScene("Death");
    }

    public void ReturnToGame()
    {
        SceneManager.LoadScene(0);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
