using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public static class UnityObjectExtensions
{
    public static string GetScenePath(this MonoBehaviour b) => GetScenePath(b.transform);
    public static string GetScenePath(this GameObject g) => GetScenePath(g.transform);

    public static string GetScenePath(this Transform t)
    {
        var name = new StringBuilder();
        name.Append(t.name);
        var cur = t;
        while (cur.parent != null)
        {
            cur = cur.parent;
            name.Insert(0, '/');
            name.Insert(0, cur.name);
        }

        return name.ToString();
    }

    /// <summary> Gets or adds a component of type <typeparamref name="T"/>. </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component => gameObject.TryGetComponent<T>(out var component) ? component : gameObject.AddComponent<T>();

    /// <summary> Gets or adds a component of the provided type. </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Component GetOrAddComponent(this GameObject gameObject, Type componentType) => gameObject.TryGetComponent(componentType, out var component) ? component : gameObject.AddComponent(componentType);

    /// <summary> Checks if <paramref name="gameObject"/> has a component of type <typeparamref name="T"/>. </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasComponent<T>(this GameObject gameObject) => gameObject.TryGetComponent<T>(out _);

    /// <summary> Checks if <paramref name="component"/> has a component of type <typeparamref name="T"/>. </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasComponent<T>(this Component component) => component.gameObject.TryGetComponent<T>(out _);

}
