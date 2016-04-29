// Copyright 2016 Google Inc. All Rights Reserved.
// Licensed under the Apache License Version 2.0.

using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using GoogleCloudExtension.CloudExplorer;
using GoogleCloudExtension.Utils;

namespace GoogleCloudExtension.CloudExplorerSources.PubSub.Common
{
    public class DataViewModelBase : ViewModelBase, INotifyDataErrorInfo
    {
        private readonly ConcurrentDictionary<string, List<string>> _errors =
            new ConcurrentDictionary<string, List<string>>();

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        public ICloudExplorerSource Owner { get; set; }

        public DataSourceManager DataManager { get; set; }

        public bool HasErrors
        {
            get { return _errors.Any(x => x.Value != null && x.Value.Any()); }
        }

        public DataViewModelBase(ICloudExplorerSource owner)
        {
            Owner = owner;
            DataManager = new DataSourceManager(owner);
        }

        protected void RaiseErrorsChanged(string propertyName)
        {
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }

        public IEnumerable GetErrors(string propertyName)
        {
            List<string> errors;
            _errors.TryGetValue(propertyName, out errors);
            return errors;
        }

        public Task ValidateAsync()
        {
            return Task.Run(() => Validate());
        }

        protected virtual void ValidationFinished(bool hasErrors) { }

        public void Validate()
        {
            lock (_errors)
            {
                var validationContext = new ValidationContext(this, null, null);
                var validationResults = new List<ValidationResult>();
                Validator.TryValidateObject(this, validationContext, validationResults, true);

                foreach (var kv in _errors.ToList())
                {
                    if (validationResults.All(r => r.MemberNames.All(m => m != kv.Key)))
                    {
                        List<string> outputList;
                        _errors.TryRemove(kv.Key, out outputList);
                        RaiseErrorsChanged(kv.Key);
                    }
                }

                var results = from valResult in validationResults
                              from memberNames in valResult.MemberNames
                              group valResult by memberNames into valGroup
                              select valGroup;

                foreach (var prop in results)
                {
                    var messages = prop.Select(r => r.ErrorMessage).ToList();

                    if (_errors.ContainsKey(prop.Key))
                    {
                        List<string> outLi;
                        _errors.TryRemove(prop.Key, out outLi);
                    }

                    _errors.TryAdd(prop.Key, messages);
                    RaisePropertyChanged(prop.Key);
                }
            }

            ValidationFinished(HasErrors);
        }
    }
}
