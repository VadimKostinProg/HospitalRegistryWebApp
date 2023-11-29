using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace HospitalRegistry.Application.Enums;

public enum Specialty
{
    [EnumMember(Value = "Dermatologist")] Dermatologist,
    [EnumMember(Value = "Cardiologist")] Cardiologist,
    [EnumMember(Value = "Neurologist")] Neurologist,
    [EnumMember(Value = "Psychiatrist")] Psychiatrist,
    [EnumMember(Value = "Ophthalmologist")] Ophthalmologist,
    [EnumMember(Value = "Urologist")] Urologist,
    [EnumMember(Value = "Gastroenterologist")] Gastroenterologist,
    [EnumMember(Value = "Allergist")] Allergist,
    [EnumMember(Value = "Radiologist")] Radiologist,
    [EnumMember(Value = "Endocrinologist")] Endocrinologist,
    [EnumMember(Value = "Surgeon")] Surgeon
}