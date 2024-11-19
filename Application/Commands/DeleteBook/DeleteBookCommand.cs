﻿using Application.Dtos;
using Domain.Model;
using MediatR;

namespace Application.Commands.DeleteBook
{
    public class DeleteBookCommand(Guid id) : IRequest<bool>
    {
        public Guid bookIdToDelete { get; } = id; 
    }
}