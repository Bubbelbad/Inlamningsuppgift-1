﻿using Domain.Model;
using MediatR;

namespace Application.Queries.UserQueries
{
    public class GetAllUsersQuery() : IRequest<OperationResult<List<User>>>
    {

    }
}
