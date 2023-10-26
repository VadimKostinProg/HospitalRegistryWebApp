using HospitalRegistry.Application.DTO;

namespace HospitalRegistry.Application.Helpers;

public static class TimeHelper
{
    public static bool TimeSlotsCross(TimeSlotDTO timeSlot1, TimeSlotDTO timeSlot2)
    {
        return timeSlot1.DayOfWeek == timeSlot2.DayOfWeek &&
               ((timeSlot1.StartTime > timeSlot2.StartTime && timeSlot1.StartTime < timeSlot2.EndTime) ||
                (timeSlot1.EndTime > timeSlot2.StartTime && timeSlot1.EndTime < timeSlot2.EndTime));
    }
}