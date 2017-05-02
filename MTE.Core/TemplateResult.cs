using System;
using System.Collections.Generic;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace MTE.Core
{
    [Serializable]
    public sealed class TemplateResult : MarshalByRefObject
    {
        public bool Success { get; private set; } = true;

        private readonly List<string> _messages = new List<string>();
        public IReadOnlyList<string> Messages => _messages;

        private readonly List<NewItem> _addedItems = new List<NewItem>();
        public IReadOnlyList<NewItem> AddedItems => _addedItems;

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

        public void AddItem(NewItem item)
        {
            _addedItems.Add(item);
        }

        public void RemoveItem(ITaskItem item)
        {
            _removedItems.Add(item);
        }
    }

    [Serializable]
    public class NewItem : MarshalByRefObject
    {
        public NewItem(string filePath)
        {
            FilePath = filePath;
        }

        public string FilePath { get; }
    }
}