// Copyright 2016 Google Inc. All Rights Reserved.
// Licensed under the Apache License Version 2.0.

using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace GoogleCloudExtension.Utils
{
    /// <summary>
    /// INotifyDataErrorInfo implementation is needed to support data validation in the UI
    /// </summary>
    public partial class ViewModelBase : INotifyDataErrorInfo
    {
        private readonly object _lockObj = new object();
        private readonly ConcurrentDictionary<string, List<string>> _errors =
            new ConcurrentDictionary<string, List<string>>();

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        public bool IsValid => !HasErrors;

        public bool HasErrors => _errors.Any(x => x.Value != null && x.Value.Any());

        public IEnumerable GetErrors(string propertyName)
        {
            if (!string.IsNullOrWhiteSpace(propertyName))
            {
                List<string> errors;
                _errors.TryGetValue(propertyName, out errors);
                return errors;
            }

            return _errors.SelectMany(error => error.Value);
        }

        protected void RaiseErrorsChanged(string propertyName)
        {
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }

        protected virtual void OnValidationFinished(bool hasErrors) { }

        public Task ValidateAsync()
        {
            return Task.Run(() => Validate());
        }

        public Task ValidatePropertyAsync<T>(T value, [CallerMemberName] string propertyName = null)
        {
            return Task.Run(() => ValidateProperty(value, propertyName));
        }

        private void ValidateProperty<T>(T value, string propertyName)
        {
            lock (_lockObj)
            {
                var validationContext = new ValidationContext(this, null, null)
                {
                    MemberName = propertyName
                };

                var validationResults = new List<ValidationResult>();
                Validator.TryValidateProperty(value, validationContext, validationResults);

                List<string> outputList;
                _errors.TryRemove(propertyName, out outputList);
                RaiseErrorsChanged(propertyName);

                HandleValidationResults(validationResults);
                OnValidationFinished(HasErrors);
            }
        }

        private void Validate()
        {
            lock (_lockObj)
            {
                var validationContext = new ValidationContext(this, null, null);
                var validationResults = new List<ValidationResult>();
                Validator.TryValidateObject(this, validationContext, validationResults, true);

                var errors = _errors.ToList();
                foreach (var error in errors)
                {
                    if (validationResults.All(res => res.MemberNames.All(name => name != error.Key)))
                    {
                        List<string> outputList;
                        _errors.TryRemove(error.Key, out outputList);
                        RaiseErrorsChanged(error.Key);
                    }
                }

                HandleValidationResults(validationResults);
            }

            OnValidationFinished(HasErrors);
        }

        private void HandleValidationResults(IEnumerable<ValidationResult> validationResults)
        {
            var results = from res in validationResults
                          from memberNames in res.MemberNames
                          group res by memberNames into x
                          select x;

            foreach (var group in results)
            {
                var messages = group.Select(r => r.ErrorMessage).ToList();

                if (_errors.ContainsKey(group.Key))
                {
                    List<string> outLi;
                    _errors.TryRemove(group.Key, out outLi);
                }

                _errors.TryAdd(group.Key, messages);
                RaiseErrorsChanged(group.Key);
            }
        }
    }
}
