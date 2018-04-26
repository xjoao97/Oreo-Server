using System;

using Quasar.Database.Interfaces;

namespace Quasar.Database.Adapter
{
    public class NormalQueryReactor : QueryAdapter, IQueryAdapter, IRegularQueryAdapter, IDisposable
    {
        public NormalQueryReactor(IDatabaseClient Client)
            : base(Client)
        {
            base.command = Client.CreateNewCommand();
        }

        public void Dispose()
        {
            base.command.Dispose();
            base.client.ReportDone();
            GC.SuppressFinalize(this);
        }
    }
}