using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Security.Permissions;
using System.Security.Principal;

namespace Jellyfish.Library
{
    [StructLayout(LayoutKind.Sequential)]
    public sealed class SecurityAttributes
    {
        [SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode = true)]
        public SecurityAttributes()
        {
            _length = Marshal.SizeOf(typeof(SecurityAttributes));
        }

        public int Length { get { return _length; } }
        public IntPtr SecurityDescriptor { get { return _securityDescriptor; } set { _securityDescriptor = value; } }
        public bool InheritHandle { get { return _inheritHandle; } set { _inheritHandle = value; } }

        private int _length;
        private IntPtr _securityDescriptor;
        private bool _inheritHandle;
    }

    public sealed class GeneralAccessRule : AccessRule
    {
        public GeneralAccessRule(IdentityReference identity, int rights, AccessControlType type) : 
            base(identity, rights, false, InheritanceFlags.None, PropagationFlags.None, type)
        {
        }

        public GeneralAccessRule(IdentityReference identity, int rights, InheritanceFlags inheritance, PropagationFlags propagation, AccessControlType type) : 
            base(identity, rights, false, inheritance, propagation, type)
        {
        }

        public GeneralAccessRule(IdentityReference identity, int rights, bool isInherited, InheritanceFlags inheritance, PropagationFlags propagation, 
            AccessControlType type) : 
            base(identity, rights, isInherited, inheritance, propagation, type)
        {
        }

        public int AccessRights { get { return AccessMask; } }
    }

    public sealed class GeneralAuditRule : AuditRule
    {
        public GeneralAuditRule(IdentityReference identity, int rights, AuditFlags audit) : 
            base(identity, rights, false, InheritanceFlags.None, PropagationFlags.None, audit)
        {
        }

        public GeneralAuditRule(IdentityReference identity, int rights, InheritanceFlags inheritance, PropagationFlags propagation, AuditFlags audit) : 
            base(identity, rights, false, inheritance, propagation, audit)
        {
        }

        public GeneralAuditRule(IdentityReference identity, int rights, bool isInherited, InheritanceFlags inheritance, PropagationFlags propagation, 
            AuditFlags audit) : 
            base(identity, rights, isInherited, inheritance, propagation, audit)
        {
        }

        public int AccessRights { get { return AccessMask; } }
    }

    public sealed class GeneralSecurity : NativeObjectSecurity
    {
        public GeneralSecurity(bool isContainer, ResourceType resourceType) : 
            base(isContainer, resourceType)
        {
        }

        public GeneralSecurity(bool isContainer, ResourceType resourceType, SafeHandle handle) : 
            base(isContainer, resourceType, handle, AccessControlSections.Access | AccessControlSections.Group | AccessControlSections.Owner)
        {
        }

        public GeneralSecurity(bool isContainer, ResourceType resourceType, SafeHandle handle, AccessControlSections includeSections) : 
            base(isContainer, resourceType, handle, includeSections)
        {
        }

        public GeneralSecurity(bool isContainer, ResourceType resourceType, string name) : 
            base(isContainer, resourceType, name, AccessControlSections.Access | AccessControlSections.Group | AccessControlSections.Owner)
        {
        }

        public GeneralSecurity(bool isContainer, ResourceType resourceType, string name, AccessControlSections includeSections) : 
            base(isContainer, resourceType, name, includeSections)
        {
        }

        public override AccessRule AccessRuleFactory(IdentityReference identityReference, int accessMask, bool isInherited, InheritanceFlags inheritanceFlags, 
            PropagationFlags propagationFlags, AccessControlType type)
        {
            return new GeneralAccessRule(identityReference, accessMask, isInherited, inheritanceFlags, propagationFlags, type);
        }

        public override AuditRule AuditRuleFactory(IdentityReference identityReference, int accessMask, bool isInherited, InheritanceFlags inheritanceFlags, 
            PropagationFlags propagationFlags, AuditFlags flags)
        {
            return new GeneralAuditRule(identityReference, accessMask, isInherited, inheritanceFlags, propagationFlags, flags);
        }

        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
        public void AddAccessRule(GeneralAccessRule rule)
        {
            base.AddAccessRule(rule);
        }

        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
        public void AddAuditRule(GeneralAuditRule rule)
        {
            base.AddAuditRule(rule);
        }

        [SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode = true)]
        public void GetSecurityAttributes(bool inheritable, Action<SecurityAttributes> action)
        {
            GetSecurityAttributes(this, inheritable, action);
        }

        [SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode = true)]
        public static void GetSecurityAttributes(ObjectSecurity security, bool inheritable, Action<SecurityAttributes> action)
        {
            if (security != null)
            {
                GCHandleHelpers.Pin(security.GetSecurityDescriptorBinaryForm(), securityDescriptor => 
                {
                    action(new SecurityAttributes() { SecurityDescriptor = securityDescriptor, InheritHandle = inheritable });
                });
            }
            else if (inheritable)
            {
                action(new SecurityAttributes() { InheritHandle = inheritable });
            }
            else
            {
                action(null);
            }
        }

        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
        public bool RemoveAccessRule(GeneralAccessRule rule)
        {
            return base.RemoveAccessRule(rule);
        }

        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
        public void RemoveAccessRuleAll(GeneralAccessRule rule)
        {
            base.RemoveAccessRuleAll(rule);
        }

        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
        public void RemoveAccessRuleSpecific(GeneralAccessRule rule)
        {
            base.RemoveAccessRuleSpecific(rule);
        }

        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
        public bool RemoveAuditRule(GeneralAuditRule rule)
        {
            return base.RemoveAuditRule(rule);
        }

        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
        public void RemoveAuditRuleAll(GeneralAuditRule rule)
        {
            base.RemoveAuditRuleAll(rule);
        }

        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
        public void RemoveAuditRuleSpecific(GeneralAuditRule rule)
        {
            base.RemoveAuditRuleSpecific(rule);
        }

        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
        public void ResetAccessRule(GeneralAccessRule rule)
        {
            base.ResetAccessRule(rule);
        }

        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
        public void SetAccessRule(GeneralAccessRule rule)
        {
            base.SetAccessRule(rule);
        }

        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
        public void SetAuditRule(GeneralAuditRule rule)
        {
            base.SetAuditRule(rule);
        }

        public void Persist(SafeHandle handle)
        {
            WriteLock();
            try
            {
                AccessControlSections sectionsModified = GetAccessControlSectionsModified();
                if (sectionsModified != AccessControlSections.None)
                {
                    Persist(handle, sectionsModified);
                    ResetAccessControlSectionsModified();
                }
            }
            finally
            {
                WriteUnlock();
            }
        }

        public void Persist(string name)
        {
            WriteLock();
            try
            {
                AccessControlSections sectionsModified = GetAccessControlSectionsModified();
                if (sectionsModified != AccessControlSections.None)
                {
                    Persist(name, sectionsModified);
                    ResetAccessControlSectionsModified();
                }
            }
            finally
            {
                WriteUnlock();
            }
        }

        private AccessControlSections GetAccessControlSectionsModified()
        {
            AccessControlSections sectionsModified = AccessControlSections.None;
            if (AccessRulesModified)
            {
                sectionsModified = AccessControlSections.Access;
            }
            if (AuditRulesModified)
            {
                sectionsModified |= AccessControlSections.Audit;
            }
            if (OwnerModified)
            {
                sectionsModified |= AccessControlSections.Owner;
            }
            if (GroupModified)
            {
                sectionsModified |= AccessControlSections.Group;
            }

            return sectionsModified;
        }

        private void ResetAccessControlSectionsModified()
        {
            AccessRulesModified = false;
            AuditRulesModified = false;
            OwnerModified = false;
            GroupModified = false;
        }

        public override Type AccessRightType { get { return typeof(int); } }
        public override Type AccessRuleType { get { return typeof(GeneralAccessRule); } }
        public override Type AuditRuleType { get { return typeof(GeneralAuditRule); } }
    }
}
