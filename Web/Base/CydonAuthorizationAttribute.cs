using System;

namespace Web.Base
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class CydonAuthorizationAttribute : Attribute
    {
    }
}