using IniParser;
using IniParser.Exceptions;
using IniParser.Model;
using System;
using System.IO;
using System.Text;

namespace RstabToIdeaPluginInstall
{
	internal class RSTABConfig
	{
		private const string ExternalModulesSection = "ExternalModules";
		private const string ModuleName = "ModuleName";
		private const string ModuleDescription = "ModuleDescription";
		private const string ModuleClassName = "ModuleClassName";

		private readonly IniData _data;
		private readonly string _path;
		private bool _configMalformed = false;

		public RSTABConfig(string path)
		{
			_path = path;

			if (File.Exists(path))
			{
				try
				{
					_data = new FileIniDataParser().ReadFile(path);
					return;
				}
				catch (ParsingException)
				{
					_configMalformed = true;
				}
			}

			_data = new IniData();
			_data.Sections.AddSection(ExternalModulesSection);
		}

		public void Write()
		{
			if (_configMalformed)
			{
				BackupFile(_path);
				_configMalformed = true;
			}

			new FileIniDataParser().WriteFile(_path, _data, new UTF8Encoding(false));
		}

		public bool Contains(string className)
		{
			foreach (KeyData keyData in _data[ExternalModulesSection])
			{
				if (keyData.KeyName.StartsWith(ModuleClassName, StringComparison.OrdinalIgnoreCase))
				{
					if (keyData.Value == className)
					{
						return true;
					}
				}
			}

			return false;
		}

		internal void Remove(string className)
		{
			int index = -1;
			KeyDataCollection externalModules = _data[ExternalModulesSection];

			foreach (KeyData keyData in externalModules)
			{
				if (keyData.KeyName.StartsWith(ModuleClassName, StringComparison.OrdinalIgnoreCase))
				{
					if (keyData.Value != className)
					{
						continue;
					}

					index = int.Parse(keyData.KeyName.Substring(ModuleClassName.Length));

					break;
				}
			}

			if (index == -1)
			{
				return;
			}

			externalModules.RemoveKey(ModuleName + index.ToString());
			externalModules.RemoveKey(ModuleDescription + index.ToString());
			externalModules.RemoveKey(ModuleClassName + index.ToString());
		}

		public void Add(string name, string description, string className)
		{
			int index = 1;
			KeyDataCollection externalModules = _data[ExternalModulesSection];

			foreach (KeyData keyData in externalModules)
			{
				if (TryMatchValue(keyData, ModuleClassName, className, ref index) ||
					TryMatchValue(keyData, ModuleName, name, ref index))
				{
					break;
				}
			}

			externalModules[ModuleName + index.ToString()] = name;
			externalModules[ModuleDescription + index.ToString()] = description;
			externalModules[ModuleClassName + index.ToString()] = className;
		}

		private bool TryMatchValue(KeyData keyData, string keyName, string expectedValue, ref int index)
		{
			if (keyData.KeyName.StartsWith(keyName, StringComparison.OrdinalIgnoreCase))
			{
				int moduleIndex = int.Parse(keyData.KeyName.Substring(keyName.Length));

				if (keyData.Value == expectedValue)
				{
					index = moduleIndex;
					return true;
				}
				else
				{
					index = Math.Max(index, moduleIndex + 1);
				}
			}

			return false;
		}

		private void BackupFile(string path)
		{
			string bakPath = path + ".bak";
			int i = 1;

			while (File.Exists(bakPath) && i < 100)
			{
				bakPath = path + $".bak{i}";
				i++;
			}

			File.Copy(path, bakPath, true);
		}
	}
}