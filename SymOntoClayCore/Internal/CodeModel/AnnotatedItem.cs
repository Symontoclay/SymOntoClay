/*MIT License

Copyright (c) 2020 - 2024 Sergiy Tolkachov

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.*/

using NLog;
using SymOntoClay.Core.DebugHelpers;
using SymOntoClay.Core.Internal.IndexedData;
using SymOntoClay.CoreHelper;
using SymOntoClay.CoreHelper.CollectionsHelpers;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public abstract class AnnotatedItem : ItemWithLongHashCodes, IAnnotatedItem, IObjectToString, IObjectToShortString, IObjectToBriefString, IObjectToDbgString, IObjectToHumanizedString, ISymOntoClayDisposable
    {
        /// <summary>
        /// It is 'Clauses section' in the documentation.
        /// </summary>
        [ResolveToType(typeof(LogicalValue))]
        public virtual IList<Value> WhereSection { get; set; } = new List<Value>();

        /// <summary>
        /// Returns <c>true</c> if the instance has modalities or additional sections, otherwise returns <c>false</c>.
        /// </summary>
        public bool HasModalitiesOrSections => !WhereSection.IsNullOrEmpty();

        public bool HasConditionalSections => !WhereSection.IsNullOrEmpty();

        /// <inheritdoc/>
        protected override ulong CalculateLongConditionalHashCode(CheckDirtyOptions options)
        {
            ulong result = 0;

            if (!WhereSection.IsNullOrEmpty())
            {
                foreach (var whereItem in WhereSection)
                {
                    result ^= whereItem.GetLongHashCode(options);
                }
            }

            return result;
        }

        /// <inheritdoc/>
        protected override ulong CalculateLongHashCode(CheckDirtyOptions options)
        {
            ulong result = 0;

            if (!WhereSection.IsNullOrEmpty())
            {
                foreach (var item in WhereSection)
                {
                    result ^= LongHashCodeWeights.BaseModalityWeight ^ item.GetLongHashCode(options);
                }
            }

            if (!Annotations.IsNullOrEmpty())
            {
                foreach (var item in Annotations)
                {
                    result ^= LongHashCodeWeights.BaseModalityWeight ^ item.GetLongHashCode(options);
                }
            }

            return result;
        }

        private readonly object _annotationsLockObj = new object();
        private int? _isAnnotationsCount;

        /// <inheritdoc/>
        public virtual IList<RuleInstance> AnnotationFacts
        {
            get
            {
                lock(_annotationsLockObj)
                {
                    CheckAnnotationsPreparation();

                    return _annotationFacts;
                }                
            }
        }

        /// <inheritdoc/>
        public virtual IList<Value> MeaningRolesList
        {
            get
            {
                lock (_annotationsLockObj)
                {
                    CheckAnnotationsPreparation();

                    return _meaningRolesList;
                }
            }
        }

        /// <inheritdoc/>
        public virtual Value GetSettings(StrongIdentifierValue key)
        {
            lock (_annotationsLockObj)
            {
                CheckAnnotationsPreparation();

                if(_settingsDict.ContainsKey(key))
                {
                    return _settingsDict[key];
                }

                return null;
            }
        }

        private List<RuleInstance> _annotationFacts = new List<RuleInstance>();
        private List<Value> _meaningRolesList = new List<Value>();
        private Dictionary<StrongIdentifierValue, Value> _settingsDict = new Dictionary<StrongIdentifierValue, Value>();

        private void CheckAnnotationsPreparation()
        {
            if(_isAnnotationsCount.HasValue && _isAnnotationsCount.Value == Annotations.Count)
            {
                return;
            }

            if(!Annotations.IsNullOrEmpty())
            {
                _annotationFacts = Annotations.Where(p => !p.Facts.IsNullOrEmpty()).SelectMany(p => p.Facts).ToList();
                _meaningRolesList = Annotations.Where(p => !p.MeaningRolesList.IsNullOrEmpty()).SelectMany(p => p.MeaningRolesList).ToList();

                _settingsDict = new Dictionary<StrongIdentifierValue, Value>();

                foreach (var annotation in Annotations)
                {
                    var settingsDict = annotation.SettingsDict;

                    if (settingsDict.IsNullOrEmpty())
                    {
                        continue;
                    }

                    foreach (var item in settingsDict)
                    {
                        _settingsDict[item.Key] = item.Value;
                    }
                }
            }
        }

        /// <inheritdoc/>
        public virtual IList<Annotation> Annotations { get; set; }

        /// <inheritdoc/>
        public virtual void AddAnnotation(Annotation annotation)
        {
            if(Annotations == null)
            {
                Annotations = new List<Annotation>();
            }

            Annotations.Add(annotation);
        }

        /// <include file = "..\CommonDoc.xml" path='extradoc/method[@name="Clone"]/*' />
        public AnnotatedItem CloneAnnotatedItem()
        {
            var context = new Dictionary<object, object>();
            return CloneAnnotatedItem(context);
        }

        /// <include file = "..\CommonDoc.xml" path='extradoc/method[@name="CloneWithContext"]/*' />
        public abstract AnnotatedItem CloneAnnotatedItem(Dictionary<object, object> context);

        public void AppendAnnotations(AnnotatedItem source)
        {
            var context = new Dictionary<object, object>();
            AppendAnnotations(source, context);
        }

        public void AppendAnnotations(AnnotatedItem source, Dictionary<object, object> context)
        {
            if(source.WhereSection == null)
            {
                WhereSection = null;
            }
            else
            {
                if(WhereSection == null)
                {
                    WhereSection = new List<Value>();
                }                

                foreach (var item in source.WhereSection)
                {
                    WhereSection.Add(item.CloneValue(context));
                }
            }

            if(source.Annotations == null)
            {
                Annotations = null;
            }
            else
            {
                if(Annotations == null)
                {
                    Annotations = new List<Annotation>();
                }                

                foreach(var annotation in source.Annotations)
                {
                    Annotations.Add(annotation.Clone(context));
                }
            }
        }

        public IList<Annotation> GetAllAnnotations()
        {
            var result = new List<Annotation>();
            DiscoverAllAnnotations(result);
            return result.Distinct().ToList();
        }

        public virtual void DiscoverAllAnnotations(IList<Annotation> result)
        {
            if (!Annotations.IsNullOrEmpty())
            {
                foreach (var annotation in Annotations)
                {
                    result.Add(annotation);

                    annotation.DiscoverAllAnnotations(result);
                }
            }

            if (!WhereSection.IsNullOrEmpty())
            {
                foreach (var item in WhereSection)
                {
                    item.DiscoverAllAnnotations(result);
                }
            }
        }

        private Value _annotationValue;
        private readonly object _annotationValueLockObj = new object();

        /// <inheritdoc/>
        public virtual Value GetAnnotationValue()
        {
            lock(_annotationValueLockObj)
            {
                if(_annotationValue == null)
                {
                    _annotationValue = new AnnotationValue() { AnnotatedItem = this};
                }

                return _annotationValue;
            }            
        }

        private readonly object _disposingLockObj = new object();
        private bool _isDisposed;

        /// <inheritdoc/>
        public bool IsDisposed
        {
            get
            {
                lock (_disposingLockObj)
                {
                    return _isDisposed;
                }
            }
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            lock (_disposingLockObj)
            {
                if (_isDisposed)
                {
                    return;
                }

                _isDisposed = true;

                OnDisposed();
            }
        }

        protected virtual void OnDisposed()
        {
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return ToString(0u);
        }

        /// <inheritdoc/>
        public string ToString(uint n)
        {
            return this.GetDefaultToStringInformation(n);
        }

        /// <inheritdoc/>
        string IObjectToString.PropertiesToString(uint n)
        {
            return PropertiesToString(n);
        }
        
        protected virtual string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(HasModalitiesOrSections)} = {HasModalitiesOrSections}");
            sb.AppendLine($"{spaces}{nameof(HasConditionalSections)} = {HasConditionalSections}");

            sb.AppendLine($"{spaces}{nameof(_longConditionalHashCode)} = {_longConditionalHashCode}");
            sb.AppendLine($"{spaces}{nameof(_longHashCode)} = {_longHashCode}");

            sb.PrintObjListProp(n, nameof(WhereSection), WhereSection);
            
            sb.PrintObjListProp(n, nameof(Annotations), Annotations);

            return sb.ToString();
        }

        /// <inheritdoc/>
        public string ToShortString()
        {
            return ToShortString(0u);
        }

        /// <inheritdoc/>
        public string ToShortString(uint n)
        {
            return this.GetDefaultToShortStringInformation(n);
        }

        /// <inheritdoc/>
        string IObjectToShortString.PropertiesToShortString(uint n)
        {
            return PropertiesToShortString(n);
        }

        protected virtual string PropertiesToShortString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(HasModalitiesOrSections)} = {HasModalitiesOrSections}");
            sb.AppendLine($"{spaces}{nameof(HasConditionalSections)} = {HasConditionalSections}");

            sb.AppendLine($"{spaces}{nameof(_longConditionalHashCode)} = {_longConditionalHashCode}");
            sb.AppendLine($"{spaces}{nameof(_longHashCode)} = {_longHashCode}");

            sb.PrintShortObjListProp(n, nameof(WhereSection), WhereSection);
            
            sb.PrintShortObjListProp(n, nameof(Annotations), Annotations);

            return sb.ToString();
        }

        /// <inheritdoc/>
        public string ToBriefString()
        {
            return ToBriefString(0u);
        }

        /// <inheritdoc/>
        public string ToBriefString(uint n)
        {
            return this.GetDefaultToBriefStringInformation(n);
        }

        /// <inheritdoc/>
        string IObjectToBriefString.PropertiesToBriefString(uint n)
        {
            return PropertiesToBriefString(n);
        }

        protected virtual string PropertiesToBriefString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(HasModalitiesOrSections)} = {HasModalitiesOrSections}");
            sb.AppendLine($"{spaces}{nameof(HasConditionalSections)} = {HasConditionalSections}");

            sb.AppendLine($"{spaces}{nameof(_longConditionalHashCode)} = {_longConditionalHashCode}");
            sb.AppendLine($"{spaces}{nameof(_longHashCode)} = {_longHashCode}");

            sb.PrintExistingList(n, nameof(WhereSection), WhereSection);

            sb.PrintExistingList(n, nameof(Annotations), Annotations);

            return sb.ToString();
        }

        /// <inheritdoc/>
        public string ToDbgString()
        {
            return ToDbgString(0u);
        }

        /// <inheritdoc/>
        public string ToDbgString(uint n)
        {
            return this.GetDefaultToDbgStringInformation(n);
        }

        /// <inheritdoc/>
        string IObjectToDbgString.PropertiesToDbgString(uint n)
        {
            return PropertiesToDbgString(n);
        }

        protected virtual string PropertiesToDbgString(uint n)
        {
#if DEBUG
            DebugLogger.Instance.Info(this);
#endif

            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public string ToHumanizedString(HumanizedOptions options = HumanizedOptions.ShowAll)
        {
            var opt = new DebugHelperOptions()
            {
                HumanizedOptions = options
            };

            return ToHumanizedString(opt);
        }

        /// <inheritdoc/>
        public abstract string ToHumanizedString(DebugHelperOptions options);

        /// <inheritdoc/>
        public string ToHumanizedLabel(HumanizedOptions options = HumanizedOptions.ShowAll)
        {
            var opt = new DebugHelperOptions()
            {
                HumanizedOptions = options
            };

            return ToHumanizedLabel(opt);
        }

        /// <inheritdoc/>
        public abstract string ToHumanizedLabel(DebugHelperOptions options);
    }
}
