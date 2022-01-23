using System.ComponentModel.DataAnnotations;

namespace Shopping.Infra.Common
{
    public interface ICommand
    {
        Guid CommandIdentityToken { get; set; }
    }
    public abstract class BaseCommand : ICommand
    {

        protected BaseCommand()
        {

        }


        public virtual Guid CommandIdentityToken { get; set; }




    }

}

