using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Exit : MonoBehaviour
{
    [SerializeField, Tooltip("Teleports player to this location.")]
    public Transform Victory;
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag != "Player")
            return;

        var cc = other.gameObject.GetComponent<CharacterController>();
        cc.enabled = false;
        other.transform.position = Victory.position;
        cc.enabled = true;
        //Cursor.visible = true;
        //SceneManager.LoadScene("MainMenu");
    }
}
