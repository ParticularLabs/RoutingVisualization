# Routing Visualization

Using audit data stored in ServiceControl to construct a visualization of endpoints and the messages being routed between them.

To use this tool you need ServiceControl installed and collecting audit data. [Make sure the Raven database is exposed](http://docs.particular.net/servicecontrol/use-ravendb-studio) and update the `ServiceControl/RavenAddress` setting in `RoutingVisualization.exe.config`. Run the tool:

`RoutingVisualziation.exe <filename.dgml>`

This will generate a [DGML](https://en.wikipedia.org/wiki/DGML) file which can be opened and manipulated in Visual Studio. If you do not specify a filename then `route-graph.dgml` is the default.

## Notes

* This tool will read every Audit message in your ServiceControl database. This will have a performance impact on your ServiceControl server and is not recommended in a production environment.
* As the tool is using data in the audit log to generate the visualization only successfully processed messages within the retention period are being included. Any messages that failed to be processed or which have not been sent during the retention period will not be shown. Any events that were published but had no subscribers are also not shown.
* The tool shows all activity during the retention period. If a handler is moved during this period then old and new links will be shown in the same diagram. Any message types which have been renamed during the retention period will show old and new data.
