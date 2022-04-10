using System;

namespace Gilzoide.EnumBitSet.Editor
{
    public static class TypeExtensions
    {
        public static Type[] GetGenericArgumentsOfBase(this Type typeArgument, Type genericBaseType)
        {
            for (Type type = typeArgument; type != null; type = type.BaseType)
            {
                if (type.IsGenericType && type.GetGenericTypeDefinition() == genericBaseType)
                {
                    return type.GetGenericArguments();
                }
            }
            
            return null;
        }
    }
}