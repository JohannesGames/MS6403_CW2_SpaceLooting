using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerSelection : MonoBehaviour
{
    public CharacterSelectStats modelTheBigOne;
    public CharacterSelectStats modelTheFastOne;
    public CharacterSelectStats modelTheToughOne;
    public Camera renderTextureCamera;
    public AnimationCurve camMovement;
    public Button nextModel;
    public Button previousModel;
    public CharacterSelectStats[] allModels = new CharacterSelectStats[3];
    public Slider speedSlider;
    public Slider strengthSlider;
    public Slider healthSlider;
    public Button launchButton;
    public Text hostInfo;
    [HideInInspector]
    public PCControl pc;
    public RectTransform pleaseWaitPanel;

    private Quaternion currentModel;
    private int currentIndex;
    private float rotationLength = .5f;
    private bool isRotating;


    void Start()
    {
        renderTextureCamera = GetComponentInChildren<Camera>();
        renderTextureCamera.transform.LookAt(modelTheBigOne.transform);

        nextModel.onClick.AddListener(NextModel);
        previousModel.onClick.AddListener(PreviousModel);
        launchButton.onClick.AddListener(pc.BeginGame);

        currentModel = renderTextureCamera.transform.rotation;
        currentIndex = 0;
        allModels[0] = modelTheBigOne;
        allModels[1] = modelTheFastOne;
        allModels[2] = modelTheToughOne;
        BeginRotation(Random.Range(0,allModels.Length));
    }


    void Update()
    {

    }

    IEnumerator RotateTo(int newIndex)
    {
        isRotating = true;
        float progress = 0;
        float rotationTime = 0;
        renderTextureCamera.transform.LookAt(allModels[newIndex].transform);
        Quaternion newRot = renderTextureCamera.transform.rotation;
        renderTextureCamera.transform.rotation = currentModel;

        while (rotationTime < rotationLength)
        {
            yield return null;
            rotationTime += Time.deltaTime;
            progress = rotationTime / rotationLength;
            renderTextureCamera.transform.rotation = 
                Quaternion.LerpUnclamped(currentModel, newRot, camMovement.Evaluate(progress));
            speedSlider.value = Mathf.Lerp(allModels[currentIndex].speed, allModels[newIndex].speed, progress);
            strengthSlider.value = Mathf.Lerp(allModels[currentIndex].strength, allModels[newIndex].strength, progress);
            healthSlider.value = Mathf.Lerp(allModels[currentIndex].health, allModels[newIndex].health, progress);
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
        if (currentIndex < allModels.Length - 1)    // if it hasn't yet reached the last item in the array
        {
            BeginRotation(currentIndex + 1);
        }
        else
        {
            BeginRotation(0);   // else restart the array
        }
    }

    public void PreviousModel()
    {
        if (currentIndex > 0)
        {
            BeginRotation(currentIndex - 1);
        }
        else
        {
            BeginRotation(allModels.Length - 1);
        }
    }
}
