// Copyright 2016 Google Inc. All Rights Reserved.
// Licensed under the Apache License Version 2.0.

using System;
using GoogleCloudExtension.Accounts;
using GoogleCloudExtension.CloudExplorer;
using GoogleCloudExtension.DataSources;

namespace GoogleCloudExtension.Utils
{
    public class DataSourceManager
    {
        private Lazy<PubSubDataSource> _pubSubDataSource;

        public ICloudExplorerSource Owner { get; }

        public PubSubDataSource PubSub => _pubSubDataSource.Value;

        public DataSourceManager(ICloudExplorerSource owner)
        {
            Owner = owner;

            InitializeDateSources();
        }

        private void InitializeDateSources()
        {
            _pubSubDataSource = new Lazy<PubSubDataSource>(CreatePubSubDataSource);
        }

        private PubSubDataSource CreatePubSubDataSource()
        {
            return Owner.CurrentProject != null
                ? new PubSubDataSource(Owner.CurrentProject.ProjectId, AccountsManager.CurrentGoogleCredential)
                : null;
        }
    }
}
