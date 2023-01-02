using System;

namespace Larje.Core.Services.UI
{
    public interface IOpenCloseUI
    {
        public event Action Opened;
        public event Action Closed;
    }
}