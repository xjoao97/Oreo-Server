Procure por este código na gameclient.cs :

if (OreoServer.GetGame().GetSettingsManager().TryGetValue("user.login.message.enabled") == "1")


troque por este :

if (OreoServer.GetGame().GetSettingsManager().TryGetValue("user.login.message.enabled") == "1")
                    {
                        SendMessage(new MOTDNotificationComposer(OreoServer.GetGame().GetLanguageManager().TryGetValue("user.login.message")));
                    }