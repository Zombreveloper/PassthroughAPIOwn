using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
//using UnityEngine.UIElements;

public class SimPanel : MonoBehaviour
{
    [SerializeField] private MachadoSim sim;

    //UI Elements for Generalized Simulation
    [SerializeField] private Slider severSlider;
    [SerializeField] private TMP_Dropdown typeSelect;

    [SerializeField] private TMP_Dropdown profileSelect;
    [SerializeField] private TextMeshProUGUI tooltip;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        severSlider.onValueChanged.AddListener(delegate { sim.ProcessLUT(typeSelect.value, severSlider.value); });
        typeSelect.onValueChanged.AddListener(delegate { sim.ProcessLUT(typeSelect.value, severSlider.value); });
        profileSelect.onValueChanged.AddListener(delegate { sim.ProcessLUT(typeSelect.value, severSlider.value); });
        RefreshDropdown();
    }

    //Profile specific methods
    private void OnUserSelected(int index)
    {
        string name = profileSelect.options[index].text;
        var user = SaveManager.Instance.GetName(name);
        if (user != null)
        {
            sim.ProcessPersonalizedLUT(user);
            Debug.Log($" I am reading user: {user.Name}, Protan: {user.ProtanScore}, Deutan: {user.DeutanScore}, Tritan: {user.TritanScore} ");
            tooltip.text = $"current Simulation severity: {sim.activeSeverity}";
            //Nochmal nachschauen. F2 erkennt Nachkommastellen nicht als solche, wenn sie durch einen Punkt getrennt sind
            /*protanText.text = user.ProtanScore.ToString("N2");
            deutanText.text = user.DeutanScore.ToString("F2");
            tritanText.text = user.TritanScore.ToString("F2");*/
        }
    }

    private void RefreshDropdown()
    {
        var users = SaveManager.Instance.Data.Users.Select(u => u.Name).ToList();
        profileSelect.ClearOptions();
        profileSelect.AddOptions(users);

        if (users.Count > 0)
            OnUserSelected(0);
        else
        {
            CreateDefaultUser();
            RefreshDropdown();
            //protanText.text = deutanText.text = tritanText.text = "no profile selected";
        }
    }

    private void CreateDefaultUser()
    {
        var defaultuser = new UserDataCVD();

        defaultuser.Name = "Default Profile";
        defaultuser.ProtanScore = 40f;
        defaultuser.DeutanScore = 19.99999f;
        defaultuser.TritanScore = 3.66666f;
        defaultuser.ProtanUV = 1f;
        defaultuser.DeutanUV = 1f;
        defaultuser.TritanUV = 1f;

        SaveManager.Instance.AddUser(defaultuser);
    }

}
