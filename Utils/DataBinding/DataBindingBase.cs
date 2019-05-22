using System;
using UnityEngine;

namespace Hermit
{
    public abstract class DataBindingBase : MonoBehaviour
    {
        /// <summary>
        /// Parse an end-point reference including a type name and member name separated by a period.
        /// </summary>
        public static (string typeName, string memberName) ParseEndPointReference(string endPointReference)
        {
            var lastPeriodIndex = endPointReference.LastIndexOf('.');
            if (lastPeriodIndex == -1)
            {
                throw new Exception(
                    "No period was found, expected end-point reference in the following format: <type-name>.<member-name>. " +
                    "Provided end-point reference: " + endPointReference
                );
            }

            var typeName = endPointReference.Substring(0, lastPeriodIndex);
            var memberName = endPointReference.Substring(lastPeriodIndex + 1);
            //Due to (undocumented) unity behaviour, some of their components do not work with the namespace when using GetComponent(""), and all of them work without the namespace
            //So to be safe, we remove all namespaces from any component that starts with UnityEngine
            if (typeName.StartsWith("UnityEngine.")) { typeName = typeName.Substring(typeName.LastIndexOf('.') + 1); }

            if (typeName.Length == 0 || memberName.Length == 0)
            {
                throw new Exception(
                    "Bad format for end-point reference, expected the following format: <type-name>.<member-name>. " +
                    "Provided end-point reference: " + endPointReference
                );
            }

            return (typeName, memberName);
        }
    }
}