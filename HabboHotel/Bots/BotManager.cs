using System;
using System.Linq;
using System.Text;
using System.Data;
using System.Collections.Generic;

using log4net;
using Quasar.Database.Interfaces;
using Quasar.HabboHotel.Rooms.AI.Responses;
using Quasar.HabboHotel.Rooms.AI;

namespace Quasar.HabboHotel.Bots
{
    public class BotManager
    {
        private static readonly ILog log = LogManager.GetLogger("Quasar.HabboHotel.Rooms.AI.BotManager");

        private List<BotResponse> _responses;

        public BotManager()
        {
            this._responses = new List<BotResponse>();

            this.Init();
        }

        public void Init()
        {
            BotDao.LoadBotResponses(_responses);
                
	    }
            
        

        public BotResponse GetResponse(BotAIType AiType, string Message)
        {
            foreach (BotResponse Response in this._responses.Where(X => X.AiType == AiType).ToList())
            {
                if (Response.KeywordMatched(Message))
                {
                    return Response;
                }
            }

            return null;
        }
    }
}
