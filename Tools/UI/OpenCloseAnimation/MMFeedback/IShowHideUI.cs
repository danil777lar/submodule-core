using System;

namespace Larje.Core.Services.UI
{
    public interface IShowHideUI
    {
        public event Action Shown;
        public event Action Hidden;
    }
}