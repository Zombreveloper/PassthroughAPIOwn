using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;

public class MenuManager : MonoBehaviour
{
    public TMP_Dropdown ProfileSelect;
    public Text protanText, deutanText, tritanText;

    //Alles zum virtuellen Schreiben mit dem Textfeld. weil die Systemtastatur wohl kein BuildingBlock sein darf i guess.
    public TMP_InputField TextField;
    private TouchScreenKeyboard overlayKeyboard;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        var users = SaveManager.Instance.Data.Users.Select(u => u.Name).ToList();
        ProfileSelect.ClearOptions();
        ProfileSelect.AddOptions(users);

        ProfileSelect.onValueChanged.AddListener(delegate { OnUserSelected(ProfileSelect.value); });

        TextField.onSelect.AddListener(delegate { ShowKeyboard(); });
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
}
