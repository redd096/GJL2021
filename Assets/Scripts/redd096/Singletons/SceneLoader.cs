namespace redd096
{
    using UnityEngine;
    using UnityEngine.SceneManagement;

    [AddComponentMenu("redd096/Singletons/Scene Loader")]
    public class SceneLoader : Singleton<SceneLoader>
    {
        [Header("Cursor")]
        [SerializeField] Texture2D cursorTexture = default;

        protected override void Awake()
        {
            base.Awake();

            //if this is the unique scene loader
            if (instance == this)
            {
                //by default, mouse visible (used to set texture in main menu)
                LockMouse(CursorLockMode.None);
            }
        }

        #region private API

        /// <summary>
        /// Set lockState, and visible only when not locked
        /// </summary>
        public void LockMouse(CursorLockMode lockMode)
        {
            Cursor.lockState = lockMode;
            Cursor.visible = lockMode == CursorLockMode.None;

            //if cursor visible, change texture if there is one
            if(lockMode == CursorLockMode.None && cursorTexture)
                Cursor.SetCursor(cursorTexture, Vector2.zero, CursorMode.Auto);
        }

        #endregion

        /// <summary>
        /// Resume time and hide cursor
        /// </summary>
        public void ResumeGame()
        {
            //hide pause menu
            GameManager.instance.uiManager.PauseMenu(false);

            //set timeScale to 1
            Time.timeScale = 1;

            //enable player input and hide cursor
            LockMouse(CursorLockMode.Confined);
            foreach (Player player in GameManager.instance.levelManager.Players)
                player.SetState("Resume");
        }

        /// <summary>
        /// Pause time and show cursor
        /// </summary>
        public void PauseGame()
        {
            //show pause menu
            GameManager.instance.uiManager.PauseMenu(true);

            //stop time
            Time.timeScale = 0;

            //disable player input and show cursor
            LockMouse(CursorLockMode.None);
            foreach (Player player in GameManager.instance.levelManager.Players)
                player.SetState("Pause");
        }

        /// <summary>
        /// Exit game (works also in editor)
        /// </summary>
        public void ExitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        /// <summary>
        /// Reload this scene
        /// </summary>
        public void ReloadScene()
        {
            //show cursor and set timeScale to 1
            LockMouse(CursorLockMode.None);
            Time.timeScale = 1;

            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        /// <summary>
        /// Load scene by name
        /// </summary>
        public void LoadScene(string scene)
        {
            //show cursor and set timeScale to 1
            LockMouse(CursorLockMode.None);
            Time.timeScale = 1;

            //load new scene
            SceneManager.LoadScene(scene);
        }

        /// <summary>
        /// Load next scene in build settings
        /// </summary>
        public void LoadNextScene()
        {
            //show cursor and set timeScale to 1
            LockMouse(CursorLockMode.None);
            Time.timeScale = 1;

            //load next scene
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }

        /// <summary>
        /// Open url
        /// </summary>
        public void OpenURL(string url)
        {
            Application.OpenURL(url);
        }
    }
}