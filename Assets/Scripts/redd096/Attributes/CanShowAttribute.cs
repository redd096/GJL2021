namespace redd096
{
    using UnityEngine;

#if UNITY_EDITOR

    using UnityEditor;

    [CustomPropertyDrawer(typeof(CanShowAttribute))]
    public class CanShowDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            //update height only if can show (to hide space where there are hidden variables) - necessary also to update height when open a list/array/struct in inspector
            if (CanShow(property))
            {
                return EditorGUI.GetPropertyHeight(property, label, true);
            }

            //else hide space
            return 0;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            //show only if CanShow return true
            if (CanShow(property))
            {
                EditorGUI.PropertyField(position, property, label, true);
            }
        }

        #region private API

        bool CanShow(SerializedProperty property)
        {
            CanShowAttribute canShowAttribute = attribute as CanShowAttribute;

            //foreach value to check
            foreach (string value in canShowAttribute.values)
            {
                bool check = property.serializedObject.FindProperty(value).boolValue;

                //use NOT
                if (canShowAttribute.NOT)
                    check = !check;

                //if checkAND, when find one false, return false
                if (canShowAttribute.checkAND && !check)
                {
                    return false;
                }
                //if checkOR, when find one true, return true
                else if (!canShowAttribute.checkAND && check)
                {
                    return true;
                }
            }

            //else return checkAND, cause it mean every value is true if checkAND, or every value is false if checkOR
            return canShowAttribute.checkAND;
        }

        #endregion
    }

#endif

    //attribute
    public class CanShowAttribute : PropertyAttribute
    {
        /// <summary>
        /// check every value as AND or as OR
        /// </summary>
        public bool checkAND = true;

        /// <summary>
        /// set NOT for every value
        /// </summary>
        public bool NOT = false;

        public readonly string[] values;

        /// <summary>
        /// Attribute to show variable only if specific values are true
        /// </summary>
        /// <param name="values">the values to determine if show or not</param>
        public CanShowAttribute(params string[] values)
        {
            this.values = values;
        }
    }
}