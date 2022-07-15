## Plugin Lifespan

- plugin is started on-demand
  - it is started when the user wants to use the plugin
  - after that the plugin will keep running in the background

- when plugin is to be stopped it receives a *EventPluginStop* event
  - if plugin does not stop within 30 seconds it is terminated
  - this event can be cause either by user closing Checkbot or disabling of plugin

### Check Application Workflow

- start the plugin if it's not running

- plugin receives EventOperationBegin event
  - from now on the user can't change data in Checkbot
- plugin receives EventOpenCheckApplication with id and type of the object
  - e.g. id of a connection that user opened
- Checkbot waits for plugin to call OperationFinished or OperationDiscard
  - either unblocks Checkbot

