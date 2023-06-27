using System;
using System.Collections.Generic;

namespace ChatApp.Data
{
    public static class ApplicationClaimTypes
    {
        public readonly static List<string> AppClaimTypes = new() {
            "Admin",
            "ChatModerator",
            "User",
            "Disabled"
        };
    }
}
