using IntegrationPatterns.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationPatterns.Splitter
{
	class SplitterConsole
	{
		private static readonly MessageQueue SPLITTER_QUEUE = new MessageQueue(@".\private$\splitter");
		private static readonly MessageQueue PASSENGER_INFO_QUEUE = new MessageQueue(@".\private$\passenger");
		private static readonly MessageQueue LUGGAGE_QUEUE = new MessageQueue(@".\private$\luggage");

		static void Main(string[] args)
		{
			VerifyQueuesExistAndEmpty();

			var splitter = new Splitter(SPLITTER_QUEUE, PASSENGER_INFO_QUEUE, LUGGAGE_QUEUE);
			splitter.BeginReceive();

			QueueReader.BeginReceive(PASSENGER_INFO_QUEUE, OnPassengerMessageReceived);
			QueueReader.BeginReceive(LUGGAGE_QUEUE, OnLuggageMessageReceived);

			var flightDetails = CreateFlightDetails();
			SPLITTER_QUEUE.Send(new Message(flightDetails));

			Console.ReadLine();

			CleanUp();
		}

		private static void OnPassengerMessageReceived(Message message)
		{
			message.AttachFormatter(new[] { typeof(FlightDetailsInfoResponse) });
			var flightDetails = (FlightDetailsInfoResponse)message.Body;
			Console.WriteLine(
				$"Received flight information for reservation: {flightDetails.Passenger.ReservationNumber}\n" +
				$"Flight:    {flightDetails.Flight.Origin}->{flightDetails.Flight.Destination}\n" +
				$"Passenger: {flightDetails.Passenger.FirstName} {flightDetails.Passenger.LastName}\n"
				);
		}

		private static void OnLuggageMessageReceived(Message message)
		{
			message.AttachFormatter(new[] { typeof(Luggage) });
			var luggage = (Luggage)message.Body;
			Console.WriteLine(
				$"Received luggage for reservation: {luggage.ReservationId}\n" +
				$"Category: {luggage.Category}\n" +
				$"Weight:   {luggage.Weight}\n" +
				$"Seq:      {luggage.SequenceNumber}\n" +
				$"Seq max:  {luggage.TotalNumberOfLuggage}\n"
				);
		}

		private static FlightDetailsInfoResponse CreateFlightDetails()
		{
			return new FlightDetailsInfoResponse
			{
				Flight = new Flight { Origin = "Somewhere", Destination = "Knowhere" },
				Passenger = new Passenger { FirstName = "John", LastName = "Doe", ReservationNumber = "SOKN42" },
				Luggages = CreateLuggages().ToList()
			};
		}

		private static IEnumerable<Luggage> CreateLuggages()
		{
			var numberOfLuggages = 5;
			for (int i = 1; i <= numberOfLuggages; i++)
			{
				yield return new Luggage
				{
					Category = "Enormous",
					ReservationId = "SOKN42",
					SequenceNumber = i,
					TotalNumberOfLuggage = numberOfLuggages,
					Weight = 30.0f
				};
			}
		}

		private static void VerifyQueuesExistAndEmpty()
		{
			// Kontroller køerne eksisterer
			SPLITTER_QUEUE.VerifyRecreate();
			PASSENGER_INFO_QUEUE.VerifyRecreate();
			LUGGAGE_QUEUE.VerifyRecreate();

			// Tøm køerne
			SPLITTER_QUEUE.Purge();
			PASSENGER_INFO_QUEUE.Purge();
			LUGGAGE_QUEUE.Purge();
		}

		private static void CleanUp()
		{
			SPLITTER_QUEUE.Delete();
			PASSENGER_INFO_QUEUE.Delete();
			LUGGAGE_QUEUE.Delete();
		}
	}
}
