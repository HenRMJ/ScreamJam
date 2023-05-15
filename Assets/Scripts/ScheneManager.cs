using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScheneManager : MonoBehaviour
{
    [SerializeField] private bool isGameScene;

    // Start is called before the first frame update
    private void Start()
    {

        if (isGameScene)
        {
            Enemy.Instance.OnEnemyDied += Enemy_OnEnemyDied;
            Player.Instance.OnPlayerDied += Player_OnPlayerDied;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q)) Application.Quit();
    }

    private void OnDisable()
    {
        if (isGameScene)
        {
            Enemy.Instance.OnEnemyDied -= Enemy_OnEnemyDied;
            Player.Instance.OnPlayerDied -= Player_OnPlayerDied;
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
