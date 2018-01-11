using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerSelection : MonoBehaviour
{
    public Transform modelTheBigOne;
    public Transform modelTheFastOne;
    public Transform modelTheToughOne;
    public Camera renderTextureCamera;
    public AnimationCurve camMovement;
    public Button nextModel;
    public Button previousModel;
    public Transform[] allModels = new Transform[3];

    private Quaternion currentModel;
    private int currentIndex;
    private float rotationLength;
    private bool isRotating;


    void Start()
    {
        renderTextureCamera = GetComponentInChildren<Camera>();
        renderTextureCamera.transform.LookAt(modelTheBigOne);

        nextModel.onClick.AddListener(NextModel);
        previousModel.onClick.AddListener(PreviousModel);

        currentModel = renderTextureCamera.transform.rotation;
        currentIndex = 0;
        allModels[0] = modelTheBigOne;
        allModels[0] = modelTheFastOne;
        allModels[0] = modelTheToughOne;
    }


    void Update()
    {

    }

    IEnumerator RotateTo(int newIndex)
    {
        isRotating = true;
        float progress = 0;
        float rotationTime = 0;
        renderTextureCamera.transform.LookAt(allModels[newIndex]);
        Quaternion newRot = renderTextureCamera.transform.rotation;
        renderTextureCamera.transform.rotation = currentModel;

        while (rotationTime < rotationLength)
        {
            yield return null;
            rotationTime += Time.deltaTime;
            progress = rotationTime / rotationLength;
            renderTextureCamera.transform.rotation = 
                Quaternion.LerpUnclamped(currentModel, newRot, camMovement.Evaluate(progress));
        }

        isRotating = false;
        currentIndex = newIndex;
        currentModel = newRot;
    }

    private void BeginRotation(int newIndex)
    {
        if (!isRotating) StartCoroutine(RotateTo(newIndex));
    }

    public void NextModel()
    {
        if (currentIndex < allModels.Length - 1)
        {
            BeginRotation(currentIndex++);
        }
        else
        {
            BeginRotation(0);
        }
    }

    public void PreviousModel()
    {
        if (currentIndex > 0)
        {
            BeginRotation(currentIndex--);
        }
        else
        {
            BeginRotation(allModels.Length - 1);
        }
    }
}
