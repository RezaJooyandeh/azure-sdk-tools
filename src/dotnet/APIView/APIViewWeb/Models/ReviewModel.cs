﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using APIViewWeb.Models;
using APIViewWeb.Repositories;
using Newtonsoft.Json;

namespace APIViewWeb
{
    public class ReviewModel
    {
        private bool _runAnalysis;

        public ReviewModel()
        {
            Revisions = new ReviewRevisionModelList(this);
        }

        [JsonProperty("id")]
        public string ReviewId { get; set; } = IdHelper.GenerateId();

        public string Name { get; set; }
        public string Author { get; set; }
        public DateTime CreationDate { get; set; }
        public ReviewRevisionModelList Revisions { get; set; }

        [Obsolete("Back compat")]
        public List<ReviewCodeFileModel> Files { get; set; } = new List<ReviewCodeFileModel>();

        [JsonIgnore]
        public bool UpdateAvailable { get; set; }

        public bool RunAnalysis
        {
#pragma warning disable 618
            get => _runAnalysis || Revisions.SelectMany(r=>r.Files).Any(f => f.RunAnalysis);
#pragma warning restore 618
            set => _runAnalysis = value;
        }

        public bool IsClosed { get; set; }

        public HashSet<string> Subscribers { get; set; } = new HashSet<string>();

        public bool IsUserSubscribed(ClaimsPrincipal user)
        {
            string email = GetUserEmail(user);
            if (email != null)
            {
                return Subscribers.Contains(email);
            }
            return false;
        }

        public string GetUserEmail(ClaimsPrincipal user) =>
            NotificationManager.GetUserEmail(user);

        [JsonIgnore]
        public string DisplayName
        {
            get
            {
                var label = Revisions.FirstOrDefault()?.Label;
                return label != null ?
                    $"{Name} - {label}" :
                    Name;
            }
        }

        [JsonIgnore]
        public DateTime LastUpdated => Revisions.LastOrDefault()?.CreationDate ?? CreationDate;

        [JsonIgnore]
        public string Language => Revisions.FirstOrDefault()?.Files.FirstOrDefault()?.Language;
    }
}