using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;

public class MenuManager : MonoBehaviour
{
    public TMP_Dropdown ProfileSelect;
    public TextMeshProUGUI protanText, deutanText, tritanText; //Debug Ausgabetext der gespeicherten CVD-Werte. Auf diese Weise sichtbar im Headset Build

    //Alles zum virtuellen Schreiben mit dem Textfeld. weil die Systemtastatur wohl kein BuildingBlock sein darf I guess.
    public TMP_InputField NameField;
    public TMP_InputField ProtanField;
    public TMP_InputField DeutanField;
    public TMP_InputField TritanField;
    public Button SubmitButton, DeleteButton;
    private TouchScreenKeyboard overlayKeyboard;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        var users = SaveManager.Instance.Data.Users.Select(u => u.Name).ToList();
        ProfileSelect.ClearOptions();
        ProfileSelect.AddOptions(users);

        ProfileSelect.onValueChanged.AddListener(delegate { OnUserSelected(ProfileSelect.value); });

        //NameField.onSelect.AddListener(delegate { ShowKeyboard(); });
        SubmitButton.onClick.AddListener(() => SubmitEntry());
    }

    private void OnUserSelected(int index)
    {
        string name = ProfileSelect.options[index].text;
        var user = SaveManager.Instance.GetName(name);
        if (user != null)
        {
            //Nehme hier die Daten raus und dann kann ich die von ner anderen Klasse in die Simulation werfen lassen
            //Hier erstmal Platzhalter weil ich hab die Daten noch nicht.
            protanText.text = user.ProtanScore.ToString("F2");
            deutanText.text = user.DeutanScore.ToString("F2");
            tritanText.text = user.TritanScore.ToString("F2");
        }
    }

    //InputField methods
    public void ShowKeyboard()
    {
        overlayKeyboard = TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default);
    }

    //[TODO] Check schreiben, ob Felder gefüllt und sonst Warnhinweis geben.
    //Oder halt nix abschicken lassen und hoffen, dass User schlau ist.
    public void SubmitEntry()
    {
        var newUser = new UserDataCVD();

        newUser.Name = NameField.text;
        newUser.ProtanScore = float.Parse(ProtanField.text);
        newUser.DeutanScore = float.Parse(DeutanField.text);
        newUser.TritanScore = float.Parse(TritanField.text);
        //uv-Werte werden später hinzugefügt. Für Machado sind die irrelevant.

        SaveManager.Instance.AddUser(newUser);

        /*var name = NameField.text;
        var prot = ProtanField.text;
        var deut = DeutanField.text;
        var trit = TritanField.text;*/

        
    }
}
