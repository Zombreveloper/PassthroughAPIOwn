using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;

public class MenuInteraction : MonoBehaviour
{
    public TMP_InputField NameField;
    public Button SubmitButton;
    public TextMeshProUGUI Tooltip;

    private TouchScreenKeyboard overlayKeyboard;

    public float[,] resultValues;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SubmitButton.onClick.AddListener(() => SubmitEntry());
    }

    public void ShowKeyboard()
    {
        overlayKeyboard = TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default);
    }

    private void SubmitEntry()
    {
        if (AllInputFieldsFilled())
        {
            var newUser = new UserDataCVD();

            newUser.Name = NameField.text;
            newUser.ProtanScore = resultValues[0, 1];
            newUser.DeutanScore = resultValues[1, 1];
            newUser.TritanScore = resultValues[2, 1];
            newUser.ProtanUV = resultValues[0, 0];
            newUser.DeutanUV = resultValues[1, 0];
            newUser.TritanUV = resultValues[2, 0];

            SaveManager.Instance.AddUser(newUser);
            Tooltip.text = $"Your Profile is saved as {NameField.text}";
        }
        else
        {
            Tooltip.text = "Please insert a name before submitting!";
        }
    }

    private bool AllInputFieldsFilled()
    {
        TMP_InputField[] allFields = { NameField};

        //Checks for All InputFields if they are Empty or contain only WhiteSpaces (Leerzeichen) and returns bool
        //I for now leave it as is since I never know if other text fields may be added.
        return allFields.All(f => !string.IsNullOrWhiteSpace(f.text));
    }

    public void SetResultValues(float[,] testResults)
    {
        resultValues = testResults;
    }
}
