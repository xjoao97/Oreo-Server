using System;
using log4net;
using Quasar.HabboHotel;

using Quasar.Communication.Packets.Outgoing.Moderation;
using System.Threading;
using System.Threading.Tasks;
using Quasar.Communication.Packets.Outgoing.Notifications;
using Quasar.Communication.Packets.Outgoing.Catalog;
using Quasar.Communication.Packets.Outgoing.Rooms.Notifications;
using static Quasar.HabboHotel.GameClients.GameClient;

namespace Quasar.Core
{
    public class ConsoleCommandHandler
    {
        private static readonly ILog log = LogManager.GetLogger("Quasar.Core.ConsoleCommandHandler");

        public static void InvokeCommand(string inputData)
        {
            if (string.IsNullOrEmpty(inputData))
                return;

            try
            {
                #region Command parsing
                string[] parameters = inputData.Split(' ');

                switch (parameters[0].ToLower())
                {
                    #region stop
                    case "stop":
                    case "desligar":
                    case "shutdown":
                        {
                            Logging.DisablePrimaryWriting(true);
                            QuasarEnvironment.PerformShutDown();
                            break;
                        }
                    #endregion

                    #region alert
                    case "alert":
                        {
                            string Notice = inputData.Substring(6);
                            QuasarEnvironment.GetGame().GetClientManager().SendMessage(new BroadcastMessageAlertComposer(Notice));
                            Console.WriteLine("Alerta enviada com sucesso!", ConsoleColor.DarkGreen);
                            break;
                        }
                    #endregion

                    #region help
                    case "help":
                    case "ajuda":
                    case "h":
                        {
                            Console.WriteLine("\n\nComandos disponíveis:\n\nalert - Alertar todos do hotel.\n\nstop - Desligar o emulador por completo.\n\nclear - Limpar essa tela do emulador.\n\ncata - Atualizar a Loja.\n\nitems - Atualizar items da Loja.\n\nnav - Atualize o navegador.");
                            break;
                        }
                    #endregion

                    #region cata
                    case "cata":
                    case "catalog":
                    case "shop":
                    case "c":
                        {
                            QuasarEnvironment.GetGame().GetCatalog().Init(QuasarEnvironment.GetGame().GetItemManager());
                            QuasarEnvironment.GetGame().GetLandingManager().LoadPromotions();
                            QuasarEnvironment.GetGame().GetClientManager().SendMessage(new CatalogUpdatedComposer());
                            Console.WriteLine("Loja atualizada.", ConsoleColor.DarkGreen);
                            break;
                        }
                    #endregion

                    #region items
                    case "items":
                    case "i":
                        {
                            QuasarEnvironment.GetGame().GetItemManager().Init();
                            Console.WriteLine("Items atualizados.", ConsoleColor.DarkGreen);
                            break;
                        }
                    #endregion

                    #region clear
                    case "clear":
                    case "limpar":
                        {
                            Console.Clear();
                            Console.WriteLine();
                            Console.WriteLine(@" Registros limpados com sucesso!", ConsoleColor.DarkCyan);
                            Console.WriteLine();
                            Console.ForegroundColor = ConsoleColor.DarkRed;
                            Console.WriteLine(@"--------------------------------------------------------------------------------");
                            break;
                        }
                    #endregion

                    #region navigator
                    case "navigator":
                    case "nav":
                    case "n":
                        {
                            QuasarEnvironment.GetGame().GetNavigator().Init();
                            Console.WriteLine("Navegadr atualizado.", ConsoleColor.DarkGreen);
                            break;
                        }
                    #endregion

                    default:
                        {
                            Console.WriteLine(parameters[0].ToLower() + " é um comando invalido no Sirio.", ConsoleColor.DarkRed);
                            break;
                        }
                }
                #endregion
            }
            catch (Exception e)
            {
                log.Error("Error en el comando [" + inputData + "]: " + e);
            }
        }

        public static void ShutdownIn(int time)
        {
            Thread.Sleep(time);
            
            Logging.DisablePrimaryWriting(true);
            Logging.WriteLine("The server is saving users furniture, rooms, etc. WAIT FOR THE SERVER TO CLOSE, DO NOT EXIT THE PROCESS IN TASK MANAGER!!", ConsoleColor.Yellow);
            QuasarEnvironment.PerformShutDown();
        }
    }
}