/* Simple Data Containers to store the Test Scores in, bound to a name that can be set at will
 */

using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UserDataCVD
{
    public string Name;

    public float ProtanScore;
    public float DeutanScore;
    public float TritanScore;

    public float ProtanUV;
    public float DeutanUV;
    public float TritanUV;
}

[System.Serializable]
public class AllUserData
{
    public List<UserDataCVD> Users = new List<UserDataCVD>();
}
