using System;
using System.Collections.Generic;
using Microsoft.Build.Framework;

namespace MTE.Core
{
    [Serializable]
    public sealed class TemplateResult : MarshalByRefObject
    {
        public bool Success { get; private set; } = true;

        private readonly List<string> _messages = new List<string>();
        public IReadOnlyList<string> Messages => _messages;

        private readonly List<ITaskItem> _addedItems = new List<ITaskItem>();
        public IReadOnlyList<ITaskItem> AddedItem => _addedItems;

        private readonly List<ITaskItem> _removedItems = new List<ITaskItem>();
        public IReadOnlyList<ITaskItem> RemovedItems => _removedItems;


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

        public void AddItem(ITaskItem item)
        {
            _addedItems.Add(item);
        }

        public void RemoveItem(ITaskItem item)
        {
            _removedItems.Add(item);
        }
    }
}