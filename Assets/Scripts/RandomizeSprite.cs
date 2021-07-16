using UnityEngine;

#region editor

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(RandomizeSprite), true)]
[CanEditMultipleObjects]
public class RandomizeSpriteEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        //show complessive percentage
        RandomizeSprite randomize = target as RandomizeSprite;
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Complessive Percentage", randomize.ComplessivePercentage.ToString());
    }
}

#endif

#endregion

[System.Serializable]
public struct RandomSpriteStruct
{
    [Range(0, 100)] public int percentage;
    public Sprite sprite;
}

public class RandomizeSprite : MonoBehaviour
{
    [Header("Random Sprite - by default get in children")]
    [SerializeField] SpriteRenderer spriteRenderer = default;
    [SerializeField] RandomSpriteStruct[] sprites = default;

    //complessive percentage from every drop
    public int ComplessivePercentage
    {
        get
        {
            //sum percentage of every drop
            int complessivePercentage = 0;
            foreach (RandomSpriteStruct spriteStruct in sprites)
                complessivePercentage += spriteStruct.percentage;

            return complessivePercentage;
        }
    }

    void Start()
    {
        //get sprite renderer
        if (spriteRenderer == null)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        //load random sprite
        if(spriteRenderer)
            LoadRandomSprite();
    }

    void LoadRandomSprite()
    {
        int random = Mathf.FloorToInt(Random.value * 100);
        float currentPercentage = 0;

        //foreach sprite
        foreach (RandomSpriteStruct spriteStruct in sprites)
        {
            currentPercentage += spriteStruct.percentage;

            //if in percentage, set this
            if (currentPercentage >= random)
            {
                spriteRenderer.sprite = spriteStruct.sprite;

                return;
            }
        }
    }
}
