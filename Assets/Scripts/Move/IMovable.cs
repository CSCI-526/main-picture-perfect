using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMovable
{
    void Nudge(float amount); //Param/Arg is "strength" of the nudge
}
