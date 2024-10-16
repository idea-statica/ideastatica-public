using IdeaStatiCa.ConnectionApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CodeSamples
{
    public static partial class ClientExamples
    {
        public static MethodInfo[] GetExampleMethods()
        {
            return typeof(ClientExamples)
                .GetMethods(BindingFlags.Public | BindingFlags.Static)
                .Where(m => m.GetParameters().Length == 1 && m.GetParameters()[0].ParameterType == typeof(ConnectionApiClient))
                .Where(m => typeof(Task).IsAssignableFrom(m.ReturnType)) // Ensure it returns a Task
                .ToArray();
        }

        public static string GetExampleFolderPathOnDesktop(string exampleName)
        {
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            string folderPath = Path.Combine(desktopPath, "ConApiSamples_" + exampleName);

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            return folderPath;
        }
    }
}
