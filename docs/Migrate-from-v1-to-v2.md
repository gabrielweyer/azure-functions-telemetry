# Migrate from v1 to v2

The goal for v2 is to make the customisation configurable using application settings. To configure v1, you had to modify the code and redeploy. Two fluent configurations were obsoleted and replaced by application settings:

- WithServiceBusTriggerFilter
- WithHealthRequestFilter

## Service Bus trigger filter

`WithServiceBusTriggerFilter()` has been deprecated. It has been replaced by the application setting `ApplicationInsights:DiscardServiceBusTrigger`.

The default value is `false`, meaning that the Service Bus trigger traces will not be discarded.

You can either set it to `true` or `false`.

## Health request filter

`WithHealthRequestFilter(string healthCheckFunctionName)` has been deprecated. It has been replaced by the application setting `ApplicationInsights:HealthCheckFunctionName`.

The default value is `null`, meaning that no request will be discaded.

You can provide it a Function name if you want to discard a specific Function's requests.
