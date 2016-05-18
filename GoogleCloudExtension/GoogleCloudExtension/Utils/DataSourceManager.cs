// Copyright 2016 Google Inc. All Rights Reserved.
// Licensed under the Apache License Version 2.0.

using System;
using GoogleCloudExtension.Accounts;
using GoogleCloudExtension.DataSources;

namespace GoogleCloudExtension.Utils
{
    /// <summary>
    /// This class provides access to all Data Sources
    /// </summary>
    public class DataSourceManager
    {
        private Lazy<PubSubDataSource> _pubSubDataSource;
        private Lazy<GceDataSource> _gceDataSource;
        private Lazy<GcsDataSource> _gcsDataSource;
        private Lazy<GPlusDataSource> _gPlusDataSource;

        public PubSubDataSource PubSub => _pubSubDataSource.Value;
        public GceDataSource Gce => _gceDataSource.Value;
        public GcsDataSource Gcs => _gcsDataSource.Value;
        public GPlusDataSource GPlus => _gPlusDataSource.Value;

        public DataSourceManager()
        {
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
            return CredentialsStore.Default.CurrentProjectId != null 
                ? new PubSubDataSource(CredentialsStore.Default.CurrentProjectId, CredentialsStore.Default.CurrentGoogleCredential)
                : null;
        }

        private GceDataSource CreateGceDataSource()
        {
            return CredentialsStore.Default.CurrentProjectId != null
                ? new GceDataSource(CredentialsStore.Default.CurrentProjectId, CredentialsStore.Default.CurrentGoogleCredential)
                : null;
        }

        private GcsDataSource CreateGcsDataSource()
        {
            return CredentialsStore.Default.CurrentProjectId != null
                ? new GcsDataSource(CredentialsStore.Default.CurrentProjectId, CredentialsStore.Default.CurrentGoogleCredential)
                : null;
        }

        private GPlusDataSource CreateGPlusDataSource()
        {
            return CredentialsStore.Default.CurrentProjectId != null
                ? new GPlusDataSource(CredentialsStore.Default.CurrentGoogleCredential)
                : null;
        }
    }
}
