using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projem.BOL.Entities
{
    [Table("Contact")]
    public class Contact
    {
        public int ID { get; set; }

        [StringLength(50), Column(TypeName = "varchar"), Display(Name = "Mail Gönderenin Adı Soyadı")]
        public string NameSurname { get; set; }

        [StringLength(50), Column(TypeName = "varchar"), Display(Name = "Mail Gönderenin EMail Adresi")]
        public string Email { get; set; }

        [StringLength(50), Column(TypeName = "varchar"), Display(Name = "Mailin Konusu")]
        public string Subject { get; set; }

        [StringLength(250), Column(TypeName = "varchar"), Display(Name = "Mailin İçeriği")]
        public string Message { get; set; }

        [DataType(DataType.DateTime),Display(Name = "Gönderim Tarihi")]
        public DateTime SendDate { get; set; }


    }
}
