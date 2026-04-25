using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(TMP_Text))]
public class LevelButton : MonoBehaviour
{
    private int _level;
    private void Start() => _level = Convert.ToInt32(GetComponent<TMP_Text>().text);
    public void LoadLevel()
    {
        SceneManager.LoadScene(_level + 2);
    }
}
