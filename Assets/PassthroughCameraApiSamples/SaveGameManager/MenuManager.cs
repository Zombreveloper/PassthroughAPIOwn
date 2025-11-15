using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;

public class MenuManager : MonoBehaviour
{
    public TMP_Dropdown ProfileSelect;
    public TextMeshProUGUI protanText, deutanText, tritanText; //Debug Ausgabetext der gespeicherten CVD-Werte. Auf diese Weise sichtbar im Headset Build
    public TextMeshProUGUI WarningMessage;

    //Alles zum virtuellen Schreiben mit dem Textfeld. weil die Systemtastatur wohl kein BuildingBlock sein darf I guess.
    public TMP_InputField NameField;
    public TMP_InputField ProtanField, DeutanField, TritanField;
    public Button SubmitButton, DeleteButton;
    private TouchScreenKeyboard overlayKeyboard;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        /*var users = SaveManager.Instance.Data.Users.Select(u => u.Name).ToList();
        ProfileSelect.ClearOptions();
        ProfileSelect.AddOptions(users);*/
        RefreshDropdown();

        ProfileSelect.onValueChanged.AddListener(delegate { OnUserSelected(ProfileSelect.value); });

        //NameField.onSelect.AddListener(delegate { ShowKeyboard(); });
        SubmitButton.onClick.AddListener(() => SubmitEntry());
        DeleteButton.onClick.AddListener(() => DeleteEntry(ProfileSelect.value));
    }

    //Method for Profile Selection via Dropdown Menu
    private void OnUserSelected(int index)
    {
        string name = ProfileSelect.options[index].text;
        var user = SaveManager.Instance.GetByName(name);
        if (user != null)
        {
            //Nochmal nachschauen. F2 erkennt Nachkommastellen nicht als solche, wenn sie durch einen Punkt getrennt sind
            protanText.text = user.ProtanScore.ToString("N2");
            deutanText.text = user.DeutanScore.ToString("F2");
            tritanText.text = user.TritanScore.ToString("F2");
        }
    }

    private void RefreshDropdown()
    {
        var users = SaveManager.Instance.Data.Users.Select(u => u.Name).ToList();
        ProfileSelect.ClearOptions();
        ProfileSelect.AddOptions(users);

        if (users.Count > 0)
            OnUserSelected(0);
        else
        {
            protanText.text = deutanText.text = tritanText.text = "no profile selected";
        }
    }

    //InputField methods
    public void ShowKeyboard()
    {
        overlayKeyboard = TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default);
    }

    //[TODO] Check schreiben, ob Felder gefüllt und sonst Warnhinweis geben.
    //Oder halt nix abschicken lassen und hoffen, dass User schlau ist.
    private void SubmitEntry()
    {
        if (AllInputFieldsFilled())
        {
            var newUser = new UserDataCVD();

            newUser.Name = NameField.text;
            newUser.ProtanScore = float.Parse(ProtanField.text);
            newUser.DeutanScore = float.Parse(DeutanField.text);
            newUser.TritanScore = float.Parse(TritanField.text);
            //uv-Werte werden später hinzugefügt. Für Machado sind die irrelevant.

            SaveManager.Instance.AddUser(newUser);
            WarningMessage.text = "New Profile added";

            RefreshDropdown();
        }
        else
        {
            WarningMessage.text = "Please fill every Text Box before Submitting!";
        }
    }

    private bool AllInputFieldsFilled()
    {
        TMP_InputField[] allFields = {NameField, ProtanField, DeutanField, TritanField};

        //How didn't I know of those Linq Expressions before??
        //Anyway. Checks for All InputFields if they are Empty or contain only WhiteSpaces (Leerzeichen) and returns bool
        return allFields.All(f => !string.IsNullOrWhiteSpace(f.text));
    }

    private void DeleteEntry(int index)
    {
        if (ProfileSelect.options.Count == 0)
        {
            WarningMessage.text = "No profile selected to delete";
            return;
        }

        string name = ProfileSelect.options[index].text;
        SaveManager.Instance.DeleteUser(name);

        RefreshDropdown();
    }
}
