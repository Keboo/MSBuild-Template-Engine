
using Microsoft.Build.Framework;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("MTE.Tasks")]
[assembly: InternalsVisibleTo("MTE.Tasks.Tests")]
namespace MTE.Core
{
    [Serializable]
    public class Config : MarshalByRefObject
    {
        public string ProjectPath { get; }
        public ITaskItem[] InputItems { get; }

        internal Config(ITaskItem[] inputItems, string projectPath)
        {
            ProjectPath = projectPath;
            var nonGeneratedItems = new List<ITaskItem>();
            foreach (var item in inputItems ?? new ITaskItem[0])
            {
                if (item.GetMetadata("IsGenerated") == bool.TrueString)
                {
                    //RemoveItems.Add(item);
                }
                else
                {
                    nonGeneratedItems.Add(item);
                }
            }
            InputItems = nonGeneratedItems.ToArray();
        }

        //public string ReplaceWithGeneratedFile(ITaskItem item)
        //{
        //    string originalFilePath = item.GetMetadata("FullPath");
        //    string generatedFile = Path.Combine(Path.GetDirectoryName(originalFilePath),
        //    $"{Path.GetFileNameWithoutExtension(originalFilePath)}.g{Path.GetExtension(originalFilePath)}");
        //
        //    if (!RemoveItems.Contains(item))
        //    {
        //        RemoveItems.Add(item);
        //    }
        //    ITaskItem newItem = new TaskItem(generatedFile);
        //    AddItems.Add(newItem);
        //
        //    return generatedFile;
        //}
        //
        //public string ReplaceWithGeneratedFile(string filePath)
        //{
        //    string generatedFile = Path.Combine(Path.GetDirectoryName(filePath),
        //        $"{Path.GetFileNameWithoutExtension(filePath)}.g{Path.GetExtension(filePath)}");
        //
        //    var item = InputItems.FirstOrDefault(x => string.Equals(filePath, x.GetMetadata("FullPath")));
        //    if (!RemoveItems.Contains(item))
        //    {
        //        RemoveItems.Add(item);
        //    }
        //    ITaskItem newItem = new TaskItem(generatedFile);
        //    AddItems.Add(newItem);
        //
        //    return generatedFile;
        //}
    }
}