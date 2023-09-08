using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MSA.Common.Contracts.Domain
{
    public interface IEntity
    {
        Guid Id { get; init;}
    }
}