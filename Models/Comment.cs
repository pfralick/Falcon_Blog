using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Falcon_Blog.Models
{
    public class Comment
    {
        //The PK (primary key) of the Comment
        public int Id { get; set; }

        //This is the PK of the BlogPost entry that this comment belongs to
        public int BlogPostId { get; set; }


        //What is the PK of the User that wrote this Comment
        public string AuthorId { get; set; }

        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }

        public string UpdateReason { get; set; }
        public string Body { get; set; }

        //Navigational properties
        public virtual BlogPost BlogPost { get; set; }
        public virtual ApplicationUser Author { get; set; }

        //This doesn't do anything so we dont need to write it

    }
}