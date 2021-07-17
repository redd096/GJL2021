using UnityEngine;
using UnityEngine.UI;
using redd096;

#region save class

[System.Serializable]
public class OptionsSaveClass
{
    public float volume = 1;
    public bool dashWhereYouAim = false;
    public bool fullScreen = true;

    public OptionsSaveClass(float volume, bool dashWhereYouAim, bool fullScreen)
    {
        this.volume = volume;
        this.dashWhereYouAim = dashWhereYouAim;
        this.fullScreen = fullScreen;
    }
}

#endregion

public class OptionsManager : MonoBehaviour
{
    const string SAVENAME = "Options";

    [Header("UI")]
    [SerializeField] Slider volumeSlider = default;
    [SerializeField] Toggle dashWhereYouAimToggle = default;
    [SerializeField] Toggle fullScreenToggle = default;

    OptionsSaveClass savedOptions;

    void Start()
    {
        //load options
        savedOptions = LoadOptions();

        //update UI
        UpdateUI();
    }

    void UpdateUI()
    {
        //volume slider
        if (volumeSlider)
            volumeSlider.value = savedOptions.volume;

        //where you aim toggle
        if (dashWhereYouAimToggle)
            dashWhereYouAimToggle.isOn = savedOptions.dashWhereYouAim;

        //full screen toggle
        if (fullScreenToggle)
            fullScreenToggle.isOn = savedOptions.fullScreen;
    }

    #region UI functions

    public void SetVolume(float value)
    {
        //update saved options
        savedOptions.volume = value;
        SaveLoadJSON.Save(SAVENAME, savedOptions);

        //set in scene
        SetInScene(savedOptions);
    }

    public void SetDash(bool value)
    {
        //update saved options
        savedOptions.dashWhereYouAim = value;
        SaveLoadJSON.Save(SAVENAME, savedOptions);

        //set in scene
        SetInScene(savedOptions);
    }

    public void SetFullScreen(bool value)
    {
        //update saved options
        savedOptions.fullScreen = value;
        SaveLoadJSON.Save(SAVENAME, savedOptions);

        //set in scene
        SetInScene(savedOptions);
    }

    #endregion

    #region static

    public static void SetInScene(OptionsSaveClass save)
    {
        //set volume
        AudioListener.volume = save.volume;

        //set dash
        if (GameManager.instance.levelManager)
            foreach (Player player in GameManager.instance.levelManager.Players)
                player.SetDashOption(save.dashWhereYouAim);

        //set full screen
        if (Screen.fullScreen != save.fullScreen)
        {
            Screen.fullScreen = save.fullScreen;
        }
    }

    public static OptionsSaveClass LoadOptions()
    {
        //load
        OptionsSaveClass save = SaveLoadJSON.Load<OptionsSaveClass>(SAVENAME);

        //if there are not saved options, create new one and save
        if (save == null)
        {
            save = new OptionsSaveClass(AudioListener.volume, false, Screen.fullScreen);
            SaveLoadJSON.Save(SAVENAME, save);
        }

        return save;
    }

    #endregion
}
