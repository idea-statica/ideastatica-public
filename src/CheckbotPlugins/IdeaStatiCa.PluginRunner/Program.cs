using CommandLine;
using IdeaStatiCa.PluginRunner;
using System;
using System.IO;
using System.Reflection;

Parser.Default.ParseArguments<Arguments>(args)
	.WithParsed(Run);

static void Run(Arguments arguments)
{
	InitPlugin(arguments);
}

static void InitPlugin(Arguments arguments)
{
	string path = GetPluginPath(arguments);
	ChangeCurrentDirectory(path);

	Assembly assembly = Assembly.LoadFrom(path);
	Type entryPointClass = PluginEntryPoint.GetEntryPointClass(assembly, arguments.ClassName);

	PluginEntryPoint.GetInstance(entryPointClass);
}

static string GetPluginPath(Arguments arguments)
{
	string path = Path.GetFullPath(arguments.Path);

	if (!File.Exists(path))
	{
		throw new FileNotFoundException();
	}

	return path;
}

static void ChangeCurrentDirectory(string path)
{
	string? directory = Path.GetDirectoryName(path);

	if (directory is not null)
	{
		Directory.SetCurrentDirectory(directory);
	}
}