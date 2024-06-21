using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace OrderService.Domain.SeedWork
{
    public abstract class Enumeration : IComparable
    {
        public string Name { get; set; }
        public int Id { get; set; }

        protected Enumeration(int id, string name) => (Id, Name) = (id, name);
        public override string ToString() => Name;

        public static IEnumerable<T> GetAll<T>() where T : Enumeration =>
            typeof(T).GetFields(BindingFlags.Public |
                                BindingFlags.Static |
                                BindingFlags.DeclaredOnly).Select(f => f.GetValue(null)).Cast<T>();

        public override bool Equals(object? obj)
        {
            if(obj is not Enumeration othervalue)
            {
                return false;
            }

            var typeMatches=GetType().Equals(obj.GetType());
            var valueMathces = Id.Equals(othervalue.Id);

            return typeMatches && valueMathces;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
        public static int AbsoluteDifference(Enumeration firstValue,Enumeration secondValue)
        {
            var absoluteDifference = Math.Abs(firstValue.Id - secondValue.Id);
            return absoluteDifference;
        }
        public static T FromValue<T>(int value) where T : Enumeration
        {
            var matchingItem = Parse<T, int>(value, "value", item => item.Id == value);
            return matchingItem;
        }
        public static T FromDisplayName<T>(string displayName)where T : Enumeration
        {
            var matchingItem = Parse<T, string>(displayName, "display name", item => item.Name == displayName);
            return matchingItem;

        }
        private static T Parse<T,K>(K value,string description,Func<T,bool>predicate)where T : Enumeration
        {
            var matchingItem = GetAll<T>().FirstOrDefault(predicate);   
            if (matchingItem == null)
            {
                throw new InvalidOperationException($"{value} is not valid {description} in  {typeof(T)}");
            }
            return matchingItem;
        }
        public int CompareTo(object? obj)
        {
            return Id.CompareTo(((Enumeration)obj).Id);
        }
    }
}
