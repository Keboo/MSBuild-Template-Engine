using System;
using System.Collections.Generic;

namespace MTE.Core
{
    [Serializable]
    public sealed class TemplateResult : MarshalByRefObject
    {
        public bool Success = true;

        private readonly List<string> _messages = new List<string>();
        public IReadOnlyList<string> Messages => _messages;

        //public string Messages;

        public void Failed(string message = null)
        {
            Success = false;
            if (message != null)
            {
                _messages.Add(message);
            }
        }

        public void Log(string message)
        {
            _messages.Add(message);
        }
    }
}