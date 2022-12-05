using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Larje.Core.Services.UI
{
    public class UIToastProcessor
    {
        private Options _options;


        public UIToastProcessor(Options options)
        {
            _options = options;
        }


        [Serializable]
        public class Options
        {

        }
    }
}