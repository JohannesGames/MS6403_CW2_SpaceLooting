using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class LocalGameManager : NetworkBehaviour
{
    public static LocalGameManager LGM;

    public Sprite toolIcon;
    public Sprite compIcon;
    public Sprite boostIcon;

    void Awake()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        //// Singleton
        if (LGM == null)
        {
            LGM = this;
        }
        else if (LGM != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
        ////
    }


    void Update()
    {

    }
}
