using SymOntoClay.Core.Internal.CodeModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.Core.Internal.Helpers
{
    public static class AnnotationsHelper
    {
        private static Dictionary<KindOfAnnotationSystemEvent, AnnotationSystemEvent> EmptyAnnotationSystemEventsDict;

        public static Dictionary<KindOfAnnotationSystemEvent, AnnotationSystemEvent> GetAnnotationSystemEventsDictFromAnnotaion(Value annotation)
        {
            return GetAnnotationSystemEventsDict(annotation?.AsAnnotationValue?.AnnotatedItem);
        }

        public static Dictionary<KindOfAnnotationSystemEvent, AnnotationSystemEvent> GetAnnotationSystemEventsDict(AnnotatedItem annotatedItem)
        {
            var annotationSystemEventsDict = annotatedItem?.Annotations?.SelectMany(p => p.AnnotationSystemEventsDict).GroupBy(p => p.Key).ToDictionary(p => p.Key, p => p.Last().Value);

            if(annotationSystemEventsDict == null)
            {
                return EmptyAnnotationSystemEventsDict;
            }

            return annotationSystemEventsDict;
        }
    }
}
