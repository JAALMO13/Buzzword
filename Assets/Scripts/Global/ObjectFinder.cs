using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using FishNet.Object;

public class ObjectFinder : MonoBehaviour 
{
    // # ensure this gets called only once so performance is not destroyed
    public static ObjectFinder Instance;
    public List<GameObject> AllGameObjects = new();
    private void Awake() {
        Instance = this;
    }
    private void Start() {
        // GetAllGameObjects();
        DontDestroyOnLoad(this.gameObject);
    }
    public GameObject FindInAll(string obj)
    {
        foreach (var o in AllGameObjects)
        {
            try{
                if (o.name == obj) return o;
            }
            catch(MissingReferenceException){}
        }
        return null;
    }

    public GameObject FindInAllTag(string tag)
    {
        foreach (var o in AllGameObjects)
        {
            try{
                if (o.tag == tag) return o;
            }
            catch(MissingReferenceException){}
        }
        return null;
    }

    public List<GameObject> FindAllInAll(string name)
    {
        List<GameObject> all = new();
        foreach (var o in AllGameObjects)
        {
            try{
                if (o.name == name) all.Add(o);
            }
            catch(MissingReferenceException){}
        }
        return all;
    }
    public List<GameObject> GetPlayers(){
        List<GameObject> players = new();
        foreach (var o in AllGameObjects){
            try{
                if (o.name == "Player(Clone)") players.Add(o);
            }
            catch(MissingReferenceException){}      
        }
        print(players.Count);
        return players;
    }

    public void GetAllGameObjects()
    {
        AllGameObjects.Clear();
        AllGameObjects = Resources.FindObjectsOfTypeAll<GameObject>().ToList();
        AllGameObjects.RemoveAll(item => item == null);
    }
}
