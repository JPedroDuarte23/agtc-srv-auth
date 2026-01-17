using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AgtcSrvAuth.Domain.Enums;

namespace AgtcSrvAuth.Application.Dtos;

public record RegisterSensorRequest(
    [property: EnumDataType(typeof(SensorType), ErrorMessage = "Tipo de sensor inválido. Valores aceitos: Temperatura, Umidade, Pressao.")]
    SensorType SensorType,

    [property: Required(ErrorMessage = "O Serial é obrigatório.")]
    [property: StringLength(50, MinimumLength = 3, ErrorMessage = "O Serial deve ter entre 3 e 50 caracteres.")]
    string Serial,

    [property: Required(ErrorMessage = "O ID do talhão é obrigatório.")]
    Guid FieldId,
    [property: Required(ErrorMessage = "A data de criação é obrigatória.")]
    DateTime CreatedAt
);