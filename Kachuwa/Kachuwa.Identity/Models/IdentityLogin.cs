using System.ComponentModel.DataAnnotations.Schema;
using Kachuwa.Data.Crud.Attribute;

namespace Kachuwa.Identity.Models
{
    [Table("IdentityLogin")]
    public class IdentityLogin
    {

        public string LoginProvider { get; set; }

        public string ProviderKey { get; set; }

        public long UserId { get; set; }

        public string Name { get; set; }

      
    }
}