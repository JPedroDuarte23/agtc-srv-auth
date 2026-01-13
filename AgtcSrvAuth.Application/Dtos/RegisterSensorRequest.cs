using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AgtcSrvAuth.Domain.Enums;

namespace AgtcSrvAuth.Application.Dtos;

public record RegisterSensorRequest(
    SensorType SensorType,
    string Serial,
    Guid FieldId,
    DateTime CreatedAt
);