using MediatR;
using SFA.DAS.ApprenticeCommitments.Application.DomainEvents;
using SFA.DAS.ApprenticeCommitments.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable

namespace SFA.DAS.ApprenticeCommitments.Data.Models
{

    public abstract class Entity
    {
        public List<INotification> DomainEvents { get; }
            = new List<INotification>();

        public void AddDomainEvent(INotification eventItem)
        {
            DomainEvents.Add(eventItem);
        }

        public void RemoveDomainEvent(INotification eventItem)
        {
            DomainEvents.Remove(eventItem);
        }
    }
}