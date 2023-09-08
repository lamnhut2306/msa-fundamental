using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MSA.Common.Contracts.Domain;


namespace MSA.OrderService.Domain
{
    public class Order : IEntity
    {
        public Guid Id { get; init; }
        public Guid UserId { get; set; }
        public string OrderStatus { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}