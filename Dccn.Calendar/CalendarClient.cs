using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Exchange.WebServices.Data;

namespace Dccn.Calendar
{
    public class CalendarClient
    {
        private const int DefaultMaxEvents = 100;
        private static readonly PropertySet FolderProperties =
            new PropertySet(BasePropertySet.IdOnly, FolderSchema.DisplayName);

        private readonly ExchangeService _service;

        public CalendarClient(Uri exchangeUrl, string username, string password)
        {
            _service = new ExchangeService(ExchangeVersion.Exchange2013)
            {
                Credentials = new WebCredentials(username, password),
                Url = exchangeUrl
            };
        }

        public int MaxEvents { get; set; } = DefaultMaxEvents;

        public async Task<IEnumerable<Calendar>> ListCalendarsAsync()
        {
            var view = new FolderView(MaxEvents)
            {
                PropertySet = FolderProperties,
                Traversal = FolderTraversal.Deep
            };

            var folders = await _service.FindFolders(WellKnownFolderName.Root, view);
            return folders
                .Where(folder => folder is CalendarFolder)
                .Select(folder => new Calendar(this, (CalendarFolder) folder));
        }

        public async Task<Calendar> GetCalendarByIdAsync(string id)
        {
            return new Calendar(this, await CalendarFolder.Bind(_service, new FolderId(id), FolderProperties));
        }

        public async Task<(bool Success, Calendar Calendar)> TryGetCalendarByIdAsync(string id)
        {
            try
            {
                return (true, await GetCalendarByIdAsync(id));
            }
            catch (ServiceResponseException e) when (e.ErrorCode == ServiceError.ErrorItemNotFound || e.ErrorCode == ServiceError.ErrorInvalidIdMalformed)
            {
                return (false, null);
            }
        }
    }
}
