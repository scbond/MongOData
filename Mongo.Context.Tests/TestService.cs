﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mongo.Context.Tests
{
    using System;
    using System.ServiceModel.Web;
    using System.Threading;

    public class TestService : IDisposable
    {
        private WebServiceHost host;
        private Uri serviceUri;
        private static int lastHostId = 1;

        public TestService(Type serviceType)
        {
            for (int i = 0; i < 100; i++)
            {
                int hostId = Interlocked.Increment(ref lastHostId);
                this.serviceUri = new Uri("http://" + Environment.MachineName + "/Temporary_Listen_Addresses/MongoTestService" + hostId.ToString() + "/");
                this.host = new WebServiceHost(serviceType, this.serviceUri);
                try
                {
                    this.host.Open();
                    break;
                }
                catch (Exception)
                {
                    this.host.Abort();
                    this.host = null;
                }
            }

            if (this.host == null)
            {
                throw new InvalidOperationException("Could not open a service even after 100 tries.");
            }
        }

        public void Dispose()
        {
            if (this.host != null)
            {
                this.host.Close();
                this.host = null;
            }
        }

        public Uri ServiceUri
        {
            get { return this.serviceUri; }
        }

        public static MongoConfiguration Configuration { get; set; }
    }
}
