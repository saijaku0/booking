using Booking.Domain.Exceptions;

namespace Booking.Domain.Entities
{
    public class DoctorScheduleConfig
    {
        public Guid Id { get; private set; }
        public Guid DoctorId { get; private set; }

        public TimeSpan DayStart { get; private set; }
        public TimeSpan DayEnd { get; private set; }
        public TimeSpan LunchStart { get; private set; }
        public TimeSpan LunchEnd { get; private set; }

        public IReadOnlyCollection<int> WorkingDays => _workingDays.AsReadOnly();
        private readonly List<int> _workingDays = new();

        public int SlotDurationMinutes { get; private set; }
        public int BufferMinutes { get; private set; }
        public int MinHoursInAdvance { get; private set; }
        public int MaxDaysInAdvance { get; private set; }

        private DoctorScheduleConfig() { } 

        public DoctorScheduleConfig(Guid doctorId)
        {
            Id = Guid.NewGuid();
            DoctorId = doctorId;

            SetWorkingHours(
                new TimeSpan(9, 0, 0),
                new TimeSpan(18, 0, 0),
                new TimeSpan(13, 0, 0),
                new TimeSpan(14, 0, 0));

            SetWorkingDays(new[] { 1, 2, 3, 4, 5 });
            SetSlotSettings(30, 5);
            SetBookingLimits(2, 90);
        }

        public void SetWorkingHours(
            TimeSpan dayStart,
            TimeSpan dayEnd,
            TimeSpan lunchStart,
            TimeSpan lunchEnd)
        {
            if (dayStart >= dayEnd)
                throw new DomainException("Start befor the end");

            if (!(dayStart < lunchStart &&
                  lunchStart < lunchEnd &&
                  lunchEnd < dayEnd))
                throw new DomainException("Incorect brake");

            DayStart = dayStart;
            DayEnd = dayEnd;
            LunchStart = lunchStart;
            LunchEnd = lunchEnd;
        }

        public void SetWorkingDays(IEnumerable<int> days)
        {
            var normalized = days
                .Distinct()
                .OrderBy(x => x)
                .ToList();

            if (!normalized.Any())
                throw new DomainException("Must be at least one work day");

            if (normalized.Any(d => d < 1 || d > 7))
                throw new DomainException("Work day in range 1-7");

            _workingDays.Clear();
            _workingDays.AddRange(normalized);
        }

        public void SetSlotSettings(int slotDurationMinutes, int bufferMinutes)
        {
            if (slotDurationMinutes <= 0)
                throw new DomainException("Slot duration must be > 0");

            if (bufferMinutes < 0)
                throw new DomainException("Buffer cannot be negative");

            SlotDurationMinutes = slotDurationMinutes;
            BufferMinutes = bufferMinutes;
        }

        public void SetBookingLimits(int minHoursInAdvance, int maxDaysInAdvance)
        {
            if (minHoursInAdvance < 0)
                throw new DomainException("Min of hours in advance cannot be negative");

            if (maxDaysInAdvance <= 0)
                throw new DomainException("Max days in advance must be > 0");

            MinHoursInAdvance = minHoursInAdvance;
            MaxDaysInAdvance = maxDaysInAdvance;
        }

        public bool IsWorkingDay(DateOnly date)
        {
            var dayOfWeek = (int)date.DayOfWeek;
            dayOfWeek = dayOfWeek == 0 ? 7 : dayOfWeek; 
            return _workingDays.Contains(dayOfWeek);
        }

        public bool IsWithinWorkingHours(TimeSpan time)
        {
            if (time < DayStart || time >= DayEnd)
                return false;

            if (time >= LunchStart && time < LunchEnd)
                return false;

            return true;
        }

        public bool CanBook(DateTime appointmentDateTime, DateTime now)
        {
            if (!IsWorkingDay(DateOnly.FromDateTime(appointmentDateTime)))
                return false;

            if (!IsWithinWorkingHours(appointmentDateTime.TimeOfDay))
                return false;

            if (appointmentDateTime < now.AddHours(MinHoursInAdvance))
                return false;

            if (appointmentDateTime > now.AddDays(MaxDaysInAdvance))
                return false;

            return true;
        }
    }
}
