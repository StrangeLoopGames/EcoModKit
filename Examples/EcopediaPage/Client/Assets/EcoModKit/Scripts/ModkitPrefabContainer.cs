using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// A container for prefabs that will get loaded by the modkit. Any type of prefab the modkit can load can be used.
/// </summary>
public class ModkitPrefabContainer : MonoBehaviour
{
    public GameObject[] Prefabs = Array.Empty<GameObject>();
}
