using IntegrationPatterns.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationPatterns.Resequencer
{
	class ResequencerConsole
	{
		private static readonly MessageQueue UNSEQUENCED_QUEUE = new MessageQueue(@".\private$\unsequenced");
		private static readonly MessageQueue SEQUENCED_QUEUE = new MessageQueue(@".\private$\sequenced");
		private static readonly Random RANDOM = new Random();

		static void Main(string[] args)
		{
			VerifyQueuesExistAndEmpty();

			var resequencer = new Resequencer(UNSEQUENCED_QUEUE, SEQUENCED_QUEUE);

			resequencer.BeginReceive();

			QueueReader.BeginReceive(SEQUENCED_QUEUE, OnSequencedMessageReceived);

			var surge = CreateUnsequencedSurge(10, 50).ToList();
			SendUnsequencedSurge(surge);

			Console.ReadLine();

			CleanUp();
		}

		private static void OnSequencedMessageReceived(Message message)
		{
			message.AttachFormatter(new[] { typeof(SequenceMessage) });
			var sequenceMessage = (SequenceMessage)message.Body;
			Console.WriteLine($"{sequenceMessage.Id}\t{sequenceMessage.Sequence} of {sequenceMessage.SequenceMax}\t{sequenceMessage.Payload}");
		}

		private static void SendUnsequencedSurge(IEnumerable<SequenceMessage> surge)
		{
			foreach (var sm in surge)
			{
				UNSEQUENCED_QUEUE.Send(new Message(sm));
			}
		}

		private static IEnumerable<SequenceMessage> CreateUnsequencedSurge(int sequences, int messagesPerSequence)
		{
			if (sequences < 1 || messagesPerSequence < 1)
				throw new ArgumentException("A and B values cannot be zero or less");

			for (var i = 1; i <= sequences; i++)
			{
				var guid = Guid.NewGuid();
				foreach (var sm in CreateSequenceMessages(guid, i, messagesPerSequence))
				{
					yield return sm;
				}
			}
		}

		private static IEnumerable<SequenceMessage> CreateSequenceMessages(Guid guid, int currentSequence, int messagesPerSequence)
		{
			var messages = new List<SequenceMessage>();
			for (var i = 1; i <= messagesPerSequence; i++)
			{
				messages.Add(new SequenceMessage 
				{ 
					Id = guid, 
					Payload = TEXT_MAX_1000[currentSequence * i], 
					Sequence = i, 
					SequenceMax = messagesPerSequence 
				});
			}
			messages.Shuffle(RANDOM);
			return messages;
		}

		private static void VerifyQueuesExistAndEmpty()
		{
			// Kontroller køerne eksisterer
			UNSEQUENCED_QUEUE.VerifyRecreate();
			SEQUENCED_QUEUE.VerifyRecreate();

			// Tøm køerne
			UNSEQUENCED_QUEUE.Purge();
			SEQUENCED_QUEUE.Purge();
		}

		private static void CleanUp()
		{
			UNSEQUENCED_QUEUE.Delete();
			SEQUENCED_QUEUE.Delete();
		}

		private static readonly string[] TEXT_MAX_1000 = @"
Lorem ipsum dolor sit amet consectetur adipiscing elit Sed in consequat neque Mauris vestibulum dapibus scelerisque Aliquam
cursus nunc dolor viverra malesuada nulla imperdiet in Cras vitae mauris elit Nulla augue erat congue nec enim ut feugiat tincidunt
felis In est erat rhoncus id posuere non facilisis non eros Phasellus ut laoreet neque Sed non tristique odio Pellentesque habitant morbi
tristique senectus et netus et malesuada fames ac turpis egestas Proin tempor et ipsum tincidunt scelerisque
Fusce at bibendum mauris Fusce id vestibulum quam Vivamus et metus imperdiet egestas sem ac congue tortor Nulla sed justo
vehicula semper ipsum id sodales ipsum Fusce a placerat neque Lorem ipsum dolor sit amet consectetur adipiscing elit Suspendisse potenti
Nam odio nibh hendrerit ac enim non suscipit efficitur dolor Mauris turpis arcu dignissim at mattis a eleifend sed metus Aliquam
et ligula vel dui ultricies porttitor
Pellentesque at porttitor tortor Duis volutpat suscipit orci gravida laoreet dolor consectetur vel Nullam sit amet purus
ut sapien luctus tincidunt Aenean viverra accumsan finibus Suspendisse potenti Suspendisse potenti Cras metus augue maximus non eros
vel facilisis porttitor libero Morbi tempor enim velit Ut auctor facilisis sodales Curabitur molestie posuere nunc mattis blandit
Integer in tristique metus Etiam erat neque dictum in convallis vel vulputate a sapien Nulla laoreet dapibus justo quis euismod ipsum
elementum in Donec volutpat mi in felis finibus aliquam eleifend sapien gravida Pellentesque et est eu tellus faucibus tempus nec accumsan massa
Ut lobortis scelerisque eros commodo egestas Suspendisse potenti Cras a dapibus dui Curabitur rhoncus gravida viverra Morbi
ac venenatis nisl Ut suscipit dui finibus porttitor aliquet risus lorem lacinia urna eget porta massa tellus sit amet tortor Mauris
id leo sit amet elit posuere venenatis Vivamus ut dapibus arcu sed consequat sapien Praesent pellentesque massa risus in viverra ex
consectetur eget Nulla eu convallis justo porta venenatis risus Praesent est tortor mattis ut ligula at mollis fringilla mi Nulla sed
turpis auctor molestie libero ac accumsan arcu Maecenas tincidunt nec urna sit amet molestie Etiam eget augue tincidunt scelerisque
lorem at fringilla dolor Sed eget lacinia nisl ut sodales enim
In sodales a nisl ut imperdiet Vestibulum imperdiet leo quis malesuada posuere Mauris id nunc euismod accumsan ex a ullamcorper
libero Duis interdum sed sapien vitae facilisis Nullam a ultrices est Quisque rutrum viverra velit eget gravida Maecenas feugiat et quam non
gravida Mauris ac sem et velit sagittis blandit sed et libero Nam dictum efficitur mi eu condimentum arcu tristique sit amet
Sed a ex augue Cras iaculis justo sit amet justo egestas venenatis Donec molestie ligula vel justo auctor quis consectetur elit
commodo Phasellus ullamcorper varius felis a pharetra Vestibulum vel auctor nibh nec bibendum libero Sed vitae lobortis nulla Aliquam erat
volutpat Fusce varius porttitor hendrerit Sed vitae tortor ullamcorper consectetur sem sit amet fermentum nisl Phasellus vitae finibus
dui Cras at magna consequat hendrerit quam vitae vehicula leo Integer laoreet luctus libero Fusce auctor nisi non purus ornare bibendum
quis non diam In tempus sit amet neque in fringilla Aliquam erat volutpat Praesent sem tellus blandit et neque at lobortis volutpat lectus
Aliquam sed vestibulum urna Aenean ac aliquet quam id imperdiet sem Proin lacinia ornare ligula vel pretium Quisque rhoncus
dolor ac nisl porta sed tincidunt ante lacinia Donec ac bibendum velit Etiam condimentum scelerisque mi ac euismod Nam sagittis nibh non
varius luctus leo lorem laoreet risus id tristique nisi neque ultrices massa Morbi at risus vel est lobortis lacinia Quisque sed sapien
vel tellus imperdiet congue eu sit amet lacus Donec nec lectus congue nisi eleifend pretium
Sed quis massa pulvinar tempor ante non ullamcorper risus Sed nec metus ultrices hendrerit velit eget blandit nisl Nullam
facilisis eros nisl eget suscipit sapien tristique non Nunc vel elit eget nibh dictum rutrum In sed tortor volutpat scelerisque leo
commodo egestas libero Mauris malesuada metus in consequat pulvinar odio dui lobortis ex et cursus felis erat nec nulla Nunc ornare
mattis aliquet Integer dignissim ex odio ut consectetur tortor tincidunt vitae Sed pharetra accumsan ex quis mollis Vestibulum pretium
tempus est et molestie Suspendisse lacus enim bibendum eget rutrum eget mollis id ante Suspendisse maximus eleifend orci ac tincidunt
lorem volutpat ac Mauris ornare ex quam vel hendrerit nunc consectetur quis Cras gravida nisl at ornare consequat enim augue gravida
nibh ut semper eros ex quis ligula Nulla molestie est sit amet cursus porttitor
Phasellus eget eros justo Pellentesque mollis pretium imperdiet Proin feugiat tortor ut ultrices dapibus Nullam sed odio sit
amet magna sodales condimentum Praesent non mauris sapien Curabitur purus nibh rutrum at elit non congue luctus arcu Cras nec diam
viverra laoreet quam accumsan mollis est Phasellus non imperdiet elit Morbi commodo eleifend aliquam Quisque pellentesque porttitor
massa sed dictum Integer consequat nunc a libero scelerisque congue Suspendisse sit amet pharetra nunc Nunc eu sapien leo Nullam at sem
id massa condimentum laoreet Etiam pellentesque eros non ullamcorper tristique libero quam egestas erat tincidunt tincidunt justo
dui vitae augue Phasellus id tincidunt metus
Nulla gravida erat id nulla consequat vel fermentum lectus cursus Phasellus cursus tincidunt odio at feugiat Phasellus
dolor ante sodales vitae odio in pellentesque iaculis mi Aliquam in purus dolor Nulla nibh leo sollicitudin eu magna non pretium 
ornare nibh Quisque a interdum mi Vestibulum pulvinar nibh efficitur lacinia dui et aliquet ipsum Fusce in facilisis dolor Orci 
varius natoque penatibus et magnis dis parturient montes nascetur ridiculus mus Vivamus ultrices nibh non consectetur varius Suspendisse
eu ullamcorper est Aenean hendrerit lacus justo ut convallis justo tempus in Sed vulputate nunc at pellentesque mattis magna massa
vulputate odio nec scelerisque ex nibh ac nisi
Sed convallis quam eget massa convallis convallis Sed id vulputate lectus Nunc lorem libero condimentum nec ante ut
interdum faucibus arcu Vestibulum tellus nisi feugiat id mi et cursus condimentum ante Donec nec augue ac diam viverra viverra Aenean
pulvinar consectetur enim sit amet lacinia sem faucibus eu Curabitur ac molestie arcu Cras lacinia auctor mauris a tincidunt felis
convallis et In hac habitasse platea dictumst
			".Split(' ');
	}
}
