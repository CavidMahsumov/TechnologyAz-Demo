using AutoMapper;
using MediatR;
using OrderService.Application.Features.Queries.ViewModels;
using OrderService.Application.Interfaces.Repositories;
using OrderService.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderService.Application.Features.Queries.GetOrderDetailById
{
    public class GetOrderDetailsQueryHandler : IRequestHandler<GetOrderDetailsQuery, OrderDetailViewModel>
    {
        IOrderRepository _orderRepository;
        private readonly IMapper mapper;

        public GetOrderDetailsQueryHandler(IOrderRepository orderrepository, IMapper mapper)
        {
            this._orderRepository = orderrepository ??throw new ArgumentNullException(nameof(orderrepository));  
            this.mapper = mapper;
        }

        public async Task<OrderDetailViewModel> Handle(GetOrderDetailsQuery request, CancellationToken cancellationToken)
        {
            var order = await _orderRepository.GetByIdAsync(request.OrderId, i => i.OrderItems);
            var result=mapper.Map<OrderDetailViewModel>(order);
            return result;
        }
    }
}
