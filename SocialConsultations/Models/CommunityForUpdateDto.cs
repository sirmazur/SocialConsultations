﻿using SocialConsultations.Entities;

namespace SocialConsultations.Models
{
    public class CommunityForUpdateDto
    {

        public string Name { get; set; }

        public string Description { get; set; }

        public FileData? Avatar { get; set; }

        public FileData? Background { get; set; }

        public List<User> Administrators { get; set; }

        public List<User> Members { get; set; }

        public List<Issue> Issues { get; set; }

        public List<JoinRequest> JoinRequests { get; set; }

        public bool IsPublic { get; set; }
    }
}
