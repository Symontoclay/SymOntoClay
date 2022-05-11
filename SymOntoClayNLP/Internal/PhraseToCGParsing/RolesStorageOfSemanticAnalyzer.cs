using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.NLP.Internal.CG;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.NLP.Internal.PhraseToCGParsing
{
    public class RolesStorageOfSemanticAnalyzer : IObjectToString
    {
        private Dictionary<string, List<ConceptCGNode>> mRolesDict = new Dictionary<string, List<ConceptCGNode>>();

        public void Add(string role, ConceptCGNode concept)
        {
            if (mRolesDict.ContainsKey(role))
            {
                var itemsList = mRolesDict[role];
                if (itemsList.Contains(concept))
                {
                    return;
                }
                itemsList.Add(concept);
                return;
            }

            var newItemsList = new List<ConceptCGNode>();
            newItemsList.Add(concept);
            mRolesDict[role] = newItemsList;
        }

        public List<string> Roles
        {
            get
            {
                return mRolesDict.Keys.ToList();
            }
        }

        public int Count
        {
            get
            {
                return mRolesDict.Count;
            }
        }

        public void Assing(RolesStorageOfSemanticAnalyzer source)
        {
            if (source.Count == 0)
            {
                return;
            }

            foreach (var rolesDictKVPItem in source.mRolesDict)
            {
                var role = rolesDictKVPItem.Key;

                foreach (var extendedToken in rolesDictKVPItem.Value)
                {
                    Add(role, extendedToken);
                }
            }
        }

        public List<ConceptCGNode> GetByRole(string role)
        {
            if (mRolesDict.ContainsKey(role))
            {
                return mRolesDict[role];
            }

            return null;
        }

        public override string ToString()
        {
            return ToString(0u);
        }

        public string ToString(uint n)
        {
            return this.GetDefaultToStringInformation(n);
        }

        public string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var nextN = n + 4;
            var nextNSpaces = DisplayHelper.Spaces(nextN);
            var nextNextN = nextN + 4;
            var sb = new StringBuilder();
            if (mRolesDict == null)
            {
                sb.AppendLine($"{spaces}{nameof(mRolesDict)} = null");
            }
            else
            {
                sb.AppendLine($"{spaces}Begin {nameof(mRolesDict)}");
                foreach (var role in mRolesDict)
                {
                    sb.AppendLine($"{nextNSpaces}{nameof(role)} = {role.Key}");
                    var itemsOfRole = role.Value;
                    sb.AppendLine($"{spaces}Begin {nameof(itemsOfRole)}");
                    foreach (var itemOfRole in itemsOfRole)
                    {
                        sb.Append(itemOfRole.ToBriefString(nextNextN));
                    }
                    sb.AppendLine($"{spaces}End {nameof(itemsOfRole)}");
                }
                sb.AppendLine($"{spaces}End {nameof(mRolesDict)}");
            }
            return sb.ToString();
        }
    }
}
