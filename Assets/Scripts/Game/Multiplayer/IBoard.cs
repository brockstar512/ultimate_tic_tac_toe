using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public interface IBoard 
{
    public Button _button { get; }
    public byte _index { get; }
    public byte _row { get; }
    public byte _col { get; }
}
