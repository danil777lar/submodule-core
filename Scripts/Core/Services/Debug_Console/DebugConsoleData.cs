using System;
using UnityEngine;

namespace Larje.Core.Services
{
    public partial class IternalData
    {
        public DebugConsoleData DebugConsoleData;
    }
}

[Serializable]
public class DebugConsoleData
{
    public bool overlayEnabled;
}
