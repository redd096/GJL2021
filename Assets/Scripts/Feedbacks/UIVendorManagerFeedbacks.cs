using UnityEngine;
using redd096;

#region editor

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(UIVendorManagerFeedbacks))]
public class UIVendorManagerFeedbacksEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorGUILayout.Space();
        EditorGUILayout.HelpBox("If you put more things in the array, will get only one random for every event", MessageType.Info);
    }
}
#endif

#endregion

public class UIVendorManagerFeedbacks : MonoBehaviour
{
    [Header("For Animations - by default get in children")]
    [SerializeField] Animator anim;

    [Header("On Enter In Vendor Scene")]
    [SerializeField] InstantiatedGameObjectStruct[] gameObjectsOnEnter = default;
    [SerializeField] ParticleSystem[] particlesOnEnter = default;
    [SerializeField] AudioStruct[] audiosOnEnter = default;
    [SerializeField] string[] animationsOnEnter = default;

    [Header("On Open Vendor")]
    [SerializeField] InstantiatedGameObjectStruct[] gameObjectsOnOpen = default;
    [SerializeField] ParticleSystem[] particlesOnOpen = default;
    [SerializeField] AudioStruct[] audiosOnOpen = default;
    [SerializeField] string[] animationsOnOpen = default;

    [Header("On Close Vendor")]
    [SerializeField] InstantiatedGameObjectStruct[] gameObjectsOnClose = default;
    [SerializeField] ParticleSystem[] particlesOnClose = default;
    [SerializeField] AudioStruct[] audiosOnClose = default;
    [SerializeField] string[] animationsOnClose = default;

    [Header("On Buy")]
    [SerializeField] InstantiatedGameObjectStruct[] gameObjectsOnBuy = default;
    [SerializeField] ParticleSystem[] particlesOnBuy = default;
    [SerializeField] AudioStruct[] audiosOnBuy = default;
    [SerializeField] string[] animationsOnBuy = default;

    [Header("On Select Another Weapon")]
    [SerializeField] InstantiatedGameObjectStruct[] gameObjectsOnSelect = default;
    [SerializeField] ParticleSystem[] particlesOnSelect = default;
    [SerializeField] AudioStruct[] audiosOnSelect = default;
    [SerializeField] string[] animationsOnSelect = default;

    UIVendorManager vendor;

    void Awake()
    {
        //get references
        if (anim == null)
            anim = GetComponentInChildren<Animator>();
    }

    void OnEnable()
    {
        //get references
        vendor = GetComponent<UIVendorManager>();

        //add events
        if(vendor)
        {
            vendor.onEnterInVendorScene += OnEnterInVendorScene;
            vendor.onOpenVendor += OnOpenVendor;
            vendor.onCloseVendor += OnCloseVendor;
            vendor.onBuy += OnBuy;
            vendor.onSelectAnotherWeapon += OnSelectAnotherWeapon;
        }
    }

    void OnDisable()
    {
        //remove events
        if (vendor)
        {
            vendor.onEnterInVendorScene -= OnEnterInVendorScene;
            vendor.onOpenVendor -= OnOpenVendor;
            vendor.onCloseVendor -= OnCloseVendor;
            vendor.onBuy -= OnBuy;
            vendor.onSelectAnotherWeapon -= OnSelectAnotherWeapon;
        }
    }

    void InstantiateVFXAndSFX(InstantiatedGameObjectStruct[] gameObjectsOnEvent, ParticleSystem[] particlesOnEvent, AudioStruct[] audiosOnEvent, string[] animationsOnEvent)
    {
        //instantiate vfx and sfx
        GameObject instantiatedGameObject = InstantiateGameObjectManager.instance.Play(gameObjectsOnEvent, transform.position, transform.rotation);
        if (instantiatedGameObject)
        {
            //rotate left/right
            instantiatedGameObject.transform.localScale = transform.lossyScale;
        }
        ParticlesManager.instance.Play(particlesOnEvent, transform.position, transform.rotation);
        SoundManager.instance.Play(audiosOnEvent, transform.position);

        //set animations
        anim.SetTrigger(animationsOnEvent[Random.Range(0, animationsOnEvent.Length)]);
    }

    #region events

    void OnEnterInVendorScene()
    {
        //instantiate vfx and sfx
        InstantiateVFXAndSFX(gameObjectsOnEnter, particlesOnEnter, audiosOnEnter, animationsOnEnter);
    }

    void OnOpenVendor()
    {
        //instantiate vfx and sfx
        InstantiateVFXAndSFX(gameObjectsOnOpen, particlesOnOpen, audiosOnOpen, animationsOnOpen);
    }

    void OnCloseVendor()
    {
        //instantiate vfx and sfx
        InstantiateVFXAndSFX(gameObjectsOnClose, particlesOnClose, audiosOnClose, animationsOnClose);
    }

    void OnBuy(WeaponBASE weapon)
    {
        //instantiate vfx and sfx
        InstantiateVFXAndSFX(gameObjectsOnBuy, particlesOnBuy, audiosOnBuy, animationsOnBuy);
    }

    void OnSelectAnotherWeapon(WeaponBASE weapon)
    {
        //instantiate vfx and sfx
        InstantiateVFXAndSFX(gameObjectsOnSelect, particlesOnSelect, audiosOnSelect, animationsOnSelect);
    }

    #endregion
}
