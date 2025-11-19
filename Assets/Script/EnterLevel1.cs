using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnterLevel1 : MonoBehaviour
{


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Transform root = collision.transform.root;
            DontDestroyOnLoad(root.gameObject);
            SceneManager.LoadScene("Level1");
        }
    }
}
