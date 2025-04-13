using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace SmartSpin.Support
{
    internal class MachineSettings
    {
        private readonly string _setupFilename;
        private JsonNode? _jsonData;

        public MachineSettings(string setupFilename)
        {
            _setupFilename = setupFilename;
            LoadFile();
        }

        private void LoadFile()
        {
            if (!File.Exists(_setupFilename))
            {
                throw new FileNotFoundException($"The file '{_setupFilename}' does not exist.");
            }

            var jsonContent = File.ReadAllText(_setupFilename);
            _jsonData = JsonNode.Parse(jsonContent) ?? throw new InvalidOperationException("Failed to parse JSON file.");
        }

        public string? GetValue(string nodePath)
        {
            if (_jsonData == null)
                throw new InvalidOperationException("JSON data is not loaded.");

            return _jsonData.SelectToken(nodePath)?.ToString();
        }

        public bool SetupReadBool(string item, bool defaultValue)
        {
            if (_jsonData.SelectToken(item) is JsonNode data)
            {
                return (bool)data;
            }
            else
            {
                return defaultValue;
            }
        }

        public JsonNode? GetNode(string nodePath)
        {
            if (_jsonData == null)
                throw new InvalidOperationException("JSON data is not loaded.");

            return _jsonData.SelectToken(nodePath);
        }

        public void SetValue<T>(string nodePath, T value) where T : IComparable, IConvertible, IFormattable
        {
            if (_jsonData == null)
                throw new InvalidOperationException("JSON data is not loaded.");

            var node = _jsonData.SelectToken(nodePath) ?? throw new ArgumentException($"Node path '{nodePath}' does not exist.");
            if (!node.AsValue().Equals(value))
            {
                node.ReplaceWith(value);
                Save();
            }
        }

        public void Save()
        {
            if (_jsonData != null)
            {
                File.WriteAllText(_setupFilename, _jsonData.ToJsonString(new JsonSerializerOptions { WriteIndented = true }));
            }
        }

        public void SetupWriteValue(string XPath, string NodeName, string value)
        {
            if (_jsonData == null)
                throw new InvalidOperationException("JSON data is not loaded.");

            var xn = _jsonData.SelectToken(XPath);
            if (xn == null)
                return;

            xn[NodeName] = value;
            Save();
        }

        public void SetupWriteValue(string XPath, string NodeName, int value)
        {
            if (_jsonData == null)
                throw new InvalidOperationException("JSON data is not loaded.");

            var xn = _jsonData.SelectToken(XPath);
            if (xn == null)
                return;

            xn[NodeName] = value;
            Save();
        }

        public void SetupWriteValue(string XPath, string NodeName, double value)
        {
            if (_jsonData == null)
                throw new InvalidOperationException("JSON data is not loaded.");

            var xn = _jsonData.SelectToken(XPath);
            if (xn == null)
                return;

            xn[NodeName] = value;
            Save();
        }

        public void SetupWriteValue(string XPath, string NodeName, bool value)
        {
            if (_jsonData == null)
                throw new InvalidOperationException("JSON data is not loaded.");

            var xn = _jsonData.SelectToken(XPath);
            if (xn == null)
                return;

            xn[NodeName] = value;
            Save();
        }
    }

    internal static class JsonNodeExtensions
    {
        public static JsonNode? SelectToken(this JsonNode node, string path)
        {
            var segments = path.Split('.', StringSplitOptions.RemoveEmptyEntries);
            JsonNode current = node;

            foreach (var segment in segments)
            {
                if (segment == "$")
                {
                    continue; // Skip the root token
                }
                if (current is JsonObject obj && obj.TryGetPropertyValue(segment, out var child))
                {
                    current = child;
                }
                else if (current is JsonArray arr && int.TryParse(segment, out var index) && index >= 0 && index < arr.Count)
                {
                    current = arr[index];
                }
                else
                {
                    return null;
                }
            }

            return current;
        }
    }
}
