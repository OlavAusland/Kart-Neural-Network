using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Layout : MonoBehaviour
{
    [HideInInspector] public List<GameObject> pieces;
    private List<GameObject> instantiatedPieces = new List<GameObject>();

    public Vector3 UpdatePosition(){if(instantiatedPieces == null) { return instantiatedPieces[instantiatedPieces.Count - 1].transform.position; } return Vector3.zero; }

    public void Add(int index, Vector3 position)
    {
        GameObject GO = Instantiate(pieces[index - 1], position, Quaternion.identity);
        instantiatedPieces.Add(GO);
    }

    public void NewLayout()
    {
        foreach(GameObject obj in instantiatedPieces) { DestroyImmediate(obj, false); }
    }

    public void SaveLayout(){ print("Failed To Save"); }
}
