# Cornerstone Teams Scheduler Api
This API is an ASP Core 3.1 Web API designed to run as an Azure App Service. 
It allows Cornerstone LMS (CSOD) to connect via Basic Auth to create/update online meetings in Microsoft Teams (or Skype) for virtual instructor lead training (vILT)

This is a sanitized repository without development commit history and without organization-specific keys and info.

## Azure Services Information and Dependencies
 - A service cloud account is used as a resource account for its mailbox. The mailbox's calendar acts as a repository for the online meetings. Username and password (if needed) are stored in the Azure Key Vault mentioned in this documentation.
 - App secrets (Client Secret, Basic Auth Username/Password) should be saved in Azure Key Vault. The app uses [Visual Studio's "Connected Services" to connect to Key Vault as if it were a secondary config provider](https://docs.microsoft.com/en-us/azure/key-vault/general/vs-key-vault-add-connected-service).
 - App needs to be registered in Azure with "Calendars.ReadWrite" Application level permissions.
 - App is running on an App Service.

## Cornerstone
 - The methods in the API are modified from the [original vILT specs](https://app.swaggerhub.com/apis/csodedge/vILT-Connector/1.0.0#) with some minor tweaks including conversion to async threading.
 - Note: When creating a new meeting, Cornerstone sends, as the "CreatorEmail" value, the actual session creator e-mail address and *not*, as we had hoped, the instructor email address.

## Microsoft Graph
 - Authentication to Microsoft Graph follows the Confidential [Client Credentials flow](https://docs.microsoft.com/en-us/azure/active-directory/develop/msal-authentication-flows#client-credentials).
 - Creating [Events enabled as Online Meetings](https://docs.microsoft.com/en-us/graph/api/calendar-post-events?view=graph-rest-1.0&tabs=http#example-2-create-and-enable-an-event-as-an-online-meeting)
 - Note: Cornerstone sends it's own GUID ("Session ID") to this connector when creating and updating meetings. You can certainly make use of "[Schema Extensions](https://docs.microsoft.com/en-us/graph/extensibility-overview)", but this app simply stores the Session Id in the event's location/displayName field.

