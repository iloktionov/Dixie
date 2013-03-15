using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Dixie.Core
{
	internal partial class HeartBeatProcessor
	{
		public HeartBeatProcessor(Topology topology, Master master, TimeSpan heartBeatPeriod)
		{
			this.topology = topology;
			this.master = master;
			this.heartBeatPeriod = heartBeatPeriod;
			outgoingMessages = new List<KeyValuePair<HeartBeatMessage, TimeSpan>>();
			incomingResponses = new List<KeyValuePair<HeartBeatResponse, TimeSpan>>();
			watch = Stopwatch.StartNew();
		}

		public void DeliverMessagesAndResponses()
		{
			SendMessages();
			DeliverOutgoingMessages();
			DeliverIncomingResponses();
		}

		private void SendMessages()
		{
			List<Node> workerNodes = topology.GetWorkerNodes();
			TimeSpan timeElapsed = watch.Elapsed;
			foreach (Node workerNode in workerNodes)
			{
				if (!NeededToSendMessage(workerNode, timeElapsed))
					continue;
				TimeSpan latency;
				if (!topology.TryGetNodeLatency(workerNode.Id, out latency))
					continue;
				HeartBeatMessage message = workerNode.GetHeartBeatMessage();
				message.CommunicationLatency = latency;
				outgoingMessages.Add(new KeyValuePair<HeartBeatMessage, TimeSpan>(message, timeElapsed + latency));
			}
		}

		private void DeliverOutgoingMessages()
		{
			TimeSpan timeElapsed = watch.Elapsed;
			var toRemove = new HashSet<KeyValuePair<HeartBeatMessage, TimeSpan>>(outgoingMessages.Where(pair => timeElapsed >= pair.Value));
			if (toRemove.Count <= 0)
				return;
			outgoingMessages.RemoveAll(toRemove.Contains);
			foreach (KeyValuePair<HeartBeatMessage, TimeSpan> pair in toRemove)
			{
				HeartBeatMessage message = pair.Key;
				HeartBeatResponse response = master.HandleHeartBeatMessage(message);
				incomingResponses.Add(new KeyValuePair<HeartBeatResponse, TimeSpan>(response, timeElapsed + message.CommunicationLatency));
			}
		}

		private void DeliverIncomingResponses()
		{
			TimeSpan timeElapsed = watch.Elapsed;
			var toRemove = new HashSet<KeyValuePair<HeartBeatResponse, TimeSpan>>(incomingResponses.Where(pair => timeElapsed >= pair.Value));
			if (toRemove.Count <= 0)
				return;
			incomingResponses.RemoveAll(toRemove.Contains);
			foreach (KeyValuePair<HeartBeatResponse, TimeSpan> pair in toRemove)
			{
				HeartBeatResponse response = pair.Key;
				Node node;
				if (topology.TryGetNode(response.NodeId, out node))
					node.HandleHeartBeatResponse(response);
			}
		}

		private bool NeededToSendMessage(Node node, TimeSpan timeElapsed)
		{
			if (timeElapsed >= node.LastHBTimestamp + heartBeatPeriod)
			{
				node.LastHBTimestamp = timeElapsed;
				return true;
			}
			return false;
		}

		private readonly Topology topology;
		private readonly Master master;
		private readonly TimeSpan heartBeatPeriod;
		private readonly Stopwatch watch;
		private readonly List<KeyValuePair<HeartBeatMessage, TimeSpan>> outgoingMessages;
		private readonly List<KeyValuePair<HeartBeatResponse, TimeSpan>> incomingResponses;
	}
}