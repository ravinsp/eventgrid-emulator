# Azure Event Grid Emulator (Unofficial)

Azure Event Grid Emulator is intended to be a tool to help test Event Grid based interactions between your application and Azure functions locally. This is not an official feature-complete emulator. This tool should become obsolete if/when Microsoft introduces an Event Grid emulator of their own.

[Azure Event Grid](https://azure.microsoft.com/en-us/services/event-grid/) is a relatively new service (as of Nov 2017) which you can use to communicate between your application components. A common use case would be one of your application components publishes an event to the EventGrid and an Azure Function would pick it up and process it.

Microsoft provides the ability to [run your Azure Functions locally](https://docs.microsoft.com/en-us/azure/azure-functions/functions-run-local). Unfortunately there's no local emulator for running Event Grid locally (yet). Unlike Azure Service Bus (which relies on client polling), Azure Event Grid is a push-based service meaning it'll be very difficult to get the Event Grid on the cloud to invoke your local Azure Function.

##### Visit [Releases](https://github.com/ravinsp/eventgrid-emulator/releases) to download

### Features

 * No code changes required to your existing application code.
 * Supports hosting of multiple topics and multiple subscriptions on each topic.
 * Supports event type filter.
 * Supports event subject prefix/suffix filters.
 * Define your subscriptions and endpoints using a simple JSON configuration.
 * Supports HTTP endpoint invocation (compatible with [EventGridTrigger Azure Function](https://github.com/Azure/azure-functions-eventgrid-extension) as well).
 * Can be easily extended to implement different invocation strategies in the future. (However, I intentionally avoided using a DI container to keep things simple)

### Limitations

 * Does not support Labels yet.
 * Does not support Event expiration, delivery guarantees and retry policies (see [here](https://docs.microsoft.com/en-us/azure/event-grid/delivery-and-retry) for real Event Grid behaviour).
 * The subscriber endpoint has to be alive in order to deliver the event. The emulator tries to dispatch events immediately and simply logs any failures.
 * Strictly limited for developer testing. Not intended for any sort of production workloads.

### Instructions

 * Download the release package from [Releases](https://github.com/ravinsp/eventgrid-emulator/releases).
 * Extract the package in your machine.
 * Edit the `emulator-config.json` to reflect your topics/endpoints and run the `EventGridEmulator.exe` (Run as administrator)
 * When your application is publishing events to the Event Grid, use `http://localhost:5000/<TopicName>` as the Event Grid endpoint.
 * **_Topic names, Event types and Subject filters are Case Sensitive_**

### Sample Config
The following sample indicates a config for following setup:
 * Event Grid Emulator is running on port 5000 in localhost.
 * The emulator is hosting an Event Grid topic named "WeatherEvents".
 * "WeatherEvents" topic has a subscription named "MyFuncSubscription1" with matching criteria for event types "Rain" and "Sunny".
 * "MyFuncSubscription1" subscription endpoint is set to local Azure function app running on port 7071 and to invoke "MyFuncName" EventGridTrigger function. (If this is a HttpTrigger function, use the http trigger url instead)

```javascript
{
  "port": 5000,
  "topics": [
    {
      "name": "WeatherEvents",
      "subscriptions": [
        {
          "name": "MyFuncSubscription1",
          "eventTypes": [ "Rain", "Sunny" ], /*Leave null to match all event types*/
          "SubjectBeginsWith": "",
          "SubjectEndsWith": "",
          "endpointUrl": "http://localhost:7071/admin/extensions/EventGridExtensionConfig?functionName=MyFuncName",
          "dispatchStrategy": "DefaultHttpStrategy"
        }
      ]
    }
  ],
  "dispatchStrategies": [
    {
      "name": "DefaultHttpStrategy",
      "type": "EventGridEmulator.Logic.DispatchStrategies.DefaultHttpStrategy"
    }
  ]
}
```