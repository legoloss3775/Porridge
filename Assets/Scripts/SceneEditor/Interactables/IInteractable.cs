using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    int nextKeyID { get; set; }
    int previousKeyID { get; set; }

    //void GetInput();
}
