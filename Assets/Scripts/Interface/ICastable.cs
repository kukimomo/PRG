using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using System.Text;

public interface ICastable
{
    string MyTitle
    {
        get;
    }

    Sprite MyIcon
    {
        get;
    }

    float MyCastTime
    {
        get;
    }

    Color MyBarColor
    {
        get;
    }
}

