using UnityEngine;

/// <summary>
/// Automatically hooks up a property to a component on the same gameobject in the editor. Useful for skipping GetComponent calls.
/// </summary>
public class AutohookAttribute : PropertyAttribute { }

/// <summary>
/// Use an inspector that helps define a mask (for flags enums)
/// </summary>
public class EnumFlagsMaskAttribute : PropertyAttribute { }

/// <summary>
/// Use this on a <see cref="Quaternion"/> field to show a Vector3 in inspector for it.
/// </summary>
public class QuaternionToEulerAttribute : PropertyAttribute { }

/// <summary>
/// Read Only attribute. Attribute is used only to mark ReadOnly properties.
/// </summary>
public class ReadOnlyAttribute : PropertyAttribute { }
