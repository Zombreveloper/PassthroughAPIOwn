using UnityEngine;
using UnityEngine.SceneManagement;
using static Oculus.Interaction.Context;

public class SceneConsistency : MonoBehaviour
{
    public static SceneConsistency Instance;

    [SerializeField] private SaveManager saveManager; //If that is in Singleton, I shouldn't even need to get that reference.
    private UserDataCVD currentUser = null;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnEnable()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // Update is called once per frame
    void Start()
    {
        currentUser = SaveManager.Instance.currentUser;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        //Hole currentUser vom SaveManager
        currentUser = SaveManager.Instance.currentUser;
        //Suche dir eine Machado Sim in dieser neuen Szene
        var sim = GameObject.Find("SimManager");
        //Wenn MachadoSim existiert,
        if (sim != null && currentUser != null)
        {
            if (sim.TryGetComponent(out MachadoSim machadosim))
            {
                //Führe LoadPersonalizedLUT darauf aus.

                Debug.Log("currentUser == null? " + (currentUser == null));
                Debug.Log("currentUser Name? " + currentUser?.Name);

                machadosim.LoadPersonalizedLUT(currentUser.Name);
            }
        }
    }
}
