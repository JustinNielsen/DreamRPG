using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{

    private GameObject _obj;
    private PlayerController _pController;
    private EnemyController _eController;

    //Takes in an object and sets the variables for the object, playerController, and enemyController
    public Character(GameObject obj)
    {
        _obj = obj;
        _pController = obj.GetComponent<PlayerController>();
        _eController = obj.GetComponent<EnemyController>();
    }

    public GameObject Obj
    {
        get { return _obj; }
    }

    public PlayerController PController
    {
        get { return _pController; }
    }

    public EnemyController EController
    {
        get { return _eController; }
    }

    public void CreateCharacter(GameObject obj)
    {
        _obj = obj;
        _pController = obj.GetComponent<PlayerController>();
        _eController = obj.GetComponent<EnemyController>();
    }
}
