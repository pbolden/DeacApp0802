using System;
using System.Collections.Generic;
using System.Linq;
using DeaconCCGManagement.DAL;
using DeaconCCGManagement.enums;
using DeaconCCGManagement.Models;


namespace DeaconCCGManagement.Services
{
    public class CalendarService : ServiceBase
    {
        private int _counter;
        private DateTime _startDate;

        public CalendarService(UnitOfWork uow) : base(uow)
        {
        }

        public IEnumerable<CalendarEvent> GetEventsForDateRange(DateTime startDate, DateTime endDate, string userEmail, bool getAll=false)
        {
            var birthdays = new Stack<CCGMember>();
            var weddingAnniversaries = new Stack<CCGMember>();
            var joinedZionAnniversaries = new Stack<CCGMember>();
            _startDate = startDate;

            var members = GetMembers(userEmail, getAll);

            // Push member objects to stacks
            foreach (var member in members)
            {
                if (member.BirthDate != null 
                    && IsDateWithinRange(member.BirthDate.Value, startDate, endDate))
                {
                    birthdays.Push(member);
                }
                if (member.AnniversaryDate != null
                    && IsDateWithinRange(member.AnniversaryDate.Value, startDate, endDate))
                {
                    weddingAnniversaries.Push(member);
                }
                if (member.DateJoinedZion != null
                    && IsDateWithinRange(member.DateJoinedZion.Value, startDate, endDate))
                {
                    joinedZionAnniversaries.Push(member);
                }
            }

            return AddEventsToCollection(birthdays, weddingAnniversaries, joinedZionAnniversaries);
        }

        private IEnumerable<CCGMember> GetMembers(string userEmail, bool getAll)
        {
            if (getAll)
            {
                return unitOfWork.MemberRepository.FindMembers();
            }
            var user = unitOfWork.AppUserRepository.FindUserByEmail(userEmail);
            var ccgId = user.CcgId;
            return ccgId == null ? new List<CCGMember>() 
                : unitOfWork.MemberRepository.FindMembers(m => m.CcgId == ccgId);
        }

        private IEnumerable<CalendarEvent> AddEventsToCollection(IEnumerable<CCGMember> birthdays, 
            IEnumerable<CCGMember> weddingAnniversaries, IEnumerable<CCGMember> joinZionAnniversaries)
        {

            // Add calendar events to new list
            var events = birthdays.Select(member => SetCalendarEventProps(member, CalendarEvents.Birthday)).ToList();
            events.AddRange(weddingAnniversaries.Select(member => SetCalendarEventProps(member, CalendarEvents.WeddingAnniversary)));
            events.AddRange(joinZionAnniversaries.Select(member => SetCalendarEventProps(member, CalendarEvents.JoinedZionAnniversary)));

            return events;
        }

        private CalendarEvent SetCalendarEventProps(CCGMember member, CalendarEvents calEvent)
        {
            _counter++;
            var eventDate = GetEventDate(calEvent, member);
            var eventName = GetEventName(calEvent);
            return new CalendarEvent
            {
                Id = _counter,
                Title = $"{eventName} - {member.LastName}",
                Description = $"{member.FirstName} {member.LastName}'s {eventName}",
                FirstName = member.FirstName,
                LastName = member.LastName,
                EventDate = eventDate,
                DateString = eventDate.ToLongDateString(),
                Url = $"/CcgMembers/Details/{member.Id}",
                PhoneNumber = member.PhoneNumber,
                CellPhoneNumber = member.CellPhoneNumber,
                EmailAddress = member.EmailAddress,

            };
        }

        private string GetEventName(CalendarEvents calEvent)
        {
            switch (calEvent)
            {
                case CalendarEvents.Birthday:
                    return "Birthday";
                case CalendarEvents.WeddingAnniversary:
                    return "Wedding Anniversary";
                case CalendarEvents.JoinedZionAnniversary:
                    return "Joined Zion Anniversary";
                default:
                    throw new ArgumentOutOfRangeException(nameof(calEvent), calEvent, null);
            }
        }

        private bool IsDateWithinRange(DateTime date, DateTime startDate, DateTime endDate)
        {
            return date.DayOfYear >= startDate.DayOfYear && date.DayOfYear <= endDate.DayOfYear;
        }

        private DateTime GetEventDate(CalendarEvents calEvent, CCGMember member)
        {
            DateTime eventDate = DateTime.Today; 
            switch (calEvent)
            {
                case CalendarEvents.Birthday:
                {
                    if (member.BirthDate != null) eventDate = member.BirthDate.Value;
                    break;
                }
                case CalendarEvents.WeddingAnniversary:
                {
                    if (member.AnniversaryDate != null) eventDate = member.AnniversaryDate.Value;
                    break;
                }
                case CalendarEvents.JoinedZionAnniversary:
                {
                        if (member.DateJoinedZion != null) eventDate = member.DateJoinedZion.Value;
                    break;
                }
                default:
                {
                    return new DateTime();
                }
            }

            // Updates year to current year for the event date. 
            // Eg., 5-25-1961 ==> 5-25-2017 (current year)
            return new DateTime(_startDate.Year, eventDate.Month, eventDate.Day);
        }
    }
}