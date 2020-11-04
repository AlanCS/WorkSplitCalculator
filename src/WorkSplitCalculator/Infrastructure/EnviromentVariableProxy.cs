using System;
using System.IO;

namespace Infrastructure
{
    public interface IEnviromentVariableProxy
    {
        string get(string name);
    }

    public class EnviromentVariableProxy : IEnviromentVariableProxy
    {
        public string get(string name)
        {
            var result =  Environment.GetEnvironmentVariable(name);

            if(string.IsNullOrWhiteSpace(result)) throw new InvalidDataException($"Environment variable not found: [{name}]");

            return result;
        }
    }
}
