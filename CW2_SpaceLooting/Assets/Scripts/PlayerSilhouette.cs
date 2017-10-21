using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSilhouette : MonoBehaviour
{
    public Color pcColour;
    public Color enemyColour;

    public void SetSilhouetteColour(int p)  //if p == 1 go with PC outline
    {
        MeshRenderer mr = GetComponent<MeshRenderer>();

        foreach (Material item in mr.materials)
        {
            if (p == 1)
                item.SetColor("Outline Color", pcColour);
            else
                item.SetColor("Outline Color", enemyColour);
        }
    }
}
