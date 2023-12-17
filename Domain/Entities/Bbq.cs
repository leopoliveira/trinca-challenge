using System;
using System.Collections.Generic;
using System.Linq;

using Domain.Events;

namespace Domain.Entities
{
    public class Bbq : AggregateRoot
    {
        private const int MINIMUM_NUMBER_OF_PEOPLE_TO_HAPPEN = 7;
        private int _numberOfConfirmedPeople = 0;

        public string Reason { get; set; }
        public BbqStatus Status { get; set; }
        public DateTime Date { get; set; }
        public bool IsTrincasPaying { get; set; }
        public List<BbqConfirmedPerson> ConfirmedPeople { get; set; }
        public BbqShoppingList ShoppingList { get; set; }

        public void When(ThereIsSomeoneElseInTheMood @event)
        {
            Id = @event.Id.ToString();
            Date = @event.Date;
            Reason = @event.Reason;
            Status = BbqStatus.New;
            ConfirmedPeople = new List<BbqConfirmedPerson>();
            ShoppingList = new BbqShoppingList();
        }

        public void When(BbqStatusUpdated @event)
        {
            if (@event.GonnaHappen)
                Status = BbqStatus.PendingConfirmations;
            else
                Status = BbqStatus.ItsNotGonnaHappen;

            if (@event.TrincaWillPay)
                IsTrincasPaying = true;
        }

        public void When(InviteWasAccepted @event)
        {
            // Não permite que a pessoa aceite o convite mais de uma vez.
            if (PersonIsConfirmed(@event.PersonId))
            {
                return;
            }

            ConfirmedPeople.Add(new BbqConfirmedPerson { PersonId = @event.PersonId, IsVeg = @event.IsVeg });

            UpdateNumberOfConfirmedPeopleAndBbqStatus();

            ShoppingList.AddItemsToShoppingList(@event.IsVeg);
        }

        public void When(InviteWasDeclined @event)
        {
            if (PersonIsConfirmed(@event.PersonId))
            {
                var confirmedPerson = GetConfirmedPersonById(@event.PersonId);

                ConfirmedPeople.Remove(confirmedPerson);

                UpdateNumberOfConfirmedPeopleAndBbqStatus();

                ShoppingList.RemoveItemsFromShoppingList(confirmedPerson.IsVeg);
            }
        }

        public object TakeSnapshot()
        {
            return new
            {
                Id,
                Date,
                IsTrincasPaying,
                Status = Status.ToString()
            };
        }

        private bool PersonIsConfirmed(string personId)
        {
            return ConfirmedPeople.Any(people => people.PersonId == personId);
        }

        private void UpdateNumberOfConfirmedPeopleAndBbqStatus()
        {
            _numberOfConfirmedPeople = ConfirmedPeople.Count;

            Status = _numberOfConfirmedPeople >= MINIMUM_NUMBER_OF_PEOPLE_TO_HAPPEN ?
                     BbqStatus.Confirmed :
                     BbqStatus.PendingConfirmations;
        }

        private BbqConfirmedPerson GetConfirmedPersonById(string personId)
        {
            return ConfirmedPeople.SingleOrDefault(people => people.PersonId == personId);
        }

    }

}
