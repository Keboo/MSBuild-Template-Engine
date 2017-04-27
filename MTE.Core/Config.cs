
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("MTE.Tasks")]
namespace MTE.Core
{
    [Serializable]
    public class Config
    {
        public ITaskItem[] InputItems { get; }

        internal Config(ITaskItem[] inputItems)
        {
            var nonGeneratedItems = new List<ITaskItem>();
            foreach (var item in inputItems ?? new ITaskItem[0])
            {
                if (item.GetMetadata("IsGenerated") == bool.TrueString)
                {
                    RemoveItems.Add(item);
                }
                else
                {
                    nonGeneratedItems.Add(item);
                }
            }
            InputItems = nonGeneratedItems.ToArray();
        }

        public IList<ITaskItem> RemoveItems { get; } = new List<ITaskItem>();
        public IList<ITaskItem> AddItems { get; } = new List<ITaskItem>();

        public string ReplaceWithGeneratedFile(ITaskItem item)
        {
            string originalFilePath = item.GetMetadata("FullPath");
            string generatedFile = Path.Combine(Path.GetDirectoryName(originalFilePath),
            $"{Path.GetFileNameWithoutExtension(originalFilePath)}.g{Path.GetExtension(originalFilePath)}");

            if (!RemoveItems.Contains(item))
            {
                RemoveItems.Add(item);
            }
            ITaskItem newItem = new TaskItem(generatedFile);
            AddItems.Add(newItem);

            return generatedFile;
        }
    }
}