﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.3074
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Jellyfish.Virtu.Properties {
    using System;
    
    
    /// <summary>
    /// A strongly-typed resource class, for looking up localized strings, formatting them, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilderEx class via the ResXFileCodeGeneratorEx custom tool.
    // To add or remove a member, edit your .ResX file then rerun the ResXFileCodeGeneratorEx custom tool or rebuild your VS.NET project.
    // Copyright (c) Dmytro Kryvko 2006-2009 (http://dmytro.kryvko.googlepages.com/)
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("DMKSoftware.CodeGenerators.Tools.StronglyTypedResourceBuilderEx", "2.4.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Naming", "CA1724:TypeNamesShouldNotMatchNamespaces")]
    public partial class SR {
        
        private static global::System.Resources.ResourceManager _resourceManager;
        
        private static object _internalSyncObject;
        
        private static global::System.Globalization.CultureInfo _resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public SR() {
        }
        
        /// <summary>
        /// Thread safe lock object used by this class.
        /// </summary>
        public static object InternalSyncObject {
            get {
                if (object.ReferenceEquals(_internalSyncObject, null)) {
                    global::System.Threading.Interlocked.CompareExchange(ref _internalSyncObject, new object(), null);
                }
                return _internalSyncObject;
            }
        }
        
        /// <summary>
        /// Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(_resourceManager, null)) {
                    global::System.Threading.Monitor.Enter(InternalSyncObject);
                    try {
                        if (object.ReferenceEquals(_resourceManager, null)) {
                            global::System.Threading.Interlocked.Exchange(ref _resourceManager, new global::System.Resources.ResourceManager("Jellyfish.Virtu.Properties.SR", typeof(SR).Assembly));
                        }
                    }
                    finally {
                        global::System.Threading.Monitor.Exit(InternalSyncObject);
                    }
                }
                return _resourceManager;
            }
        }
        
        /// <summary>
        /// Overrides the current thread's CurrentUICulture property for all
        /// resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Globalization.CultureInfo Culture {
            get {
                return _resourceCulture;
            }
            set {
                _resourceCulture = value;
            }
        }
        
        /// <summary>
        /// Looks up a localized string similar to 'Rom &apos;{0}&apos; invalid.'.
        /// </summary>
        public static string RomInvalid {
            get {
                return ResourceManager.GetString(ResourceNames.RomInvalid, _resourceCulture);
            }
        }
        
        /// <summary>
        /// Looks up a localized string similar to 'Service type &apos;{0}&apos; already present.'.
        /// </summary>
        public static string ServiceAlreadyPresent {
            get {
                return ResourceManager.GetString(ResourceNames.ServiceAlreadyPresent, _resourceCulture);
            }
        }
        
        /// <summary>
        /// Looks up a localized string similar to 'Service type &apos;{0}&apos; must be assignable from service provider &apos;{1}&apos;.'.
        /// </summary>
        public static string ServiceMustBeAssignable {
            get {
                return ResourceManager.GetString(ResourceNames.ServiceMustBeAssignable, _resourceCulture);
            }
        }
        
        /// <summary>
        /// Formats a localized string similar to 'Rom &apos;{0}&apos; invalid.'.
        /// </summary>
        /// <param name="arg0">An object (0) to format.</param>
        /// <returns>A copy of format string in which the format items have been replaced by the String equivalent of the corresponding instances of Object in arguments.</returns>
        public static string RomInvalidFormat(object arg0) {
            return string.Format(_resourceCulture, RomInvalid, arg0);
        }
        
        /// <summary>
        /// Formats a localized string similar to 'Service type &apos;{0}&apos; already present.'.
        /// </summary>
        /// <param name="arg0">An object (0) to format.</param>
        /// <returns>A copy of format string in which the format items have been replaced by the String equivalent of the corresponding instances of Object in arguments.</returns>
        public static string ServiceAlreadyPresentFormat(object arg0) {
            return string.Format(_resourceCulture, ServiceAlreadyPresent, arg0);
        }
        
        /// <summary>
        /// Formats a localized string similar to 'Service type &apos;{0}&apos; must be assignable from service provider &apos;{1}&apos;.'.
        /// </summary>
        /// <param name="arg0">An object (0) to format.</param>
        /// <param name="arg1">An object (1) to format.</param>
        /// <returns>A copy of format string in which the format items have been replaced by the String equivalent of the corresponding instances of Object in arguments.</returns>
        public static string ServiceMustBeAssignableFormat(object arg0, object arg1) {
            return string.Format(_resourceCulture, ServiceMustBeAssignable, arg0, arg1);
        }
        
        /// <summary>
        /// Lists all the resource names as constant string fields.
        /// </summary>
        public class ResourceNames {
            
            /// <summary>
            /// Stores the resource name 'RomInvalid'.
            /// </summary>
            public const string RomInvalid = "RomInvalid";
            
            /// <summary>
            /// Stores the resource name 'ServiceAlreadyPresent'.
            /// </summary>
            public const string ServiceAlreadyPresent = "ServiceAlreadyPresent";
            
            /// <summary>
            /// Stores the resource name 'ServiceMustBeAssignable'.
            /// </summary>
            public const string ServiceMustBeAssignable = "ServiceMustBeAssignable";
        }
    }
}
