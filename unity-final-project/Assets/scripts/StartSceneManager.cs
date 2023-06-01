using System;
using System.Collections;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;
using Image = UnityEngine.UI.Image;

public class StartSceneManager: MonoBehaviour
{
    public Button StartButton;
    public TextMeshProUGUI progressText;
    public String sceneName;
    private bool loadingScene = false;
    public IPanel imagePanel;
    public Image controlsImage;

    private void Start()
    {
        controlsImage.gameObject.SetActive(false);
    }

    private void Update()
    {
        StartButton.onClick.AddListener(LoadButton);
    }
    void LoadButton()
    {
        //Start loading the Scene asynchronously and output the progress bar
        StartCoroutine(LoadScene());
    }

    IEnumerator LoadScene()
    {
        if (!loadingScene) {
            controlsImage.gameObject.SetActive(true);
            loadingScene = true;
            //Begin to load the Scene you specify
            AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName);
            //Don't let the Scene activate until you allow it to
            asyncOperation.allowSceneActivation = false;
            //When the load is still in progress, output the Text and progress bar
            while (!asyncOperation.isDone)
            {
                //Output the current progress
                progressText.text = "Loading progress: " + (asyncOperation.progress * 100) + "%";

                // Check if the load has finished
                if (asyncOperation.progress >= 0.9f)
                {
                    //Change the Text to show the Scene is ready
                    progressText.text = "Press the space bar to continue";
                    //Wait to you press the space key to activate the Scene
                    if (Input.GetKeyDown(KeyCode.Space))
                        //Activate the Scene
                        asyncOperation.allowSceneActivation = true;
                }
                yield return null;
            }
        }
    }
}