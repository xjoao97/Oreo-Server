using Quasar.Communication.Packets.Outgoing.Help.Helpers;
using Quasar.HabboHotel.GameClients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quasar.HabboHotel.Helpers
{
    public class HelperCase : IHelperElement
    {
        public GameClient Session { get; set; }
        public HelpCaseType Type;
        public string Message;
        public HabboHelper Helper;
        int Expire;


        public IHelperElement OtherElement
        {
            get
            {
                return Helper;
            }
        }

        public List<HabboHelper> DeclinedHelpers;

        public int ReamingToExpire
        {
            get
            {
                return Expire - (int)QuasarEnvironment.GetUnixTimestamp();
            }
        }

        public HelperCase(GameClient client, string msg, int category)
        {
            Session = client;
            Message = msg;
            Type = (HelpCaseType)category;
            Expire = (int)QuasarEnvironment.GetUnixTimestamp() + HelperToolsManager.ANSWER_CALL_WAIT_TIME;
            DeclinedHelpers = new List<HabboHelper>();
        }

        public void OnDecline(HabboHelper Helper)
        {
            DeclinedHelpers.Add(Helper);

            var newhelper = HelperToolsManager.GetHelpersToCase(this).FirstOrDefault();
            if (newhelper != null)
                HelperToolsManager.InvinteHelpCall(newhelper, this);
            else
            {
                Session.SendMessage(new CallForHelperErrorComposer(1));
                HelperToolsManager.RemoveCall(this);

            }
        }

        public void End(int ErrorCode = 1)
        {
            Session.SendMessage(new EndHelperSessionComposer(ErrorCode));
        }
        public void Close()
        {
            HelperToolsManager.RemoveCall(this);
            Session.SendMessage(new CloseHelperSessionComposer());
        }
    }

    public enum HelpCaseType
    {
        MEET_HOTEL = 0,
        INSTRUCTION = 1,
        EMERGENCY = 2
    }
}
