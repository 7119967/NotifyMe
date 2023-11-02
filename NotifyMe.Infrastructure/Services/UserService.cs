using System.Reflection;

using NotifyMe.Core.Entities;
using NotifyMe.Core.Interfaces.Repositories;
using NotifyMe.Core.Interfaces.Services;

namespace NotifyMe.Infrastructure.Services;

public class UserService : Service<User>, IUserService
{
    public UserService(IUnitOfWork unitOfWork) : base(unitOfWork) { }
 
    public void ApplyChanges(object source, object target)
    {
        PropertyInfo[] properties = typeof(User).GetProperties();

        foreach (PropertyInfo property in properties)
        {
            var sourceValue = property.GetValue(source);
            var targetValue = property.GetValue(target);
            
            if (sourceValue == null && targetValue == null)
            {
                continue;
            }
            
            switch (property.Name)
            {
                case "UserName":
                case "FirstName":
                case "LastName":
                case "Email":
                case "PhoneNumber":
                case "Avatar":
                case "Info":
                case "GroupId":
                    property.SetValue(target, sourceValue);
                    break;
            }
        }
    }
}