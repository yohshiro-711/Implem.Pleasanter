﻿using Implem.IRds;
using Implem.Libraries.Utilities;
using System;
using System.Collections.Generic;

namespace Implem.PostgreSql
{
    internal class PostgreSqlSqls : ISqls
    {
        public string TrueString { get; } = "true";

        public string FalseString { get; } = "false";

        public string IsNotTrue { get; } = " is not true ";

        public string CurrentDateTime { get; } = " CURRENT_TIMESTAMP ";

        public string Like { get; } = " ilike ";

        public string NotLike { get; } = " not ilike ";

        public string LikeWithEscape { get; } = " ilike {0} escape '|'";

        public string NotLikeWithEscape { get; } = " not ilike {0} escape '|'";

        public string Escape { get; } = " escape '|'";

        public string EscapeValue(string value)
        {
            return value?
                .Replace("|", "||")
                .Replace("_", "|_")
                .Replace("%", "|%");
        }

        public string IsNull { get; } = "coalesce";

        public string WhereLikeTemplateForward { get; } = "'%' || ";

        public string WhereLikeTemplate { get; } = "#ParamCount#_#CommandCount# || '%'";

        public string GenerateIdentity { get; } = " generated by default as identity (start with {0} increment by 1)";

        public object DateTimeValue(object value)
        {
            return value != null &&
                !(value is DateTime) &&
                DateTime.TryParse(value.ToString(), out var data)
                ? data
                : value;
        }

        public string BooleanString(string bit)
        {
            return bit == "1" ? TrueString : FalseString;
        }

        public string IntegerColumnLike(string tableName, string columnName)
        {
            return "(cast(\"" + tableName + "\".\"" + columnName + "\" as text) like ";
        }

        public string DateAddDay(int day, string columnBracket)
        {
            return $"\"{columnBracket}\" + cast('{day} days' as interval)";
        }

        public string DateAddHour(int hour, string columnBracket)
        {
            return $"{columnBracket} + interval '{hour} hour'";
        }

        public string DateGroupYearly { get; } = "to_char({0}, 'YYYY')";

        public string DateGroupMonthly { get; } = "to_char({0}, 'YYYY/MM')";

        public string DateGroupWeeklyPart { get; } = "case date_part('dow',{0}) when 0 then {0} + '-6 days' else {0} + CAST((1-date_part('dow',{0})) || 'days' as interval) end";

        public string DateGroupWeekly { get; } ="date_part('year',{0}) * 100 + date_part('week',{0})";

        public string DateGroupDaily { get; } = "to_char({0}, 'YYYY/MM/DD')";

        public string GetPermissions { get; } = @"
            select distinct
                ""Sites"".""SiteId"" as ""ReferenceId"",
                ""Permissions"".""PermissionType"" 
            from ""Sites""
                inner join ""Permissions"" on ""Permissions"".""ReferenceId""=""Sites"".""InheritPermission""
                inner join ""Depts"" on ""Permissions"".""DeptId""=""Depts"".""DeptId""
            where ""Sites"".""TenantId""=@ipT
                and ""Depts"".""DeptId""=@ipD
                and ""Depts"".""Disabled""='false'
            union all
            select distinct
                ""Sites"".""SiteId"" as ""ReferenceId"",
                ""Permissions"".""PermissionType"" 
            from ""Sites""
                inner join ""Permissions"" on ""Permissions"".""ReferenceId""=""Sites"".""InheritPermission""
                inner join ""Groups"" on ""Permissions"".""GroupId""=""Groups"".""GroupId""
                inner join ""GroupMembers"" on ""Groups"".""GroupId""=""GroupMembers"".""GroupId""
                inner join ""Depts"" on ""GroupMembers"".""DeptId""=""Depts"".""DeptId""
            where ""Sites"".""TenantId""=@ipT
                and ""Groups"".""Disabled""='false'
                and ""Depts"".""DeptId""=@ipD
                and ""Depts"".""Disabled""='false'
            union all
            select distinct
                ""Sites"".""SiteId"" as ""ReferenceId"",
                ""Permissions"".""PermissionType"" 
            from ""Sites""
                inner join ""Permissions"" on ""Permissions"".""ReferenceId""=""Sites"".""InheritPermission""
                inner join ""Groups"" on ""Permissions"".""GroupId""=""Groups"".""GroupId""
                inner join ""GroupMembers"" on ""Groups"".""GroupId""=""GroupMembers"".""GroupId""
                inner join ""Users"" on ""GroupMembers"".""UserId""=""Users"".""UserId""
            where ""Sites"".""TenantId""=@ipT
                and ""Groups"".""Disabled""='false'
                and ""Users"".""UserId""=@ipU
                and ""Users"".""Disabled""='false'
            union all
            select distinct
                ""Sites"".""SiteId"" as ""ReferenceId"",
                ""Permissions"".""PermissionType""
            from ""Sites""
                inner join ""Permissions"" on ""Permissions"".""ReferenceId""=""Sites"".""InheritPermission""
            where ""Sites"".""TenantId""=@ipT
                and ""Permissions"".""UserId"" > 0
                and ""Permissions"".""UserId""=@ipU
            union all
            select distinct
                ""Sites"".""SiteId"" as ""ReferenceId"",
                ""Permissions"".""PermissionType""
            from ""Sites""
                inner join ""Permissions"" on ""Permissions"".""ReferenceId""=""Sites"".""InheritPermission""
            where ""Sites"".""TenantId""=@ipT
                and ""Permissions"".""UserId""=-1";

        public string GetPermissionsById { get; } = @"
            union all
            select distinct
                ""Items"".""ReferenceId"",
                ""Permissions"".""PermissionType"" 
            from ""Items""
                inner join ""Sites"" on ""Items"".""SiteId""=""Sites"".""SiteId""
                inner join ""Permissions"" on ""Permissions"".""ReferenceId""=""Items"".""ReferenceId""
                inner join ""Depts"" on ""Permissions"".""DeptId""=""Depts"".""DeptId""
            where ""Items"".""ReferenceId""=@ReferenceId
                and ""Sites"".""TenantId""=@ipT
                and ""Depts"".""DeptId""=@ipD
                and ""Depts"".""Disabled""='false'
            union all
            select distinct
                ""Items"".""ReferenceId"",
                ""Permissions"".""PermissionType"" 
            from ""Items""
                inner join ""Sites"" on ""Items"".""SiteId""=""Sites"".""SiteId""
                inner join ""Permissions"" on ""Permissions"".""ReferenceId""=""Items"".""ReferenceId""
                inner join ""Groups"" on ""Permissions"".""GroupId""=""Groups"".""GroupId""
                inner join ""GroupMembers"" on ""Groups"".""GroupId""=""GroupMembers"".""GroupId""
                inner join ""Depts"" on ""GroupMembers"".""DeptId""=""Depts"".""DeptId""
            where ""Items"".""ReferenceId""=@ReferenceId
                and ""Sites"".""TenantId""=@ipT
                and ""Groups"".""Disabled""='false'
                and ""Depts"".""DeptId""=@ipD
                and ""Depts"".""Disabled""='false'
            union all
            select distinct
                ""Items"".""ReferenceId"",
                ""Permissions"".""PermissionType"" 
            from ""Items""
                inner join ""Sites"" on ""Items"".""SiteId""=""Sites"".""SiteId""
                inner join ""Permissions"" on ""Permissions"".""ReferenceId""=""Items"".""ReferenceId""
                inner join ""Groups"" on ""Permissions"".""GroupId""=""Groups"".""GroupId""
                inner join ""GroupMembers"" on ""Groups"".""GroupId""=""GroupMembers"".""GroupId""
                inner join ""Users"" on ""GroupMembers"".""UserId""=""Users"".""UserId""
            where ""Items"".""ReferenceId""=@ReferenceId
                and ""Sites"".""TenantId""=@ipT
                and ""Groups"".""Disabled""='false'
                and ""Users"".""UserId""=@ipU
                and ""Users"".""Disabled""='false'
            union all
            select distinct
                ""Items"".""ReferenceId"",
                ""Permissions"".""PermissionType""
            from ""Items""
                inner join ""Sites"" on ""Items"".""SiteId""=""Sites"".""SiteId""
                inner join ""Permissions"" on ""Permissions"".""ReferenceId""=""Items"".""ReferenceId""
            where ""Items"".""ReferenceId""=@ReferenceId
                and ""Sites"".""TenantId""=@ipT
                and ""Permissions"".""UserId"" > 0
                and ""Permissions"".""UserId""=@ipU
            union all
            select distinct
                ""Items"".""ReferenceId"",
                ""Permissions"".""PermissionType""
            from ""Items""
                inner join ""Sites"" on ""Items"".""SiteId""=""Sites"".""SiteId""
                inner join ""Permissions"" on ""Permissions"".""ReferenceId""=""Items"".""ReferenceId""
            where ""Items"".""ReferenceId""=@ReferenceId
                and ""Sites"".""TenantId""=@ipT
                and ""Permissions"".""UserId""=-1;";

        public string GetGroup { get; } = @"
            select ""Groups"".""GroupId"" 
            from ""Groups"" as ""Groups""
                inner join ""GroupMembers"" on ""Groups"".""GroupId""=""GroupMembers"".""GroupId""
                inner join ""Depts"" on ""GroupMembers"".""DeptId""=""Depts"".""DeptId""
            where ""Depts"".""TenantId""=@ipT
                and ""Depts"".""DeptId""=@ipD
            union all
            select ""Groups"".""GroupId"" 
            from ""Groups"" as ""Groups""
                inner join ""GroupMembers"" on ""Groups"".""GroupId""=""GroupMembers"".""GroupId""
                inner join ""Users"" on ""GroupMembers"".""UserId""=""Users"".""UserId""
            where ""Users"".""TenantId""=@ipT
                and ""Users"".""UserId""=@ipU;";

        public string PermissionsWhere { get; } = @"
            (
                exists
                (
                    select ""Depts"".""DeptId"" as ""Id""
                    from ""Depts""
                    where ""Depts"".""TenantId""=@ipT
                        and ""Depts"".""DeptId""=@ipD
                        and ""Depts"".""Disabled""='false'
                        and ""Permissions"".""DeptId""=""Depts"".""DeptId""
                        and @ipD<>0
                    union all
                    select ""Groups"".""GroupId"" as ""Id""
                    from ""Groups"" inner join ""GroupMembers"" on ""Groups"".""GroupId""=""GroupMembers"".""GroupId""
                    where ""Groups"".""TenantId""=@ipT
                        and ""Groups"".""Disabled""='false'
                        and ""Permissions"".""GroupId""=""Groups"".""GroupId""
                        and exists
                        (
                            select ""DeptId""
                            from ""Depts""
                            where ""Depts"".""TenantId""=@ipT
                                and ""Depts"".""DeptId""=@ipD
                                and ""Depts"".""Disabled""='false'
                                and ""GroupMembers"".""DeptId""=""Depts"".""DeptId""
                                and @ipD<>0
                        )
                    union all
                    select ""Groups"".""GroupId"" as ""Id""
                    from ""Groups"" inner join ""GroupMembers"" on ""Groups"".""GroupId""=""GroupMembers"".""GroupId""
                    where ""Groups"".""TenantId""=@ipT
                        and ""Groups"".""Disabled""='false'
                        and ""Permissions"".""GroupId""=""Groups"".""GroupId""
                        and ""GroupMembers"".""UserId""=@ipU
                        and @ipU<>0
                    union all
                    select ""P"".""UserId"" as ""Id""
                    from ""Permissions"" as ""P""
                    where ""P"".""ReferenceId""=""Permissions"".""ReferenceId""
                        and ""P"".""UserId""=""Permissions"".""UserId""
                        and ""P"".""UserId""=@ipU
                        and @ipU<>0
                    union all
                    select ""P"".""UserId"" as ""Id""
                    from ""Permissions"" as ""P""
                    where ""P"".""ReferenceId""=""Permissions"".""ReferenceId""
                        and ""P"".""UserId""=-1
                )
            )";

        public string SiteDeptWhere { get; } = @"
            (
                exists
                (
                    select *
                    from ""Permissions""
                        left outer join ""Depts"" on ""Permissions"".""DeptId""=""Depts"".""DeptId""
                        left outer join ""Groups"" on ""Permissions"".""GroupId""=""Groups"".""GroupId""
                        left outer join ""GroupMembers"" on ""Groups"".""GroupId""=""GroupMembers"".""GroupId""
                        left outer join ""Depts"" as ""GroupMemberDepts"" on ""GroupMembers"".""DeptId""=""GroupMemberDepts"".""DeptId""
                    where
                        ""Permissions"".""ReferenceId""={0}
                        and
                        (
                            (
                                ""Depts"".""Disabled""='false'
                                and ""Depts"".""DeptId""=""Depts"".""DeptId""
                            )
                            or 
                            (
                                ""Groups"".""Disabled""='false' and 
                                (
                                    (
                                        ""GroupMemberDepts"".""Disabled""='false'
                                        and ""GroupMemberDepts"".""DeptId""=""Depts"".""DeptId""
                                    )
                                )
                            )
                        )
                )
            )";

        public string SiteGroupWhere { get; } = @"
            (
                exists
                (
                    select *
                    from ""Permissions""
                    where
                        ""Permissions"".""ReferenceId""={0}
                        and ""Groups"".""Disabled""='false'
                        and ""Permissions"".""GroupId""=""Groups"".""GroupId""
                        and ""Groups"".""GroupId"">0
                )
            )";

        public string SiteUserWhere { get; } = @"
            (
                exists
                (
                    select ""Permissions"".""ReferenceId""
                    from ""Permissions""
                        inner join ""Depts"" as ""PermissionDepts"" on ""Permissions"".""DeptId""=""PermissionDepts"".""DeptId""
                        inner join ""Users"" as ""PermissionUsers"" on ""PermissionDepts"".""DeptId""=""PermissionUsers"".""DeptId""
                    where
                        ""Permissions"".""ReferenceId""={0}
                        and ""PermissionUsers"".""UserId""=""Users"".""UserId""
                        and ""PermissionDepts"".""Disabled""='false'
                        and ""PermissionUsers"".""Disabled""='false'
                    union all
                    select ""Permissions"".""ReferenceId""
                    from ""Permissions""
                        inner join ""Groups"" on ""Permissions"".""GroupId""=""Groups"".""GroupId""
                        inner join ""GroupMembers"" on ""Groups"".""GroupId""=""GroupMembers"".""GroupId""
                        inner join ""Depts"" as ""GroupMemberDepts"" on ""GroupMembers"".""DeptId""=""GroupMemberDepts"".""DeptId""
                        inner join ""Users"" as ""GroupMemberUsers"" on ""GroupMemberDepts"".""DeptId""=""GroupMemberUsers"".""DeptId""
                    where
                        ""Permissions"".""ReferenceId""={0}
                        and ""GroupMemberUsers"".""UserId""=""Users"".""UserId""
                        and ""Groups"".""Disabled""='false'
                        and ""GroupMemberDepts"".""Disabled""='false'
                        and ""GroupMemberUsers"".""Disabled""='false'
                    union all
                    select ""Permissions"".""ReferenceId""
                    from ""Permissions""
                        inner join ""Groups"" on ""Permissions"".""GroupId""=""Groups"".""GroupId""
                        inner join ""GroupMembers"" on ""Groups"".""GroupId""=""GroupMembers"".""GroupId""
                        inner join ""Users"" as ""GroupMemberUsers"" on ""GroupMembers"".""UserId""=""GroupMemberUsers"".""UserId""
                    where
                        ""Permissions"".""ReferenceId""={0}
                        and ""GroupMemberUsers"".""UserId""=""Users"".""UserId""
                        and ""Groups"".""Disabled""='false'
                        and ""GroupMemberUsers"".""Disabled""='false'
                    union all
                    select ""Permissions"".""ReferenceId""
                    from ""Permissions""
                        inner join ""Users"" as ""PermissionUsers"" on ""Permissions"".""UserId""=""PermissionUsers"".""UserId""
                    where
                        ""Permissions"".""ReferenceId""={0}
                        and ""PermissionUsers"".""UserId""=""Users"".""UserId""
                        and ""Users"".""Disabled""='false'
                        and ""PermissionUsers"".""Disabled""='false'
                    union all
                    select ""Permissions"".""ReferenceId""
                    from ""Permissions""
                    where
                        ""Permissions"".""ReferenceId""={0}
                        and ""Permissions"".""UserId""=-1
                )
            )";

        public string IntegratedSitesPermissionsWhere(string tableName, List<long> sites)
        {
            return $@"
                ""{tableName}_Items"".""SiteId"" in ({sites.Join()})
                and exists(
                    select ""Permissions"".""ReferenceId""
                    from ""Permissions""
                    where ""Permissions"".""ReferenceId""=
                        (
                            select ""Sites"".""InheritPermission""
                            from ""Sites""
                            where ""Sites"".""SiteId""=""{tableName}_Items"".""SiteId""
                        )
                        and ""Permissions"".""PermissionType"" & 1 = 1
                        and {PermissionsWhere}
                    union
                    select ""Permissions"".""ReferenceId""
                    from ""Permissions""
                    where ""Permissions"".""ReferenceId""=""{tableName}_Items"".""ReferenceId""
                        and ""Permissions"".""PermissionType"" & 1 = 1
                        and {PermissionsWhere}
                )";
        }
    }
}
