﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Bitbird.Core.Data.Validation {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "15.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class ValidationMessages {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal ValidationMessages() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Bitbird.Core.Data.Validation.ValidationMessages", typeof(ValidationMessages).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The value must be between {0} and {1}, not including {0} and {1}. Current value: {2}..
        /// </summary>
        public static string BetweenExclusive {
            get {
                return ResourceManager.GetString("BetweenExclusive", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The value must be between {0} and {1}, including {0} and {1}. Current value: {2}..
        /// </summary>
        public static string BetweenInclusive {
            get {
                return ResourceManager.GetString("BetweenInclusive", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Cannot be changed..
        /// </summary>
        public static string CannotBeChanged {
            get {
                return ResourceManager.GetString("CannotBeChanged", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Cannot delete an item that is already deleted..
        /// </summary>
        public static string CannotDeleteDeleted {
            get {
                return ResourceManager.GetString("CannotDeleteDeleted", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Direct manipulation of the relation is not supported. Only n-to-m-relations may be manipulated directly..
        /// </summary>
        public static string DirectManipulationNToM {
            get {
                return ResourceManager.GetString("DirectManipulationNToM", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to At least one value is contained more than once..
        /// </summary>
        public static string Distinct {
            get {
                return ResourceManager.GetString("Distinct", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The value {0} is not valid for this field..
        /// </summary>
        public static string EnumValueIsDefined {
            get {
                return ResourceManager.GetString("EnumValueIsDefined", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The value must be greater than {0}. Current value: {1}..
        /// </summary>
        public static string GreaterThan {
            get {
                return ResourceManager.GetString("GreaterThan", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The value must be greater than or equal to {0}. Current value: {1}..
        /// </summary>
        public static string GreaterThanEqual {
            get {
                return ResourceManager.GetString("GreaterThanEqual", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The value must be less than {0}. Current value: {1}..
        /// </summary>
        public static string LessThan {
            get {
                return ResourceManager.GetString("LessThan", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The value must be less than or equal to {0}. Current value: {1}..
        /// </summary>
        public static string LessThanEqual {
            get {
                return ResourceManager.GetString("LessThanEqual", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The value must not be empty..
        /// </summary>
        public static string NotEmpty {
            get {
                return ResourceManager.GetString("NotEmpty", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The value must be set..
        /// </summary>
        public static string NotNull {
            get {
                return ResourceManager.GetString("NotNull", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The value must be set and not be empty..
        /// </summary>
        public static string NotNullOrEmpty {
            get {
                return ResourceManager.GetString("NotNullOrEmpty", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The value must not be set..
        /// </summary>
        public static string Null {
            get {
                return ResourceManager.GetString("Null", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The referenced entry does not exist. The referenced id was {0}..
        /// </summary>
        public static string RelatedEntryExists {
            get {
                return ResourceManager.GetString("RelatedEntryExists", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The text must not exceed {0} characters..
        /// </summary>
        public static string StringMaxLength {
            get {
                return ResourceManager.GetString("StringMaxLength", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The value contains leading or trailing whitespaces..
        /// </summary>
        public static string Trimmed {
            get {
                return ResourceManager.GetString("Trimmed", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to An entry with the value {0} already exists..
        /// </summary>
        public static string Unique_AlreadyExists {
            get {
                return ResourceManager.GetString("Unique_AlreadyExists", resourceCulture);
            }
        }
    }
}
