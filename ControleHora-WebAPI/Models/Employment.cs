using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;

namespace ControleHora_WebAPI.Models
{
    public enum Position
    {
        Junior = 0,
        Experienced = 1,
        Senior = 2
    }
    public enum Role
    {
        Developer = 0,
        Tester = 1,
        TechnicalLeader = 2,
        ProjectLeader = 3,
        ProjectManager = 4
    }

    public enum HoursWeek
    {
        FullTimeExtra = 44,
        FullTime = 40,
        PartTime = 30,
        PartTimeLesser = 20
    }
}