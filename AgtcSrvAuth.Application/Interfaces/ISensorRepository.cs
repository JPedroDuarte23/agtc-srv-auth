using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AgtcSrvAuth.Domain.Models;

namespace AgtcSrvAuth.Application.Interfaces;

public interface ISensorRepository
{
    Task CreateAsync(Sensor sensor);
}
