# APIGateway service

## Architectural overview

The main functionality of an Ocelot API Gateway is to take incoming HTTP requests and forward them on to a downstream service, currently as another HTTP request.
Ocelot describes the routing of one request to another as a ReRoute.

Also we could leverage Route authentication feature to validate JWT and pass into inner network only authenticated requests where required with User information in Claims

For `Login` call we pass RemoteIpAddress into `Identity` service to track customers geo information.


## Using RSA for JWT

To add more security to JWT solution we'll use Asymmetric algorithm to validate token signature.
[Details in Identity service article](../Services/Identity/README.md)


Add new endpoints
webhook for strip payment


