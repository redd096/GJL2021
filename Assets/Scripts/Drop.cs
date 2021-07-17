using UnityEngine;
using redd096;

#region editor

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(Drop), true)]
[CanEditMultipleObjects]
public class DropEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        //show complessive percentage
        Drop drop = target as Drop;
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Complessive Percentage", drop.ComplessivePercentage.ToString());
    }
}

#endif

#endregion

[System.Serializable]
public struct DropStruct
{
    [Range(0, 100)] public int percentage;
    public GameObject objectToDrop;
    public AudioStruct audioOnDrop;
}

public class Drop : MonoBehaviour
{
    [Header("Drop")]
    [SerializeField] DropStruct[] drops = default;

    //complessive percentage from every drop
    public int ComplessivePercentage
    {
        get
        {
            //sum percentage of every drop
            int complessivePercentage = 0;
            foreach (DropStruct drop in drops)
                complessivePercentage += drop.percentage;

            return complessivePercentage;
        }
    }

    IDamageable damageable;

    void OnEnable()
    {
        //get references
        damageable = GetComponent<IDamageable>();

        //add events
        if (damageable != null)
        {
            damageable.onDie += OnDie;
        }
    }

    private void OnDisable()
    {
        //remove events
        if(damageable != null)
        {
            damageable.onDie -= OnDie;
        }
    }

    void OnDie()
    {
        int random = Mathf.FloorToInt(Random.value * 100);
        float currentPercentage = 0;

        //foreach drop
        foreach(DropStruct drop in drops)
        {
            currentPercentage += drop.percentage;

            //if in percentage, drop this
            if(currentPercentage >= random)
            {
                if (drop.objectToDrop != null)
                    InstantiateDrop(drop);

                return;
            }
        }
    }

    void InstantiateDrop(DropStruct drop)
    {
        //instantiate drop
        Instantiate(drop.objectToDrop, transform.position, transform.rotation);

        //instantiate sound
        SoundManager.instance.Play(drop.audioOnDrop.audioClip, transform.position, drop.audioOnDrop.volume);
    }
}
