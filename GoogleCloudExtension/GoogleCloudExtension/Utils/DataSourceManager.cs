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
        private Lazy<GceDataSource> _gceDataSource;
        private Lazy<GcsDataSource> _gcsDataSource;
        private Lazy<GPlusDataSource> _gPlusDataSource;

        public ICloudExplorerSource Owner { get; }

        public PubSubDataSource PubSub => _pubSubDataSource.Value;
        public GceDataSource Gce => _gceDataSource.Value;
        public GcsDataSource Gcs => _gcsDataSource.Value;
        public GPlusDataSource GPlus => _gPlusDataSource.Value;

        public DataSourceManager(ICloudExplorerSource owner)
        {
            Owner = owner;

            InitializeDateSources();
        }

        private void InitializeDateSources()
        {
            _pubSubDataSource = new Lazy<PubSubDataSource>(CreatePubSubDataSource);
            _gceDataSource = new Lazy<GceDataSource>(CreateGceDataSource);
            _gcsDataSource = new Lazy<GcsDataSource>(CreateGcsDataSource);
            _gPlusDataSource = new Lazy<GPlusDataSource>(CreateGPlusDataSource);
        }

        private PubSubDataSource CreatePubSubDataSource()
        {
            return Owner.CurrentProject != null
                ? new PubSubDataSource(Owner.CurrentProject.ProjectId, AccountsManager.CurrentGoogleCredential)
                : null;
        }

        private GceDataSource CreateGceDataSource()
        {
            return Owner.CurrentProject != null
                ? new GceDataSource(Owner.CurrentProject.ProjectId, AccountsManager.CurrentGoogleCredential)
                : null;
        }

        private GcsDataSource CreateGcsDataSource()
        {
            return Owner.CurrentProject != null
                ? new GcsDataSource(Owner.CurrentProject.ProjectId, AccountsManager.CurrentGoogleCredential)
                : null;
        }

        private GPlusDataSource CreateGPlusDataSource()
        {
            return Owner.CurrentProject != null
                ? new GPlusDataSource(AccountsManager.CurrentGoogleCredential)
                : null;
        }
    }
}
