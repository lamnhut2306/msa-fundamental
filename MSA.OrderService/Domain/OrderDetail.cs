using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MSA.Common.Contracts.Domain;

namespace MSA.OrderService.Domain
{
    public class OrderDetail : IEntity
    {
        public Guid Id { get; init; }
        public Guid ProductId { get; set; }
    }
}