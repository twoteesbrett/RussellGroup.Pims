﻿using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RussellGroup.Pims.DataAccess.Models
{
    public class ApplicationRole : IdentityRole
    {
        public const string CanView = "CanView";
        public const string CanEdit = "CanEdit";
        public const string CanEditCategories = "CanEditCategories";
        public const string CanEditUsers = "CanEditUsers";

        [NotMapped]
        public bool IsChecked { get; set; }

        [DisplayName("description")]
        public string Description { get; set; }
    }
}
