using vILT.Domain;
using Microsoft.Extensions.Configuration;
using Microsoft.Graph;
using Microsoft.Graph.Auth;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MicrosoftGraphViltService
{
    public class MicrosoftViltSessionService : IViltSessionService
    {
        public GraphServiceClient confClient;
        //private readonly IConfiguration _config;
        private readonly string ServiceAccountEmail;
        private OnlineMeetingConfig _meetingConfig;

        public MicrosoftViltSessionService(IConfiguration config)
        {
            ServiceAccountEmail = config["ServiceMailboxAccountEmail"]; //From appsettings.json
            _meetingConfig = new OnlineMeetingConfig();
            config.GetSection("OnlineMeetingConfig").Bind(_meetingConfig);
             
            CreateClientCredentialGraphClient(config);

        }
        /// <summary>
        /// Initializes GraphClient using the confidential Client Credentials authoriztion flow.
        /// </summary>
        private void CreateClientCredentialGraphClient(IConfiguration config)
        {
            //https://docs.microsoft.com/en-us/graph/sdks/choose-authentication-providers?tabs=CS
            IConfidentialClientApplication confidentialClientApplication = ConfidentialClientApplicationBuilder
                .Create(config["ClientId"]) //From appsettings.json
                .WithTenantId(config["TenantId"]) //From appsettings.json
                .WithClientSecret(config["ClientSecret"]) //This fetches the secret from Azure Key Vault (see Program.cs)
                .Build();

            ClientCredentialProvider authProvider = new ClientCredentialProvider(confidentialClientApplication);
            confClient = new GraphServiceClient(authProvider);
        }

        /// <summary>
        /// Creates Teams online meeting enabled Calendar event
        /// </summary>
        /// <param name="request">The Out-of-the-box vILT SessionRequest class</param>
        /// <returns>Graph.Event (https://docs.microsoft.com/en-us/graph/api/resources/event?view=graph-rest-1.0) </returns>
        public async Task CreateSessionAsync(SessionRequest request)
        {
            var newEvent = new Event
            {
                IsOnlineMeeting = true,
                IsReminderOn = _meetingConfig.IsReminderOn,
                ResponseRequested = _meetingConfig.ResponseRequested,
                OnlineMeetingProvider = _meetingConfig.ProviderType(),
                Start = DateTimeTimeZone.FromDateTimeOffset(DateTimeOffset.Parse(request.DateBegin)),
                End = DateTimeTimeZone.FromDateTimeOffset(DateTimeOffset.Parse(request.DateEnd)),
                Subject = request.Title,
                Body = new ItemBody
                {
                    ContentType = _meetingConfig.BodyType(),
                    Content = request.Description
                },
                Organizer = new Recipient
                {
                    EmailAddress = new EmailAddress
                    {
                        Address = ServiceAccountEmail
                    }
                },
                Location = new Location
                {
                    //ToDo: consider a SchemaExtension (https://docs.microsoft.com/en-us/graph/extensibility-overview)
                    DisplayName = request.SessionId
                },

            };

            if (_meetingConfig.CreatorIsAttendee)
            {
                newEvent.Attendees = new List<Attendee> //ToDo: consider whether this is desirable. This is not the Cornerstone session's "instructor"
                    {
                        new Attendee
                        {
                            EmailAddress = new EmailAddress { Address = request.CreatorEmail }
                        }
                    };
            }

            await confClient.Users[ServiceAccountEmail].Events.Request().AddAsync(newEvent);
            return;
        }

        /// <summary>
        /// Adds Attendee to event if they aren't already listed.
        /// </summary>
        /// <param name="sessionId"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        public async Task AddAttendeeAsync(string sessionId, string email)
        {
            var @event = await GetMeetingByExternalIdAsync(sessionId);
            if (string.IsNullOrEmpty(email)) throw new ArgumentNullException("email required");

            Attendee attendee = new Attendee
            {
                EmailAddress = new EmailAddress { Address = email }
            };

            var attendees = @event.Attendees?.ToList();

            if (attendees == null) attendees = new List<Attendee>();
            if (attendees.Any() && attendees.Any(a => a.EmailAddress.Address.Equals(email, StringComparison.CurrentCultureIgnoreCase)))
            {
                return;
            }

            attendees.Add(attendee);

            @event.Attendees = attendees;

            @event =
                    await confClient.Users[ServiceAccountEmail].Events[@event.Id]
                    .Request().UpdateAsync(@event);

            return;
        }

        /// <summary>
        /// Returns Session information 
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Session object (JoinURL and list of Attendees if present)</returns>
        public async Task<ViltSession> GetSessionAsync(string id)
        {
            var meeting = await GetMeetingForSessionConversionAsync(id);

            var session = new ViltSession
            {
                Id = id,
                JoinUrl = meeting.OnlineMeeting.JoinUrl,
                Attendees = meeting.Attendees != null && meeting.Attendees.Any() 
                    ? meeting.Attendees.Select(a => new ViltSessionAttendee { Email = a.EmailAddress.Address }).ToList()
                    : new List<ViltSessionAttendee>()
            };

            return session;
        }

        private async Task<Event> GetMeetingForSessionConversionAsync(string id)
        {
            var meetingResult =
                await confClient.Users[ServiceAccountEmail].Events
                .Request(MeetingQueryOptions(id)).Select(m => new { m.OnlineMeeting, m.Attendees })
                .GetAsync();

            if (meetingResult == null || !meetingResult.Any()) 
                throw new SessionNotFoundException($"no session found for session Id: {id}");
            
            if (meetingResult.Count > 1) 
                throw new Exception($"there were multiple results found for {id} when only one was expected");

            return meetingResult.First();

        }

        /// <summary>
        /// Gets the Event via Graph query
        /// </summary>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        private async Task<Event> GetMeetingByExternalIdAsync(string sessionId)
        {

            var meeting =
                await confClient.Users[ServiceAccountEmail].Events
                .Request(MeetingQueryOptions(sessionId))
                .GetAsync();

            if (meeting == null || !meeting.Any()) throw new SessionNotFoundException($"no session found for session Id: {sessionId}");

            return meeting.First();
        }

        private List<QueryOption> MeetingQueryOptions(string sessionId)
        {
            return new List<QueryOption>
            {
                //ToDo: consider SchemaExtensions (https://docs.microsoft.com/en-us/graph/extensibility-overview)
                new QueryOption("filter", $"location/displayName eq '{sessionId}'")
            };

        }

        /// <summary>
        /// checks for changes in start/end values as well as title (subject)
        /// </summary>
        /// <param name="sessionId"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task UpdateSessionAsync(string sessionId, SessionRequest request)
        {
            var @event = await GetMeetingByExternalIdAsync(sessionId);
            if (@event == null) throw new SessionNotFoundException($"no session found for session Id: {sessionId}");

            var newBegin = DateTimeTimeZone.FromDateTimeOffset(DateTimeOffset.Parse(request.DateBegin));
            var newEnd = DateTimeTimeZone.FromDateTimeOffset(DateTimeOffset.Parse(request.DateEnd));
            if (newBegin != @event.Start) @event.Start = newBegin;
            if (newEnd != @event.End) @event.End = newEnd;


            if (@event.Subject != request.Title) @event.Subject = request.Title;
            //ToDo: consider appending new message to top of body if diferent?

            await confClient.Users[ServiceAccountEmail].Events[@event.Id]
            .Request().UpdateAsync(@event);

            return;

        }
        /// <summary>
        /// confirms that the provided e-mail address is valid.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<bool> AddInstructorAsync(AddInstructorRequest request)
        {
            //This isn't supposed to add instructor to our team meeting so we're assuming they just want to verify that the user exists before adding on the Cornerstone side.
            return await UserExists(request.Email);
        }

        /// <summary>
        /// confirms that the provided new e-mail address is valid.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<bool> UpdateInstructorAsync(UpdateInstructorRequest request)
        {
            //This isn't supposed to actually update our team meeting so we're assuming they just want to verify that the user exists before adding on the Cornerstone side.
            return await UserExists(request.NewEmail);
        }

        private async Task<bool> UserExists(string email)
        {
            return (await confClient.Users[email].Request().Select(u => new
            {
                u.DisplayName
            }).GetAsync()) != null;
        }


    }
}
