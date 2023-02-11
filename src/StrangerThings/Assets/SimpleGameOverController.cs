using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SimpleGameOverController : MonoBehaviour
{
    [SerializeField]
    private Button restartButton;

    // Start is called before the first frame update
    void Start()
    {
        restartButton.onClick.AddListener(OnClickRestartButton);
    }

    // Update is called once per frame
    void OnClickRestartButton()
    {
        Debug.Log("Loading game scene");
        SceneManager.LoadScene("SampleScene");
    }
}
