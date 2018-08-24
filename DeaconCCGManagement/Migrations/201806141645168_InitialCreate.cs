namespace DeaconCCGManagement.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CCGs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CCGName = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.CCGAppUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        FirstName = c.String(nullable: false),
                        LastName = c.String(nullable: false),
                        EmailAddress = c.String(),
                        CellNumber = c.String(),
                        ChangeRequestManager = c.Boolean(nullable: false),
                        CcgId = c.Int(),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.CCGs", t => t.CcgId)
                .Index(t => t.CcgId)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.CCGClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.CCGAppUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.CCGLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.CCGAppUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.CCGAppUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.CCGAppUsers", t => t.UserId, cascadeDelete: true)
                .ForeignKey("dbo.Roles", t => t.RoleId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.CCGMembers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FamilyNumber = c.String(),
                        EnvelopNumber = c.String(),
                        FamDistrictDeacon = c.String(),
                        IndividualId = c.String(),
                        ZionEntryDate = c.DateTime(),
                        DateLastChanged = c.DateTime(),
                        LastDateContacted = c.DateTime(),
                        CcgId = c.Int(),
                        FirstName = c.String(nullable: false, maxLength: 160),
                        LastName = c.String(nullable: false, maxLength: 160),
                        Title = c.String(maxLength: 50),
                        Suffix = c.String(maxLength: 50),
                        Address = c.String(nullable: false, maxLength: 500),
                        City = c.String(nullable: false, maxLength: 160),
                        State = c.String(nullable: false, maxLength: 50),
                        ZipCode = c.String(nullable: false, maxLength: 50),
                        PhoneNumber = c.String(),
                        CellPhoneNumber = c.String(),
                        BirthDate = c.DateTime(),
                        EmailAddress = c.String(),
                        DateJoinedZion = c.DateTime(),
                        InactiveDate = c.DateTime(),
                        AnniversaryDate = c.DateTime(),
                        IsMemberActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.CCGs", t => t.CcgId)
                .Index(t => t.CcgId);
            
            CreateTable(
                "dbo.ContactRecords",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Private = c.Boolean(nullable: false),
                        ContactDate = c.DateTime(nullable: false),
                        Timestamp = c.DateTime(nullable: false),
                        Duration = c.Time(precision: 7),
                        PassAlong = c.Boolean(nullable: false),
                        PassAlongComments = c.String(maxLength: 1000),
                        PassAlongFollowUpComments = c.String(maxLength: 1000),
                        Subject = c.String(maxLength: 160),
                        Comments = c.String(maxLength: 1000),
                        Archive = c.Boolean(nullable: false),
                        ContactTypeId = c.Int(nullable: false),
                        AppUserId = c.String(maxLength: 128),
                        CCGMemberId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.CCGAppUsers", t => t.AppUserId)
                .ForeignKey("dbo.CCGMembers", t => t.CCGMemberId, cascadeDelete: true)
                .ForeignKey("dbo.ContactTypes", t => t.ContactTypeId, cascadeDelete: true)
                .Index(t => t.ContactTypeId)
                .Index(t => t.AppUserId)
                .Index(t => t.CCGMemberId);
            
            CreateTable(
                "dbo.ContactTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 160),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.PassAlongContacts",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        Archive = c.Boolean(nullable: false),
                        Timestamp = c.DateTime(nullable: false),
                        PassAlongEmailSent = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ContactRecords", t => t.Id)
                .Index(t => t.Id);
            
            CreateTable(
                "dbo.ChangeRequests",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CRDate = c.DateTime(nullable: false),
                        CcgMemberId = c.Int(nullable: false),
                        DeaconId = c.String(maxLength: 128),
                        FirstName = c.String(nullable: false, maxLength: 160),
                        LastName = c.String(nullable: false, maxLength: 160),
                        Title = c.String(maxLength: 50),
                        Suffix = c.String(maxLength: 50),
                        Address = c.String(nullable: false, maxLength: 500),
                        City = c.String(nullable: false, maxLength: 160),
                        State = c.String(nullable: false, maxLength: 50),
                        ZipCode = c.String(nullable: false, maxLength: 50),
                        PhoneNumber = c.String(),
                        CellPhoneNumber = c.String(),
                        BirthDate = c.DateTime(),
                        EmailAddress = c.String(),
                        DateJoinedZion = c.DateTime(),
                        InactiveDate = c.DateTime(),
                        AnniversaryDate = c.DateTime(),
                        IsMemberActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.CCGMembers", t => t.CcgMemberId, cascadeDelete: true)
                .ForeignKey("dbo.CCGAppUsers", t => t.DeaconId)
                .Index(t => t.CcgMemberId)
                .Index(t => t.DeaconId);
            
            CreateTable(
                "dbo.MemberPhotoes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Photo = c.Binary(nullable: false),
                        Thumbnail = c.Binary(nullable: false),
                        MimeType = c.String(nullable: false, maxLength: 50),
                        MemberId = c.Int(),
                        AppUserId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.CCGAppUsers", t => t.AppUserId)
                .ForeignKey("dbo.CCGMembers", t => t.MemberId)
                .Index(t => t.MemberId)
                .Index(t => t.AppUserId);
            
            CreateTable(
                "dbo.NeedsCommunion",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        MemberId = c.Int(nullable: false),
                        Timestamp = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.CCGMembers", t => t.MemberId, cascadeDelete: true)
                .Index(t => t.MemberId);
            
            CreateTable(
                "dbo.Roles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CCGAppUserRoles", "RoleId", "dbo.Roles");
            DropForeignKey("dbo.NeedsCommunion", "MemberId", "dbo.CCGMembers");
            DropForeignKey("dbo.MemberPhotoes", "MemberId", "dbo.CCGMembers");
            DropForeignKey("dbo.MemberPhotoes", "AppUserId", "dbo.CCGAppUsers");
            DropForeignKey("dbo.ChangeRequests", "DeaconId", "dbo.CCGAppUsers");
            DropForeignKey("dbo.ChangeRequests", "CcgMemberId", "dbo.CCGMembers");
            DropForeignKey("dbo.CCGMembers", "CcgId", "dbo.CCGs");
            DropForeignKey("dbo.PassAlongContacts", "Id", "dbo.ContactRecords");
            DropForeignKey("dbo.ContactRecords", "ContactTypeId", "dbo.ContactTypes");
            DropForeignKey("dbo.ContactRecords", "CCGMemberId", "dbo.CCGMembers");
            DropForeignKey("dbo.ContactRecords", "AppUserId", "dbo.CCGAppUsers");
            DropForeignKey("dbo.CCGAppUserRoles", "UserId", "dbo.CCGAppUsers");
            DropForeignKey("dbo.CCGLogins", "UserId", "dbo.CCGAppUsers");
            DropForeignKey("dbo.CCGClaims", "UserId", "dbo.CCGAppUsers");
            DropForeignKey("dbo.CCGAppUsers", "CcgId", "dbo.CCGs");
            DropIndex("dbo.Roles", "RoleNameIndex");
            DropIndex("dbo.NeedsCommunion", new[] { "MemberId" });
            DropIndex("dbo.MemberPhotoes", new[] { "AppUserId" });
            DropIndex("dbo.MemberPhotoes", new[] { "MemberId" });
            DropIndex("dbo.ChangeRequests", new[] { "DeaconId" });
            DropIndex("dbo.ChangeRequests", new[] { "CcgMemberId" });
            DropIndex("dbo.PassAlongContacts", new[] { "Id" });
            DropIndex("dbo.ContactRecords", new[] { "CCGMemberId" });
            DropIndex("dbo.ContactRecords", new[] { "AppUserId" });
            DropIndex("dbo.ContactRecords", new[] { "ContactTypeId" });
            DropIndex("dbo.CCGMembers", new[] { "CcgId" });
            DropIndex("dbo.CCGAppUserRoles", new[] { "RoleId" });
            DropIndex("dbo.CCGAppUserRoles", new[] { "UserId" });
            DropIndex("dbo.CCGLogins", new[] { "UserId" });
            DropIndex("dbo.CCGClaims", new[] { "UserId" });
            DropIndex("dbo.CCGAppUsers", "UserNameIndex");
            DropIndex("dbo.CCGAppUsers", new[] { "CcgId" });
            DropTable("dbo.Roles");
            DropTable("dbo.NeedsCommunion");
            DropTable("dbo.MemberPhotoes");
            DropTable("dbo.ChangeRequests");
            DropTable("dbo.PassAlongContacts");
            DropTable("dbo.ContactTypes");
            DropTable("dbo.ContactRecords");
            DropTable("dbo.CCGMembers");
            DropTable("dbo.CCGAppUserRoles");
            DropTable("dbo.CCGLogins");
            DropTable("dbo.CCGClaims");
            DropTable("dbo.CCGAppUsers");
            DropTable("dbo.CCGs");
        }
    }
}
