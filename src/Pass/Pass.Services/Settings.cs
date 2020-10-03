using Pass.Services.Compiler;
using Pass.Services.Database;
using Pass.Services.Test;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Pass.Services
{
    internal class Settings : ISerializable
    {
        [NonSerialized]
        private static Settings _instance;

        internal Settings Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new Settings();
                }

                return _instance;
            }
            set
            {
                _instance = value;
            }
        }

        #region ::Services::

        internal List<Service> Services { get; set; } = new List<Service>();

        internal List<CompilerService> CompilerServices { get; set; } = new List<CompilerService>();

        internal DatabaseService DatabaseService { get; set; } = new DatabaseService();

        internal List<TestService> TestServices { get; set; } = new List<TestService>();

        #endregion

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            ((ISerializable)this).GetObjectData(info, context);
        }
    }
}
