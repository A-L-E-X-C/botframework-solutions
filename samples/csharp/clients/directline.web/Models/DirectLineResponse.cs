﻿// Copyright(c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace DirectLine.Web.Models
{
    public class DirectLineResponse
    {
        public string conversationId { get; set; }

        public string token { get; set; }

        public int expires_in { get; set; }
    }
}
