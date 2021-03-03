using IntegrationPatterns.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationPatterns.Splitter
{
	class Splitter
	{
		private MessageQueue inQueue;
		private MessageQueue passengerInfoQueue;
		private MessageQueue luggageQueue;

		public Splitter(MessageQueue inQueue, MessageQueue passengerInfoQueue, MessageQueue luggageQueue)
		{
			this.inQueue = inQueue;
			this.passengerInfoQueue = passengerInfoQueue;
			this.luggageQueue = luggageQueue;
		}

		public void BeginReceive()
		{
			QueueReader.BeginReceive(inQueue, OnMessageReceived);
		}

		private void OnMessageReceived(Message message)
		{
			Split(message);
			inQueue.BeginReceive();
		}

		private void Split(Message message)
		{
			message.AttachFormatter(new[] { typeof(FlightDetailsInfoResponse) });
			var messageToSplit = (FlightDetailsInfoResponse)message.Body;

			// Opret en ny besked, som ikke indeholder baggage elementet
			var messageToPassengerInfoQueue = new FlightDetailsInfoResponse
			{
				Flight = messageToSplit.Flight,
				Passenger = messageToSplit.Passenger
			};

			// Send PassengerInfo beskeden
			SendPassengerInfoMessage(messageToPassengerInfoQueue);

			// Send Luggage beskederne hver for sig
			SendLuggageMessages(messageToSplit.Luggages);
		}

		private void SendPassengerInfoMessage(FlightDetailsInfoResponse messageToPassengerInfoQueue)
		{
			this.passengerInfoQueue.Send(new Message(messageToPassengerInfoQueue));
		}

		private void SendLuggageMessages(List<Luggage> luggages)
		{
			foreach (var l in luggages)
			{
				this.luggageQueue.Send(new Message(l));
			}
		}
	}
}
