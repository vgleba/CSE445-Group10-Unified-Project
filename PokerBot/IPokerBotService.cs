using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace PokerBot
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IPokerBotService" in both code and config file together.
    [ServiceContract]
    public interface IPokerBotService
    {
        [OperationContract]
        BotDecisionResponse GetBotDecision(BotRequest request);
    }

    [DataContract]
    public class BotRequest
    {
        [DataMember(IsRequired = true)]
        public string GameStateJson { get; set; }
    }

    [DataContract]
    public class BotDecisionResponse
    {
        [DataMember]
        public string ActionType { get; set; }

        [DataMember]
        public int Amount { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public string RawModelResponse { get; set; }
    }
}
