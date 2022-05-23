interface IPlugin
{
    PluginInfo PluginInfo { get; }

    void Entrypoint(IServiceProvider serviceProvider);
}