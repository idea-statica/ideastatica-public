﻿using CommandLine;
using IdeaStatiCa.CheckbotPlugin.Models;
using IdeaStatiCa.CheckbotPlugin.Services;
using IdeaStatiCa.PluginRunner;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

await new Parser(x => x.GetoptMode = true)
	.ParseArguments<Arguments>(args)
	.WithParsedAsync(Run);

static async Task Run(Arguments arguments)
{
	string path = GetPluginPath(arguments);
	ChangeCurrentDirectory(path);

	PluginLaunchRequest pluginLaunchRequest = new(
		arguments.Path,
		arguments.ClassName);

	CancellationTokenSource cancellationTokenSource = new();

	PluginRunner pluginRunner = PluginRunner.Create(arguments.Port, arguments.CommunicationId);
	PluginLaunchResponse pluginLaunchResponse = await pluginRunner.Run(pluginLaunchRequest);

	Console.WriteLine("Starting plugin");

	pluginLaunchRequest.Plugin.Entrypoint(pluginRunner);

	pluginRunner.GetService<IEventService>().Subscribe(x =>
	{
		if (x is EventPluginStop)
		{
			cancellationTokenSource.Cancel();
		}
	});

	await pluginRunner.Ready();

	await Task.Delay(-1, cancellationTokenSource.Token);

	Console.WriteLine("Stopping plugin");
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