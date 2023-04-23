using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScheneManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (SceneManager.GetSceneByBuildIndex(0) == SceneManager.GetActiveScene())
        {
            Enemy.OnEnemyDied += Enemy_OnEnemyDied;
            Player.OnPlayerDied += Player_OnPlayerDied;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Application.Quit();
        }
    }

    private void Enemy_OnEnemyDied(object sender, EventArgs e)
    {
        SceneManager.LoadScene(1);
    }

    private void Player_OnPlayerDied(object sender, EventArgs e)
    {
        SceneManager.LoadScene(2);
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
