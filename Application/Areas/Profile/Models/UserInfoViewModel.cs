using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Application.Infrastructure.Mapping;
using Models;

namespace Application.Areas.Profile.Models
{
    public class UserInfoViewModel: IMapTo<UserInfo>, IMapFrom<UserInfo>
    {
        [Required(ErrorMessage = "First name is required."), 
            MinLength(3, ErrorMessage = "First name should be at least 3 symbols long"), 
            MaxLength(20, ErrorMessage = "First name should be no more than 20 symbols long")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required."),
         MinLength(3, ErrorMessage = "Last name should be at least 3 symbols long"),
         MaxLength(20, ErrorMessage = "Last name should be no more than 20 symbols long")]
        public string LastName { get; set; }

        [Required]
        [DataType(DataType.PhoneNumber, ErrorMessage = "Invalid phone")]
        public string Phone { get; set; }

        [Required]
        [DataType(DataType.EmailAddress, ErrorMessage = "Invalid email.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Address is required."),
         MinLength(3, ErrorMessage = "Address should be at least 3 symbols long"),
         MaxLength(50, ErrorMessage = "Address should be no more than 20 symbols long")]
        public string Address { get; set; }

        [Required(ErrorMessage = "City is required."),
         MinLength(3, ErrorMessage = "City should be at least 3 symbols long"),
         MaxLength(20, ErrorMessage = "City should be no more than 20 symbols long")]
        public string City { get; set; }

        [Required]
        public string PostCode { get; set; }

        public string AdditionalInfo { get; set; }
    }
}
